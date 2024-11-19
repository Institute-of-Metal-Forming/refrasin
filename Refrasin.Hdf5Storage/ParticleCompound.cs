using System.Runtime.InteropServices;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Particles;

namespace Refrasin.HDF5Storage;

internal struct ParticleCompound(IParticle particle)
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string Id = particle.Id.ToString();

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public double[] CenterCoordinates = particle.Coordinates.ToArray();

    public double RotationAngle = particle.RotationAngle;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string MaterialId = particle.MaterialId.ToString();
}
