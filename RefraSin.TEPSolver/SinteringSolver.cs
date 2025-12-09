using RefraSin.Numerics.Exceptions;
using RefraSin.ParticleModel.Pores;
using RefraSin.ParticleModel.Pores.Extensions;
using RefraSin.ParticleModel.Remeshing;
using RefraSin.ParticleModel.System;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Recovery;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.StepVectors;
using RefraSin.TEPSolver.TimeSteppers;

namespace RefraSin.TEPSolver;

/// <summary>
/// Solver for performing time integration of sintering processes based on the thermodynamic extremal principle (TEP).
/// </summary>
public class SinteringSolver : IProcessStepSolver<ISinteringStep>
{
    public SinteringSolver(ISolverRoutines routines, int remeshingEverySteps = 10)
    {
        Routines = routines;
        RemeshingEverySteps = remeshingEverySteps;
        routines.RegisterWithSolver(this);
    }

    /// <summary>
    /// Collection of subroutines to use.
    /// </summary>
    public ISolverRoutines Routines { get; }

    /// <summary>
    /// Count of time steps to compute before a remeshing is performed.
    /// </summary>
    public int RemeshingEverySteps { get; }

    /// <summary>
    /// Run the solution procedure starting with the given state till the specified time.
    /// </summary>
    public ISystemState Solve(ISinteringStep processStep, ISystemState inputState)
    {
        var session = new SolverSession(this, inputState, processStep);
        InvokeSessionInitialized(session);
        session.ReportCurrentState();
        TryTimeIntegration(ref session);

        return new SystemState(
            session.CurrentState.Id,
            session.CurrentState.Time,
            session.CurrentState.Particles
        );
    }

    private void TryTimeIntegration(ref SolverSession session)
    {
        try
        {
            DoTimeIntegration(ref session);
        }
        catch (Exception e)
        {
            session.Logger.Error(e, "Solution procedure failed due to exception.");
            InvokeSolutionFailed(session.CurrentState);
            throw;
        }
    }

    private void DoTimeIntegration(ref SolverSession session)
    {
        session.Logger.Information("Starting time integration.");

        int i = 0;
        var recoverersArray = session.Routines.StateRecoverers.ToArray();

        while (session.CurrentState.Time < session.EndTime)
        {
            var currentSession = session;
            var stepVector = SolveStepUntilValid(
                currentSession,
                currentSession.CurrentState,
                recoverersArray
            );
            var timeStepWidth =
                session.Routines.StepWidthControllers.Min(c =>
                    c.GetStepWidth(currentSession, currentSession.CurrentState, stepVector)
                ) ?? throw new InvalidOperationException("No step width could be computed.");
            var newState = session.CurrentState.ApplyTimeStep(stepVector, timeStepWidth);

            InvokeStepSuccessfullyCalculated(
                currentSession,
                currentSession.CurrentState,
                newState,
                stepVector,
                timeStepWidth
            );

            session.CurrentState = newState;
            session.ReportCurrentState(stepVector);

            session.Logger.Information(
                "Time step {Index} successfully calculated. ({Time:e2}/{EndTime:e2} = {Percent:f2}%)",
                i,
                session.CurrentState.Time,
                session.EndTime,
                session.CurrentState.Time / session.EndTime * 100
            );
            i++;

            if (session.Routines.BreakConditions.Any(c => c.IsMet(newState)))
            {
                session.Logger.Information("A break condition was met.");
                break;
            }

            if (i % RemeshingEverySteps == 0)
            {
                session.CurrentState.Sanitize();
                var remeshedSystem = session.Routines.Remeshers.Aggregate<
                    IParticleSystemRemesher,
                    IParticleSystem<IParticle<IParticleNode>, IParticleNode>
                >(session.CurrentState, (state, remesher) => remesher.RemeshSystem(state));

                session = new SolverSession(
                    session,
                    session.CurrentState.Pores.Count == 0
                        ? new SystemState(Guid.NewGuid(), session.CurrentState.Time, remeshedSystem)
                        : new SystemStateWithPores<
                            IParticle<IParticleNode>,
                            IParticleNode,
                            IPoreState<IParticleNode>
                        >(
                            Guid.NewGuid(),
                            session.CurrentState.Time,
                            remeshedSystem.Particles,
                            session
                                .CurrentState.Pores.Zip(
                                    session.CurrentState.Pores.UpdatePores<
                                        IPore<IParticleNode>,
                                        IParticleNode
                                    >(remeshedSystem.Nodes)
                                )
                                .Select(t => new PoreState<IParticleNode>(
                                    t.First.Id,
                                    t.Second.Nodes,
                                    t.First.Porosity,
                                    t.First.ElasticStrain
                                ))
                        )
                );
                InvokeSessionInitialized(session);
                session.Logger.Information(
                    "Remeshed session created. Now {NodeCount} nodes present.",
                    remeshedSystem.Nodes.Count
                );
                session.ReportCurrentState();
            }
        }

        session.Logger.Information("Simulation finished after {StepCount} steps.", i);
    }

