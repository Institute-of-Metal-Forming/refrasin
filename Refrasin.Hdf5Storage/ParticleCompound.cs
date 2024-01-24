using System.Runtime.InteropServices;
using RefraSin.ParticleModel;

namespace Refrasin.HDF5Storage;

internal struct ParticleCompound(IParticle particle)
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string Id = particle.Id.ToString();

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public double[] CenterCoordinates = particle.CenterCoordinates.ToArray();

    public double RotationAngle = particle.RotationAngle;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string MaterialId = particle.MaterialId.ToString();
}