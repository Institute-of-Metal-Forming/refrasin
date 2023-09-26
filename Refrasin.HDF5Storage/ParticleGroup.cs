using RefraSin.ParticleModel;

namespace Refrasin.HDF5Storage;

internal class ParticleGroup
{
    public string Id;

    public double[] AbsoluteCenterCoordinates;

    public double RotationAngle;

    public string MaterialId;

    public double[] CenterCoordinates;

    public NodeCompound[] Nodes;

    public ParticleGroup(IParticle particle)
    {
        Id = particle.Id.ToString();
        AbsoluteCenterCoordinates = particle.AbsoluteCenterCoordinates.ToArray();
        RotationAngle = particle.RotationAngle;
        MaterialId = particle.MaterialId.ToString();
        CenterCoordinates = particle.CenterCoordinates.ToArray();
        Nodes = particle.Nodes.Select(n => new NodeCompound(n)).ToArray();
    }
}