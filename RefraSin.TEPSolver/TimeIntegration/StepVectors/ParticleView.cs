namespace RefraSin.TEPSolver.TimeIntegration.StepVectors;

public class ParticleView
{
    private readonly StepVector _vector;
    private readonly Guid _particleId;

    public ParticleView(StepVector vector, Guid particleId)
    {
        _vector = vector;
        _particleId = particleId;
    }
    public double RadialDisplacement => _vector[_vector.StepVectorMap.GetIndex(_particleId, ParticleUnknown.RadialDisplacement)];
    public double AngleDisplacement => _vector[_vector.StepVectorMap.GetIndex(_particleId, ParticleUnknown.AngleDisplacement)];
    public double RotationDisplacement => _vector[_vector.StepVectorMap.GetIndex(_particleId, ParticleUnknown.RotationDisplacement)];
}