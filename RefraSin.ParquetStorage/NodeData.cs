using Parquet.Serialization.Attributes;
using RefraSin.Coordinates;
using RefraSin.Coordinates.Polar;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;

namespace RefraSin.ParquetStorage;

public class NodeData : INode, INodeGeometry, INodeFluxes, INodeShifts, INodeContact, INodeGradients
{
    public Guid Id { get; set; }

    [ParquetIgnore]
    public Guid ParticleId { get; set; }

    public PolarPointData Coordinates { get; set; }

    IPolarPoint INode.Coordinates => new PolarPoint(Coordinates.Phi, Coordinates.R);

    /// <inheritdoc />
    public NodeType Type { get; set; }

    public static NodeData From(INode node)
    {
        var self = new NodeData
        {
            Id = node.Id,
            ParticleId = node.ParticleId,
            Type = node.Type,
            Coordinates = PolarPointData.From(node.Coordinates),
        };

        if (node is INodeGeometry geometry)
        {
            self.SurfaceDistance = ToUpperToLowerData.From(geometry.SurfaceDistance);
            self.SurfaceRadiusAngle = ToUpperToLowerData.From(geometry.SurfaceRadiusAngle);
            self.AngleDistance = ToUpperToLowerData.From(geometry.AngleDistance);
            self.Volume = ToUpperToLowerData.From(geometry.Volume);
            self.SurfaceNormalAngle = ToUpperToLowerData.From(geometry.SurfaceNormalAngle);
            self.SurfaceTangentAngle = ToUpperToLowerData.From(geometry.SurfaceTangentAngle);
            self.RadiusNormalAngle = ToUpperToLowerData.From(geometry.RadiusNormalAngle);
            self.RadiusTangentAngle = ToUpperToLowerData.From(geometry.RadiusTangentAngle);
        }

        if (node is INodeFluxes fluxes)
        {
            self.InterfaceFlux = ToUpperToLowerData.From(fluxes.InterfaceFlux);
            self.VolumeFlux = ToUpperToLowerData.From(fluxes.VolumeFlux);
            self.TransferFlux = fluxes.TransferFlux;
        }

        if (node is INodeShifts shifts)
        {
            self.Shift = NormalTangentialData.From(shifts.Shift);
        }

        if (node is INodeContact contact)
        {
            self.ContactedParticleId = contact.ContactedParticleId;
            self.ContactedNodeId = contact.ContactedNodeId;
        }

        if (node is INodeGradients gradients)
        {
            self.GibbsEnergyGradient = NormalTangentialData.From(gradients.GibbsEnergyGradient);
            self.VolumeGradient = NormalTangentialData.From(gradients.VolumeGradient);
        }

        return self;
    }

    public ToUpperToLowerData SurfaceDistance { get; set; }
    ToUpperToLower<double> INodeGeometry.SurfaceDistance => SurfaceDistance.ToDoubleValued();

    public ToUpperToLowerData SurfaceRadiusAngle { get; set; }
    ToUpperToLower<Angle> INodeGeometry.SurfaceRadiusAngle => SurfaceRadiusAngle.ToAngleValued();

    public ToUpperToLowerData AngleDistance { get; set; }
    ToUpperToLower<Angle> INodeGeometry.AngleDistance => AngleDistance.ToAngleValued();

    public ToUpperToLowerData Volume { get; set; }
    ToUpperToLower<double> INodeGeometry.Volume => Volume.ToDoubleValued();

    public ToUpperToLowerData SurfaceNormalAngle { get; set; }
    ToUpperToLower<Angle> INodeGeometry.SurfaceNormalAngle => SurfaceNormalAngle.ToAngleValued();

    public ToUpperToLowerData SurfaceTangentAngle { get; set; }
    ToUpperToLower<Angle> INodeGeometry.SurfaceTangentAngle => SurfaceTangentAngle.ToAngleValued();

    public ToUpperToLowerData RadiusNormalAngle { get; set; }
    ToUpperToLower<Angle> INodeGeometry.RadiusNormalAngle => RadiusNormalAngle.ToAngleValued();

    public ToUpperToLowerData RadiusTangentAngle { get; set; }
    ToUpperToLower<Angle> INodeGeometry.RadiusTangentAngle => RadiusTangentAngle.ToAngleValued();

    public ToUpperToLowerData InterfaceFlux { get; set; }
    ToUpperToLower<double> INodeFluxes.InterfaceFlux => InterfaceFlux.ToDoubleValued();

    public ToUpperToLowerData VolumeFlux { get; set; }
    ToUpperToLower<double> INodeFluxes.VolumeFlux => VolumeFlux.ToDoubleValued();

    public double TransferFlux { get; set; }

    public NormalTangentialData Shift { get; set; }
    NormalTangential<double> INodeShifts.Shift => Shift.ToDoubleValued();

    public Guid ContactedNodeId { get; set; }
    public Guid ContactedParticleId { get; set; }

    public NormalTangentialData GibbsEnergyGradient { get; set; }
    NormalTangential<double> INodeGradients.GibbsEnergyGradient =>
        GibbsEnergyGradient.ToDoubleValued();

    public NormalTangentialData VolumeGradient { get; set; }
    NormalTangential<double> INodeGradients.VolumeGradient => VolumeGradient.ToDoubleValued();
}
