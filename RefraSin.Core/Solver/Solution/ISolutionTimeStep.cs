using System.Collections.Generic;
using RefraSin.Core.ParticleModel.Interfaces;

namespace RefraSin.Core.Solver.Solution;

public interface ISolutionTimeStep
{
    double Time { get; }

    IReadOnlyList<IParticle> Particles { get; }
}