using System.Runtime.InteropServices;
using RefraSin.ParticleModel;

namespace Refrasin.HDF5Storage;

internal struct NodeTimeStepCompound
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
    public readonly string NodeId;

    public readonly double NormalDisplacement;

    public readonly double TangentialDisplacement;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public readonly double[] DiffusionalFlow;

    public readonly double OuterDiffusionalFlow;

    public readonly double VolumeChange;

    public NodeTimeStepCompound(INodeTimeStep nodeTimeStep)
    {
        NodeId = nodeTimeStep.NodeId.ToString();
        NormalDisplacement = nodeTimeStep.NormalDisplacement;
        TangentialDisplacement = nodeTimeStep.TangentialDisplacement;
        DiffusionalFlow = nodeTimeStep.DiffusionalFlow.ToArray();
        OuterDiffusionalFlow = nodeTimeStep.OuterDiffusionalFlow;
        VolumeChange = nodeTimeStep.OuterDiffusionalFlow;
    }
}