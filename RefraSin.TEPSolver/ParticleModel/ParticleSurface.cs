using RefraSin.Coordinates;
using RefraSin.Enumerables;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.ParticleModel;

/// <summary>
/// Stellt die Oberfläche eines Pulverpartikels als Aufzählung von Oberflächenknoten dar.
/// </summary>
internal class ParticleSurface : Ring<Node>
{
    /// <inheritdoc />
    public ParticleSurface(Particle particle, IEnumerable<INode> nodeSpecs, ISolverSession solverSession)
    {
        Particle = particle;

        foreach (var nodeSpec in nodeSpecs)
        {
            Add(nodeSpec switch
            {
                INeckNode nn           => new NeckNode(nn, particle, solverSession),
                IGrainBoundaryNode gbn => new GrainBoundaryNode(gbn, particle, solverSession),
                _                      => new SurfaceNode(nodeSpec, particle, solverSession)
            });
        }
    }

    public Particle Particle { get; }

    /// <summary>
    /// Bestimmt die beiden einem Winkel nächstgelegenen Oberflächenknoten.
    /// </summary>
    /// <param name="angle">Winkel</param>
    /// <returns></returns>
    public (Node Upper, Node Lower) GetNearestNodesToAngle(Angle angle)
    {
        var nodes = this.OrderBy(k => Angle.ReduceRadians(k.Coordinates.Phi.Radians, Angle.ReductionDomain.AllPositive)).ToArray();
        var upper = nodes.FirstOrDefault(k => k.Coordinates.Phi.Radians > angle.Radians) ?? nodes.First();
        var lower = upper.Lower;
        return (upper, lower);
    }

    /// <summary>
    /// Berechnet den zwischen den angrenzenden Knoten interpolierten Radius an einer bestimmten Winkelkoordinate.
    /// </summary>
    /// <param name="angle">Winkel</param>
    /// <returns></returns>
    public double InterpolatedRadius(Angle angle)
    {
        var (upper, lower) = GetNearestNodesToAngle(angle);
        return lower.Coordinates.R + (upper.Coordinates.R - lower.Coordinates.R) /
            (upper.Coordinates.Phi - lower.Coordinates.Phi).Reduce().Radians * (angle - lower.Coordinates.Phi).Reduce().Radians;
    }
}