using RefraSin.ParticleModel.Collections;
using RefraSin.TEPSolver.ParticleModel;
using Serilog;
using Log = Serilog.Log;

namespace RefraSin.TEPSolver.BreakConditions;

public class PoreClosedCondition(double neckDistanceThreshold = 1e-2) : IBreakCondition
{
    private ILogger _logger = Log.Logger.ForContext<PoreClosedCondition>();

    public bool IsMet(SolutionState solutionState)
    {
        var result = solutionState
            .Nodes.OfType<NeckNode>()
            .Where(n => n.Upper.Type == NodeType.Surface)
            .Any(neck =>
            {
                var poreSurfaceDistance = neck.From()
                    .While(n => n.Type == NodeType.Neck && n.Id != neck.Id)
                    .Sum(n => n.SurfaceDistance.ToUpper);
                _logger.Debug(
                    "Got pore surface distance {PoreSurfaceDistance} which is {Comparison} than {Threshold}.",
                    poreSurfaceDistance,
                    poreSurfaceDistance < neckDistanceThreshold ? "smaller" : "larger",
                    neckDistanceThreshold
                );
                return poreSurfaceDistance < neckDistanceThreshold;
            });

        if (result)
            _logger.Information("Pore closed condition was met.");

        return result;
    }

    public void RegisterWithSolver(SinteringSolver solver) { }
}
