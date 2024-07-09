using Microsoft.Extensions.Logging;
using RefraSin.Numerics.Exceptions;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Remeshing;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver.Recovery;
using RefraSin.TEPSolver.StepValidators;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver;

/// <summary>
/// Solver for performing time integration of sintering processes based on the thermodynamic extremal principle (TEP).
/// </summary>
public class SinteringSolver : IProcessStepSolver<ISinteringStep>
{
    public SinteringSolver(
        ISolutionStorage solutionStorage,
        ILoggerFactory loggerFactory,
        ISolverRoutines routines, int remeshingEverySteps = 10)
    {
        SolutionStorage = solutionStorage;
        LoggerFactory = loggerFactory;
        Routines = routines;
        RemeshingEverySteps = remeshingEverySteps;
        routines.RegisterWithSolver(this);
    }

    /// <summary>
    /// Storage for solution data.
    /// </summary>
    public ISolutionStorage SolutionStorage { get; }

    /// <summary>
    /// Factory for loggers used in the Session.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; }

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
        TryTimeIntegration(session);

        return new SystemState(
            session.CurrentState.Id,
            session.CurrentState.Time,
            session.CurrentState.Particles
        );
    }

    private void TryTimeIntegration(SolverSession session)
    {
        try
        {
            DoTimeIntegration(session);
        }
        catch (Exception e)
        {
            session.Logger.LogCritical(e, "Solution procedure failed due to exception.");
        }
    }

    private void DoTimeIntegration(SolverSession session)
    {
        session.Logger.LogInformation(
            "Starting time integration."
        );

        int i = 0;
        var recoverersArray = session.Routines.StateRecoverers.ToArray();

        while (session.CurrentState.Time < session.EndTime)
        {
            var stepVector = SolveStepUntilValid(session, session.CurrentState, recoverersArray);
            var timeStepWidth = session.Routines.StepWidthController.GetStepWidth(session, session.CurrentState, stepVector);
            var newState = session.CurrentState.ApplyTimeStep(stepVector, timeStepWidth);

            InvokeStepSuccessfullyCalculated(session, session.CurrentState, newState, stepVector);

            session.CurrentState = newState;
            session.ReportCurrentState();

            session.Logger.LogInformation("Time step {Index} successfully calculated. ({Time:e2}/{EndTime:e2} = {Percent:f2}%)", i,
                session.CurrentState.Time,
                session.EndTime, session.CurrentState.Time / session.EndTime * 100);
            i++;

            if (i % RemeshingEverySteps == 0)
            {
                var remeshedState = new SystemState(Guid.NewGuid(), session.CurrentState.Time,
                    session.CurrentState.Particles.Select(p =>
                        session.Routines.Remeshers.Aggregate((IParticle)p, (rp, remesher) => remesher.Remesh(rp))));

                session = new SolverSession(session, remeshedState,
                    session.CurrentState.ParticleContacts.Select(c => (c.Id, c.From.Id, c.To.Id)).ToArray(),
                    session.CurrentState.NodeContacts.Select(c => (c.Key, c.Value)).ToArray()
                );
                InvokeSessionInitialized(session);
                session.Logger.LogInformation("Remeshed session created. Now {NodeCount} nodes present.", remeshedState.Nodes.Count);
                session.ReportCurrentState();
            }
        }

        session.Logger.LogInformation(
            "End time successfully reached after {StepCount} steps.",
            i
        );
    }

    private StepVector SolveStepUntilValid(SolverSession session, SolutionState baseState, IStateRecoverer[] recoverers)
    {
        var stepVector = session.Routines.TimeStepper.Step(session, baseState,
            session.LastStep ?? session.Routines.StepEstimator.EstimateStep(session, baseState));

        try
        {
            foreach (var validator in session.Routines.StepValidators)
            {
                validator.Validate(baseState, stepVector);
            }
        }
        catch (InvalidStepException stepRejectedException)
        {
            session.Logger.LogError(stepRejectedException, "Calculated step was rejected. Trying to recover.");
            InvokeStepRejected(session, baseState, stepVector);

            try
            {
                stepVector = TryRecover(session, baseState, recoverers);
            }
            catch (RecoveryFailedException recoveryFailedException)
            {
                session.Logger.LogError(recoveryFailedException, "Recovery failed. Trying next recoverer.");
                stepVector = TryRecover(session, baseState, recoverers[1..]);
            }
        }

        return stepVector;
    }

    private StepVector TryRecover(SolverSession session, SolutionState invalidState, IStateRecoverer[] recoverers)
    {
        try
        {
            var recoveredState = recoverers[0].RecoverState(session, invalidState);
            return SolveStepUntilValid(session, recoveredState, recoverers);
        }
        catch (IndexOutOfRangeException)
        {
            session.Logger.LogError("No more recoverers available.");
            throw new CriticalIterationInterceptedException(nameof(SolveStepUntilValid), InterceptReason.ExceptionOccured,
                furtherInformation: "Recovery of solution finally failed.");
        }
    }

    public event EventHandler<StepSuccessfullyCalculatedEventArgs>? StepSuccessfullyCalculated;

    private void InvokeStepSuccessfullyCalculated(SolverSession solverSession, SolutionState oldState, SolutionState newState, StepVector stepVector)
    {
        StepSuccessfullyCalculated?.Invoke(this, new StepSuccessfullyCalculatedEventArgs(
            solverSession,
            oldState,
            newState,
            stepVector
        ));
    }

    public event EventHandler<StepRejectedEventArgs>? StepRejected;

    private void InvokeStepRejected(SolverSession solverSession, SolutionState baseState, StepVector stepVector)
    {
        StepRejected?.Invoke(this, new StepRejectedEventArgs(
            solverSession,
            baseState,
            stepVector
        ));
    }

    public event EventHandler<SessionInitializedEventArgs>? SessionInitialized;

    private void InvokeSessionInitialized(SolverSession solverSession)
    {
        SessionInitialized?.Invoke(this, new SessionInitializedEventArgs(
            solverSession
        ));
    }

    public class StepSuccessfullyCalculatedEventArgs(
        ISolverSession solverSession,
        SolutionState oldState,
        SolutionState newState,
        StepVector stepVector) : EventArgs
    {
        public ISolverSession SolverSession { get; } = solverSession;
        public SolutionState OldState { get; } = oldState;
        public SolutionState NewState { get; } = newState;
        public StepVector StepVector { get; } = stepVector;
    }

    public class StepRejectedEventArgs(
        ISolverSession solverSession,
        SolutionState baseState,
        StepVector stepVector) : EventArgs
    {
        public ISolverSession SolverSession { get; } = solverSession;
        public SolutionState BaseState { get; } = baseState;
        public StepVector StepVector { get; } = stepVector;
    }

    public class SessionInitializedEventArgs(ISolverSession solverSession) : EventArgs
    {
        public ISolverSession SolverSession { get; } = solverSession;
    }
}