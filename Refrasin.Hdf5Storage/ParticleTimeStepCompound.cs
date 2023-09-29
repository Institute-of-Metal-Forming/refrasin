using System.Runtime.InteropServices;
using RefraSin.ParticleModel;

namespace Refrasin.HDF5Storage;

internal struct ParticleTimeStepCompound
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string ParticleId;

    public readonly double RadialDisplacement;

    public readonly double AngleDisplacement;

    public readonly double RotationDisplacement;

    public ParticleTimeStepCompound(IParticleTimeStep particleTimeStep)
    {
        ParticleId = particleTimeStep.ParticleId.ToString();
        RadialDisplacement = particleTimeStep.RadialDisplacement;
        AngleDisplacement = particleTimeStep.AngleDisplacement;
        RotationDisplacement = particleTimeStep.RotationDisplacement;
    }
}