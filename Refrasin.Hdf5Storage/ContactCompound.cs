using System.Runtime.InteropServices;
using RefraSin.ParticleModel;

namespace Refrasin.HDF5Storage;

internal struct ContactCompound(IParticleContact contact)
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string From = contact.From.Id.ToString();

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public string To = contact.To.Id.ToString();

    public double Distance = contact.Distance;

    public double DirectionTo = contact.DirectionTo;

    public double DirectionFrom = contact.DirectionFrom;
}