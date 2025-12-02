namespace RefraSin.MaterialData;

public interface IViscoElasticProperties
{
    public double ElasticModulus { get; }
    double ShearViscosity { get; }
}
