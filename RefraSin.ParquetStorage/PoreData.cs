using RefraSin.ParticleModel.Pores;

namespace RefraSin.ParquetStorage;

public class PoreData : IPore, IPoreElasticStrain, IPoreDenseVolume, IPorePorosity
{
    public Guid Id { get; set; } = Guid.Empty;

    public double Volume { get; set; }

    public double ElasticStrain { get; set; }

    public double DenseVolume { get; set; }

    public double Porosity { get; set; }

    public static PoreData From(IPore pore)
    {
        if (pore is null)
            return null;

        var self = new PoreData { Id = pore.Id, Volume = pore.Volume };

        if (pore is IPoreElasticStrain poreElasticStrain)
            self.ElasticStrain = poreElasticStrain.ElasticStrain;

        if (pore is IPoreDenseVolume poreDenseVolume)
            self.DenseVolume = poreDenseVolume.DenseVolume;

        if (pore is IPorePorosity porePorosity)
            self.Porosity = porePorosity.Porosity;

        return self;
    }
}
