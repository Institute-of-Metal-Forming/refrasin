namespace RefraSin.TEPSolver.Step;

internal class ParticleView
{
    private readonly StepVector _vector;
    private readonly Guid _particleId;

    public ParticleView(StepVector vector, Guid particleId)
    {
        _vector = vector;
        _particleId = particleId;
    }
}