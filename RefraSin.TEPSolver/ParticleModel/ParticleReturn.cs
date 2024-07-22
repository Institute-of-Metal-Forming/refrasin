using RefraSin.Coordinates;
using RefraSin.Coordinates.Absolute;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
using RefraSin.TEPSolver.Normalization;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.ParticleModel;

internal record ParticleReturn : IParticle<NodeReturn>
{
    public ParticleReturn(Particle template, StepVector? stepVector, INorm norm)
    {
        Id = template.Id;
        Coordinates = new AbsolutePoint(
            template.Coordinates.X * norm.Length,
            template.Coordinates.Y * norm.Length
        );
        RotationAngle = template.RotationAngle;
        MaterialId = template.MaterialId;

        Nodes = template
            .Nodes.Select(n =>
                n switch
                {
                    ContactNodeBase contactNode
                        => new ContactNodeReturn(contactNode, stepVector, this, norm),
                    _ => new NodeReturn(n, stepVector, this, norm)
                }
            )
            .ToReadOnlyParticleSurface();
    }

    public Guid Id { get; }
    public ICartesianPoint Coordinates { get; }
    public Angle RotationAngle { get; }
    public Guid MaterialId { get; }
    public IReadOnlyParticleSurface<NodeReturn> Nodes { get; }
}
