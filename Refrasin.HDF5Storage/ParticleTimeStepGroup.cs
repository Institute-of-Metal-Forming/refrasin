using System.Runtime.InteropServices;
using RefraSin.ParticleModel;

namespace Refrasin.HDF5Storage;

internal class ParticleTimeStepGroup
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string ParticleId;

    public readonly double RadialDisplacement;

    public readonly double AngleDisplacement;

    public readonly double RotationDisplacement;

    public readonly double VolumeChange;

    public readonly NodeTimeStepCompound[] NodeTimeSteps;

    public ParticleTimeStepGroup(IParticleTimeStep particleTimeStep)
    {
        ParticleId = particleTimeStep.ParticleId.ToString();
        RadialDisplacement = particleTimeStep.RadialDisplacement;
        AngleDisplacement = particleTimeStep.AngleDisplacement;
        RotationDisplacement = particleTimeStep.RotationDisplacement;
        VolumeChange = particleTimeStep.VolumeChange;
        NodeTimeSteps = particleTimeStep.NodeTimeSteps.Select(kvp => new NodeTimeStepCompound(kvp.Value)).ToArray();
    }
}