    private StepVector SolveStepUntilValid(
        SolverSession session,
        SolutionState baseState,
        IStateRecoverer[] recoverers
    )
    {
        StepVector? stepVector;

        try
        {
            stepVector = session.Routines.TimeStepper.Step(session, baseState);
        }
        catch (StepFailedException failedException)
        {
            session.Logger.Error(failedException, "Step calculation failed. Trying to recover.");
            InvokeStepFailed(session, baseState);

            try
            {
                stepVector = TryRecover(session, baseState, recoverers);
            }
            catch (RecoveryFailedException recoveryFailedException)
            {
                session.Logger.Error(
                    recoveryFailedException,
                    "Recovery failed. Trying next recoverer."
                );
                stepVector = TryRecover(session, baseState, recoverers[1..]);
            }
        }

        try
        {
            foreach (var validator in session.Routines.StepValidators)
            {
                validator.Validate(baseState, stepVector);
            }
        }
        catch (InvalidStepException stepRejectedException)
        {
            session.Logger.Error(
                stepRejectedException,
                "Calculated step was rejected. Trying to recover."
            );
            InvokeStepRejected(session, baseState, stepVector);

            try
            {
                stepVector = TryRecover(session, baseState, recoverers);
            }
            catch (RecoveryFailedException recoveryFailedException)
            {
                session.Logger.Error(
                    recoveryFailedException,
                    "Recovery failed. Trying next recoverer."
                );
                stepVector = TryRecover(session, baseState, recoverers[1..]);
            }
        }

        return stepVector;
    }

    private StepVector TryRecover(
        SolverSession session,
        SolutionState invalidState,
        IStateRecoverer[] recoverers
    )
    {
        try
        {
            var recoveredState = recoverers[0].RecoverState(session, invalidState);
            return SolveStepUntilValid(session, recoveredState, recoverers);
        }
        catch (IndexOutOfRangeException)
        {
            session.Logger.Error("No more recoverers available.");
            throw new CriticalIterationInterceptedException(
                nameof(SolveStepUntilValid),
                InterceptReason.ExceptionOccured,
                furtherInformation: "Recovery of solution finally failed."
            );
        }
    }

    public event EventHandler<StepSuccessfullyCalculatedEventArgs>? StepSuccessfullyCalculated;

    private void InvokeStepSuccessfullyCalculated(
        SolverSession solverSession,
        SolutionState oldState,
        SolutionState newState,
        StepVector stepVector,
        double timeStepWidth
    )
    {
        StepSuccessfullyCalculated?.Invoke(
            this,
            new StepSuccessfullyCalculatedEventArgs(
                solverSession,
                oldState,
                newState,
                stepVector,
                timeStepWidth
            )
        );
    }

    public event EventHandler<StepRejectedEventArgs>? StepRejected;

    private void InvokeStepRejected(
        SolverSession solverSession,
        SolutionState baseState,
        StepVector stepVector
    )
    {
        StepRejected?.Invoke(this, new StepRejectedEventArgs(solverSession, baseState, stepVector));
    }

    public event EventHandler<SessionInitializedEventArgs>? SessionInitialized;

    private void InvokeSessionInitialized(SolverSession solverSession)
    {
        SessionInitialized?.Invoke(this, new SessionInitializedEventArgs(solverSession));
    }

    public class StepSuccessfullyCalculatedEventArgs(
        ISolverSession solverSession,
        SolutionState oldState,
        SolutionState newState,
        StepVector stepVector,
        double timeStepWidth
    ) : EventArgs
    {
        public ISolverSession SolverSession { get; } = solverSession;
        public SolutionState OldState { get; } = oldState;
        public SolutionState NewState { get; } = newState;
        public StepVector StepVector { get; } = stepVector;
        public double TimeStepWidth { get; } = timeStepWidth;
    }

    public class StepRejectedEventArgs(
        ISolverSession solverSession,
        SolutionState baseState,
        StepVector stepVector
    ) : EventArgs
    {
        public ISolverSession SolverSession { get; } = solverSession;
        public SolutionState BaseState { get; } = baseState;
        public StepVector StepVector { get; } = stepVector;
    }

    public class SessionInitializedEventArgs(ISolverSession solverSession) : EventArgs
    {
        public ISolverSession SolverSession { get; } = solverSession;
    }

    public event EventHandler<StepFailedEventArgs>? StepFailed;

    private void InvokeStepFailed(SolverSession solverSession, SolutionState baseState)
    {
        StepFailed?.Invoke(this, new StepFailedEventArgs(solverSession, baseState));
    }

    public class StepFailedEventArgs(ISolverSession solverSession, SolutionState baseState)
        : EventArgs
    {
        public ISolverSession SolverSession { get; } = solverSession;
        public SolutionState BaseState { get; } = baseState;
    }

    public event EventHandler<SolutionFailedEventArgs>? SolutionFailed;

    private void InvokeSolutionFailed(SolutionState lastState)
    {
        SolutionFailed?.Invoke(this, new SolutionFailedEventArgs(lastState));
    }

    public class SolutionFailedEventArgs(SolutionState lastState) : EventArgs
    {
        public SolutionState LastState { get; } = lastState;
    }
}
