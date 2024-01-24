namespace RefraSin.TEPSolver.StepVectors;

public class ContactView
{
    private readonly StepVector _vector;
    private readonly Guid _fromParticleId;
    private readonly Guid _toParticleId;

    public ContactView(StepVector vector, Guid fromParticleId, Guid toParticleId)
    {
        _vector = vector;
        _fromParticleId = fromParticleId;
        _toParticleId = toParticleId;
    }
    public double RadialDisplacement => _vector[_vector.StepVectorMap.GetIndex(_fromParticleId, _toParticleId, ContactUnknown.RadialDisplacement)];
    public double AngleDisplacement => _vector[_vector.StepVectorMap.GetIndex(_fromParticleId, _toParticleId, ContactUnknown.AngleDisplacement)];
    public double RotationDisplacement => _vector[_vector.StepVectorMap.GetIndex(_fromParticleId, _toParticleId, ContactUnknown.RotationDisplacement)];
}