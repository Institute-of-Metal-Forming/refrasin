using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using Serilog;
using static RefraSin.ParticleModel.Nodes.NodeType;
using Log = Serilog.Log;

namespace RefraSin.ParticleModel.Remeshing;

public class LastSurfaceNodeRemesher(double asymmetryLimit = 0.05) : IParticleRemesher
{
    public double AsymmetryLimit { get; } = asymmetryLimit;

    /// <inheritdoc />
    public IParticle<IParticleNode> Remesh(IParticle<IParticleNode> particle)
    {
        var logger = Log.ForContext<FreeSurfaceRemesher>();
        logger.Debug("Remeshing last surface nodes of {Particle}.", particle);

        IEnumerable<IParticleNode> NodeFactory(IParticle<IParticleNode> newParticle) =>
            FilterNodes(newParticle, particle.Nodes, logger);

        var newParticle = new Particle<IParticleNode>(
            particle.Id,
            particle.Coordinates,
            particle.RotationAngle,
            particle.MaterialId,
            NodeFactory
        );

        return newParticle;
    }

    private IEnumerable<IParticleNode> FilterNodes(
        IParticle<IParticleNode> particle,
        IEnumerable<IParticleNode> nodes,
        ILogger logger
    )
    {
        foreach (var node in nodes)
        {
            if (node is { Type: Surface, Upper.Type: Neck, Lower.Type: Neck })
            {
                var asymmetryRatio = node.SurfaceDistance.ToUpper / node.SurfaceDistance.ToLower;

                if (asymmetryRatio > 1)
                    asymmetryRatio = 1 / asymmetryRatio;

                if (asymmetryRatio < AsymmetryLimit)
                {
                    var newNode = new ParticleNode(
                        Guid.NewGuid(),
                        particle,
                        node.Upper.Coordinates.Centroid(node.Lower.Coordinates),
                        Surface
                    );
                    logger.Information(
                        "Remeshed last surface node {Node} to {NewNode}",
                        node,
                        newNode
                    );
                    yield return newNode;
                    continue;
                }
            }

            yield return new ParticleNode(node, particle); // keep node
        }
    }
}
