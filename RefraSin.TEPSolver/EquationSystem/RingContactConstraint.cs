using RefraSin.Graphs;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;

namespace RefraSin.TEPSolver.EquationSystem;

public class RingContactConstraint(IGraphCycle<Particle, ParticleContact> ring, StepVector step)
    : RingEquationBase(ring, step)
{
    /// <inheritdoc />
    public override double Value() => throw new NotImplementedException();

    /// <inheritdoc />
    public override IEnumerable<(int, double)> Derivative() => throw new NotImplementedException();
}
