namespace RefraSin.ParticleModel.Particles;

public interface IParticleMeasures
{
    double MaxRadius { get; }

    double MinRadius { get; }

    double MinX { get; }

    double MinY { get; }

    double MaxX { get; }

    double MaxY { get; }
}
