using Microsoft.Extensions.Logging;
using RefraSin.Numerics.Exceptions;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.Storage;
using RefraSin.TEPSolver.Exceptions;
using RefraSin.TEPSolver.Recovery;
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
        ISolverRoutines routines
    )
    {
        SolutionStorage = solutionStorage;
        LoggerFactory = loggerFactory;
        Routines = routines;
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
    /// Run the solution procedure starting with the given state till the specified time.
    /// </summary>
    public ISystemState Solve(ISinteringStep conditions, ISystemState inputState)
    {
        var session = new SolverSession(this, inputState, conditions);
        InvokeSessionInitialized(session);
        session.ReportCurrentState();
        DoTimeIntegration(session);

        return new SystemState(
            session.CurrentState.Id,
            session.CurrentState.Time,
            session.CurrentState.Particles
        );
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

            session.Logger.LogInformation("Time step {Index} successfully calculated. ({Time}/{EndTime})", i, session.CurrentState.Time,
                session.EndTime);
            i++;
        }

        session.Logger.LogInformation(
            "End time successfully reached after {StepCount} steps.",
            i
        );
    }

    private StepVector SolveStepUntilValid(SolverSession session, SolutionState solutionState, IStateRecoverer[] recoverers)
    {
        var stepVector = session.Routines.TimeStepper.Step(session, solutionState,
            session.LastStep ?? session.Routines.StepEstimator.EstimateStep(session, solutionState));

        try
        {
            foreach (var validator in session.Routines.StepValidators)
            {
                validator.Validate(solutionState, stepVector);
            }
        }
        catch (StepRejectedException stepRejectedException)
        {
            session.Logger.LogError(stepRejectedException, "Calculated step was rejected. Trying to recover.");
            InvokeStepRejected(session, solutionState, stepVector);

            try
            {
                stepVector = SolveStepUntilValid(session, recoverers[0].RecoverState(session, solutionState), recoverers);
            }
            catch (RecoveryFailedException recoveryFailedException)
            {
                session.Logger.LogError(recoveryFailedException, "Recovery failed. Trying next recoverer.");
                var remainingRecoverers = recoverers[1..];

                stepVector = SolveStepUntilValid(session, remainingRecoverers[0].RecoverState(session, solutionState), remainingRecoverers);
            }
            catch (ArgumentOutOfRangeException)
            {
                session.Logger.LogError("No more recoverers available.");
                throw new CriticalIterationInterceptedException(nameof(SolveStepUntilValid), InterceptReason.ExceptionOccured,
                    furtherInformation: "Recovery of solution finally failed.");
            }
        }

        return stepVector;
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