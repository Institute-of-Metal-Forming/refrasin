using System.Runtime.InteropServices;
using RefraSin.ParticleModel;

namespace Refrasin.HDF5Storage;

internal struct NodeCompound
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string Id;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] Coordinates;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] AbsoluteCoordinates;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] SurfaceDistance;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] SurfaceRadiusAngle;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] AngleDistance;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] Volume;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] SurfaceAngle;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] SurfaceEnergy;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] SurfaceDiffusionCoefficient;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] GibbsEnergyGradient;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] VolumeGradient;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string ContactedParticleId;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string ContactedNodeId;

    public readonly double TransferCoefficient;

    public NodeCompound(INode node)
    {
        Id = node.Id.ToString();
        Coordinates = node.Coordinates.ToArray();
        AbsoluteCoordinates = node.Coordinates.ToArray();
        SurfaceDistance = node.SurfaceDistance.ToArray();
        SurfaceRadiusAngle = node.SurfaceRadiusAngle.ToDoubleArray();
        AngleDistance = node.SurfaceRadiusAngle.ToDoubleArray();
        Volume = node.Volume.ToArray();
        SurfaceAngle = node.SurfaceRadiusAngle.ToDoubleArray();
        SurfaceEnergy = node.SurfaceEnergy.ToArray();
        SurfaceDiffusionCoefficient = node.SurfaceDiffusionCoefficient.ToArray();
        GibbsEnergyGradient = node.GibbsEnergyGradient.ToArray();
        VolumeGradient = node.VolumeGradient.ToArray();

        if (node is IContactNode contactNode)
        {
            ContactedParticleId = contactNode.ContactedParticleId.ToString();
            ContactedNodeId = contactNode.ContactedNodeId.ToString();
            TransferCoefficient = contactNode.TransferCoefficient;
        }
        else
        {
            ContactedParticleId = string.Empty;
            ContactedNodeId = string.Empty;
            TransferCoefficient = double.NaN;
        }
    }
}