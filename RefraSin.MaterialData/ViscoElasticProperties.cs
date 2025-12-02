namespace RefraSin.MaterialData;

public record ViscoElasticProperties(double ElasticModulus, double ShearViscosity)
    : IViscoElasticProperties { }
