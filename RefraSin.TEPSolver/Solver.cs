using System;
using System.Threading;
using RefraSin.Core.SinteringProcesses;
using RefraSin.Core.Solver.Solution;
using RefraSin.Iteration;
using static RefraSin.Iteration.InterceptReason;

namespace RefraSin.Core.Solver
{
    /// <summary>
    ///     Lösungsalgorithmus für den Sinterprozess.
    /// </summary>
    public static partial class Solver
    {
        /// <summary>
        ///     Hauptfunktion für das Lösen des Sintermodells.
        /// </summary>
        /// <returns>true wenn erfolgreich, sonst false</returns>
        public static ISinteringSolverSolution Solve(ISinteringProcess process, SolverOptions options, CancellationToken cancellationToken)
        {
            var session = Session.CreateSessionFromProcess(process, options);
            session.Start();

            try
            {
                RunTimeIntegrationLoop(session, cancellationToken);
            }
            catch (Exception e)
            {
                session.EndFailed(e);
                return new SolutionResult(false, session.TimeSeries, process);
            }

            session.End();

            return new SolutionResult(true, session.TimeSeries, process);
        }

        private static void RunTimeIntegrationLoop(Session session, CancellationToken cancellationToken)
        {
            while (EndTimeIsNotReached(session))
            {
                CalculateTimeStepWithDecreasingTimeStepWidthsUntilTimeStepIsValid(session, cancellationToken);

                session.ApplyTimeStep();
                session.IncreaseTimeStepWidthConditionally();
            }
        }

        private static bool EndTimeIsNotReached(Session session) => !(session.CurrentTime > session.EndTime);

        private static void CalculateTimeStepWithDecreasingTimeStepWidthsUntilTimeStepIsValid(Session session, CancellationToken cancellationToken)
        {
            for (var i = 0;; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                session.ThrowIfTimedOut();

                if (i > session.Options.MaxIterationCount)
                    throw new CriticalIterationInterceptedException(nameof(CalculateTimeStepWithDecreasingTimeStepWidthsUntilTimeStepIsValid),
                        MaxIterationCountExceeded, i);

                if (TryCalculateTimeStep(session))
                    return;

                session.DecreaseTimeStepWidthConditionally();
            }
        }

        private static bool TryCalculateTimeStep(Session session)
        {
            try
            {
                CalculateTimeStep(session);
            }
            catch (InvalidTimeStepException e)
            {
                session.LogInvalidTimeStep(e);
                return false;
            }
            catch (UncriticalIterationInterceptedException e)
            {
                session.LogUncriticalIterationIntercepted(e);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Berechne Zustand nach neuem Zeitschritt.
        /// </summary>
        private static void CalculateTimeStep(Session session)
        {
            InitParticleFuturePlacement(session.Particles.Root);

            RemeshParticleSurfaces(session);
            CalculateTimeStepsOnFreeSurfaces(session);
            CalculateTimeStepsOnGrainBoundaries(session);
            ValidateTimeSteps(session);
        }
    }
}