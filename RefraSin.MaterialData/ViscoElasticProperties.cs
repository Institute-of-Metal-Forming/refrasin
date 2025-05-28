namespace RefraSin.MaterialData;

public record ViscoElasticProperties(double CompressionModulus, double VolumeViscosity)
    : IViscoElasticProperties { }
