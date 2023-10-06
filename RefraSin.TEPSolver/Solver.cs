using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.RootFinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MoreLinq;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.Storage;

namespace RefraSin.TEPSolver;

/// <summary>
/// Solver for performing time integration of sintering processes based on the thermodynamic extremal principle (TEP).
/// </summary>
public partial class Solver
{
    /// <summary>
    /// Numeric options to control solver behavior.
    /// </summary>
    public ISolverOptions Options { get; set; } = new SolverOptions();

    /// <summary>
    /// Storage for solution data.
    /// </summary>
    public ISolutionStorage SolutionStorage { get; set; } = new InMemorySolutionStorage();

    /// <summary>
    /// Factory for loggers used in the session.
    /// </summary>
    public ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();

    /// <summary>
    /// Creates a new solver session for the given process.
    /// </summary>
    internal SolverSession CreateSession(ISinteringProcess process) => new SolverSession(this, process);

    /// <summary>
    /// Run the solution procedure starting with the given state till the specified time.
    /// </summary>
    public void Solve(ISinteringProcess process)
    {
        var session = CreateSession(process);
        Solve(session);
    }

    internal static void Solve(SolverSession session)
    {
        session.SolutionStorage.StoreState(new SolutionState(session.CurrentTime, session.Particles.Values));

        while (session.CurrentTime < session.EndTime)
        {
            var timeSteps = SolveStep(session);

            var nextTime = session.CurrentTime + session.TimeStepWidth;

            session.SolutionStorage.StoreStep(new SolutionStep(session.CurrentTime, nextTime, timeSteps));

            session.CurrentTime = nextTime;

            foreach (var timeStep in timeSteps)
                session.Particles[timeStep.ParticleId].ApplyTimeStep(timeStep);

            session.SolutionStorage.StoreState(new SolutionState(session.CurrentTime, session.Particles.Values));
        }
    }

    internal static IReadOnlyList<IParticleTimeStep> SolveStep(ISolverSession session)
    {
        session.LagrangianGradient.FindRoot();

        return session.Particles.Values.Select(p => new ParticleTimeStep(
            p.Id,
            0,
            0,
            0,
            p.Surface.Select(n => new NodeTimeStep(
                n.Id,
                session.LagrangianGradient.GetSolutionValue(n.Id, LagrangianGradient.NodeUnknown.NormalDisplacement),
                0,
                new ToUpperToLower(
                    session.LagrangianGradient.GetSolutionValue(n.Id, LagrangianGradient.NodeUnknown.FluxToUpper),
                    -session.LagrangianGradient.GetSolutionValue(n.Lower.Id, LagrangianGradient.NodeUnknown.FluxToUpper)
                ),
                0
            ))
        )).ToArray();
    }
}