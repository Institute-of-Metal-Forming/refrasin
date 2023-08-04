using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using IMF.Enumerables;
using IMF.Iteration;
using Microsoft.Extensions.Logging;
using RefraSin.Coordinates.Polar;
using RefraSin.Core.ParticleModel;
using RefraSin.Core.SinteringProcesses;
using RefraSin.Core.Solver.Solution;
using static MathNet.Numerics.Constants;

namespace RefraSin.Core.Solver
{
    public partial class SinteringSolver
    {
        private class Session : ISolverSession
        {
            private readonly ILogger _logger = Configuration.CreateLogger(typeof(SinteringSolver));
            private readonly List<TimeSeriesItem> _solutionStates = new();
            private readonly Stopwatch _stopwatch = new();
            private double? _initialTotalParticlesVolume;
            private double _timeStepWidth;

            private Session(ISinteringProcess process, SolverOptions solverOptions)
            {
                StartTime = process.StartTime;
                EndTime = process.EndTime;
                CurrentTime = StartTime;
                _timeStepWidth = solverOptions.InitialTimeStepWidth; // use of field is essential for LastStepsTimeStepWidth != 0
                Options = solverOptions;

                Particles = process.TreeSource.GetParticleTree(process, SurfaceNodeCountFunction);
                process.Compactor.Compact(this);
            }

            public int TimeStepsSinceLastTimeStepWidthChange { get; private set; }

            public double CurrentTime { get; private set; }
            public double StartTime { get; }
            public double EndTime { get; }

            public double TimeStepWidth
            {
                get => _timeStepWidth;
                private set
                {
                    TimeStepWidthOfLastStep = _timeStepWidth;
                    _timeStepWidth = value;
                }
            }

            public double? TimeStepWidthOfLastStep { get; private set; }

            public Tree<Particle> Particles { get; }

            public SolverOptions Options { get; }

            public IReadOnlyList<TimeSeriesItem> TimeSeries => _solutionStates;

            public void Start()
            {
                _solutionStates.Clear();
                SaveCurrentStateToSolutionStates();
                _initialTotalParticlesVolume = CalculateTotalParticlesVolume();

                _stopwatch.Reset();
                _stopwatch.Start();
                LogSolvingStarted();
            }

            public void End()
            {
                _stopwatch.Stop();
                LogSolvingEnded();
                CalculateAndLogNumericalVolumeLoss();
            }

            public void EndFailed(Exception e)
            {
                _stopwatch.Stop();
                LogSolvingFailed(e);
                LogSolvingEnded();
                CalculateAndLogNumericalVolumeLoss();
            }

            public void ThrowIfTimedOut()
            {
                if (Options.TimeOut > 0 && _stopwatch.Elapsed.TotalSeconds > Options.TimeOut)
                    throw new TimeoutException();
            }

            private void CalculateAndLogNumericalVolumeLoss()
            {
                if (_initialTotalParticlesVolume is null)
                    throw new InvalidOperationException("Numerical volume loss cannot be calculated without initial value.");

                var currentVolume = CalculateTotalParticlesVolume();
                var absoluteVolumeLoss = currentVolume - _initialTotalParticlesVolume.Value;
                var relativeVolumeLoss = absoluteVolumeLoss / _initialTotalParticlesVolume.Value;
                LogNumericalVolumeLoss(absoluteVolumeLoss, relativeVolumeLoss);
            }

            private double CalculateTotalParticlesVolume() => Particles.Sum(p => p.Surface.Sum(k => k.NeighborElementsVolume));

            public static Session CreateSessionFromProcess(ISinteringProcess process, SolverOptions solverOptions) =>
                new(process, solverOptions);

            public void ApplyTimeStep()
            {
                CurrentTime += TimeStepWidth;
                TimeStepsSinceLastTimeStepWidthChange += 1;

                foreach (var particle in Particles)
                    particle.ApplyTimeStep();

                SaveCurrentStateToSolutionStates();
                LogTimeStepSuccessful();
            }

