using RefraSin.Coordinates;

namespace RefraSin.ParticleModel.Nodes;

public interface INodeContactGradients : INodeContact
{
    NormalTangential<Angle> CenterShiftVectorDirection { get; }

    NormalTangential<double> ContactDistanceGradient { get; }

    NormalTangential<double> ContactDirectionGradient { get; }
}