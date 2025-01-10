using System.Runtime.InteropServices;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;

namespace Refrasin.HDF5Storage;

internal struct NodeCompound
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string Id;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] Coordinates;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] AbsoluteCoordinates = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] SurfaceDistance = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] SurfaceRadiusAngle = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] AngleDistance = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] Volume = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] SurfaceAngle = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] InterfaceEnergy = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] InterfaceDiffusionCoefficient = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] GibbsEnergyGradient = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] VolumeGradient = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string ContactedParticleId = string.Empty;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string ContactedNodeId = string.Empty;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] InterfaceFlux = [0.0, 0.0];

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] VolumeFlux = [0.0, 0.0];

    public readonly double TransferFlux = 0.0;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] Shift = [0.0, 0.0];

    public NodeCompound(INode node)
    {
        Id = node.Id.ToString();
        Coordinates = node.Coordinates.ToArray();
        AbsoluteCoordinates = node.Coordinates.Absolute.ToArray();

        if (node is INodeGeometry nodeGeometry)
        {
            SurfaceDistance = nodeGeometry.SurfaceDistance.ToArray();
            SurfaceRadiusAngle = nodeGeometry.SurfaceRadiusAngle.ToDoubleArray();
            AngleDistance = nodeGeometry.SurfaceRadiusAngle.ToDoubleArray();
            Volume = nodeGeometry.Volume.ToArray();
            SurfaceAngle = nodeGeometry.SurfaceRadiusAngle.ToDoubleArray();
        }

        if (node is INodeMaterialProperties nodeMaterialProperties)
        {
            InterfaceEnergy = nodeMaterialProperties.InterfaceEnergy.ToArray();
            InterfaceDiffusionCoefficient =
                nodeMaterialProperties.InterfaceDiffusionCoefficient.ToArray();
        }

        if (node is INodeGradients nodeGradients)
        {
            GibbsEnergyGradient = nodeGradients.GibbsEnergyGradient.ToArray();
            VolumeGradient = nodeGradients.VolumeGradient.ToArray();
        }

        if (node is INodeContact nodeContact)
        {
            ContactedParticleId = nodeContact.ContactedParticleId.ToString();
            ContactedNodeId = nodeContact.ContactedNodeId.ToString();
        }

        if (node is INodeFluxes nodeFluxes)
        {
            InterfaceFlux = nodeFluxes.InterfaceFlux.ToArray();
            VolumeFlux = nodeFluxes.VolumeFlux.ToArray();
            TransferFlux = nodeFluxes.TransferFlux;
        }

        if (node is INodeShifts nodeShifts)
        {
            Shift = nodeShifts.Shift.ToArray();
        }
    }
}
