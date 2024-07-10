using RefraSin.Coordinates;
using RefraSin.Coordinates.Cartesian;
using RefraSin.ParticleModel.Collections;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.TEPSolver.ParticleModel;

internal record ParticleState(
    Guid Id,
    ICartesianPoint Coordinates,
    Angle RotationAngle,
    Guid MaterialId,
    IReadOnlyParticleSurface<I> Nodes
) : IParticle;