            public void DecreaseTimeStepWidthConditionally()
            {
                var newTimeStepWidth = TimeStepWidth / Options.TimeStepAdaptationFactor;

                if (newTimeStepWidth < Options.MinTimeStepWidth)
                    throw new CriticalIterationInterceptedException(
                        nameof(CalculateTimeStepWithDecreasingTimeStepWidthsUntilTimeStepIsValid),
                        InterceptReason.InvalidStateOccured,
                        furtherInformation:
                        $"Tried to decrease current time step width of {TimeStepWidth}, but it fall below defined minimal time step width of {Options.MinTimeStepWidth}."
                    );

                TimeStepWidth = newTimeStepWidth;
                TimeStepsSinceLastTimeStepWidthChange = -1;
                LogTimeStepWidthDecreased();
            }

            public void IncreaseTimeStepWidthConditionally()
            {
                var newTimeStepWidth = TimeStepWidth * Options.TimeStepAdaptationFactor;
                if (newTimeStepWidth < Options.MaxTimeStepWidth && TimeStepsSinceLastTimeStepWidthChange >= Options.TimeStepIncreaseDelay)
                {
                    TimeStepWidth = newTimeStepWidth;
                    TimeStepsSinceLastTimeStepWidthChange = -1;
                    LogTimeStepWidthIncreased();
                }
            }

            private void SaveCurrentStateToSolutionStates() => _solutionStates.Add(new TimeSeriesItem(CurrentTime, Particles));

            private int SurfaceNodeCountFunction(double particleRadius) => (int)(Pi2 * particleRadius / Options.DiscretizationWidth);

            #region Log Methods

            public void LogChildsFutureCoordinates(Particle child, int iterationCount) =>
                _logger.LogTrace(
                    "Future slave center coordinates found with {SlaveCenterCoordinates} after {Iterations} iterations.",
                    child.FutureCenterCoordinates?.ToString("(,)::rad", CultureInfo.InvariantCulture), iterationCount);

            public void LogChildsMovementDetermined(Particle child, PolarVector movementVector) =>
                _logger.LogTrace("Movement of child particle {Child} was determined as {Vector}", child.ToString(),
                    movementVector.ToString("(,)::rad", CultureInfo.InvariantCulture));

            public void LogGrainBoundaryStressesDetermined(int iterationCount) =>
                _logger.LogTrace("Grain boundary stresses determined after {Iterations} iterations.", iterationCount);

            public void LogInvalidTimeStep(InvalidTimeStepException e) =>
                _logger.LogDebug("Invalid time step of {SourceNode} due to: {Reason}.", e.SourceNode, e.Reason);

            private void LogNumericalVolumeLoss(double absoluteVolumeLoss, double relativeVolumeLoss) =>
                _logger.LogInformation("Numerical volume loss was {VolumeLoss} which makes up {VolumeLossRatio} of total volume.", absoluteVolumeLoss,
                    relativeVolumeLoss);

            private void LogSolvingEnded() =>
                _logger.LogInformation(
                    "Ended solution process. Calculated {TimeStepCount} time steps. Took {TotalTime}.",
                    TimeSeries.Count - 1, _stopwatch.Elapsed);

            private void LogSolvingFailed(Exception e) => _logger.LogCritical(e, "Solution process failed due to: ");

            private void LogSolvingStarted() => _logger.LogInformation("Started solution process...");

            private void LogTimeStepSuccessful() => _logger.LogInformation("Time step {Time} was successful.", CurrentTime);

            private void LogTimeStepWidthDecreased() => _logger.LogInformation("Time step width decreased to {TimeStepWidth}.", TimeStepWidth);

            private void LogTimeStepWidthIncreased() => _logger.LogInformation("Time step width increased to {TimeStepWidth}.", TimeStepWidth);

            public void LogUncriticalIterationIntercepted(UncriticalIterationInterceptedException e) =>
                _logger.LogWarning("The iteration loop {LoopLabel} was intercepted at iteration {IterationCount} due to: {Reason}",
                    e.LoopLabel, e.IterationCount, e.Reason);

            #endregion
        }
    }
}