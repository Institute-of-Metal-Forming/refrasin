using System.Runtime.InteropServices;
using RefraSin.ParticleModel;

namespace Refrasin.HDF5Storage;

internal struct ParticleCompound
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string Id;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public double[] AbsoluteCenterCoordinates;

    public double RotationAngle;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string MaterialId;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public double[] CenterCoordinates;

    public ParticleCompound(IParticle particle)
    {
        Id = particle.Id.ToString();
        AbsoluteCenterCoordinates = particle.AbsoluteCenterCoordinates.ToArray();
        RotationAngle = particle.RotationAngle;
        MaterialId = particle.MaterialId.ToString();
        CenterCoordinates = particle.CenterCoordinates.ToArray();
    }
}