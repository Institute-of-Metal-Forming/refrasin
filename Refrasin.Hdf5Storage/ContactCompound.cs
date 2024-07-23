using System.Runtime.InteropServices;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Particles;

namespace Refrasin.HDF5Storage;

internal struct ContactCompound(IParticleContactEdge contact)
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string From = contact.From.ToString();

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string To = contact.To.ToString();

    public double Distance = contact.Distance;

    public double DirectionTo = contact.DirectionTo;

    public double DirectionFrom = contact.DirectionFrom;
}