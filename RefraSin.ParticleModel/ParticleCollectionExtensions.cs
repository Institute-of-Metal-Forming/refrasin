namespace RefraSin.ParticleModel;

public static class ParticleCollectionExtensions
{
    public static ReadOnlyParticleCollection<TParticle> ToReadOnlyParticleCollection<TParticle>(this IEnumerable<TParticle> source)
        where TParticle : IParticle => new(source);

    public static Dictionary<Guid, TParticle> ToDictionaryById<TParticle>(this IEnumerable<TParticle> source)
        where TParticle : IParticle =>
        source.ToDictionary(n => n.Id, n => n);
}