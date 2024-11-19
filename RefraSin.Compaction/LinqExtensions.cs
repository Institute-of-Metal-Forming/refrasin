using RefraSin.Compaction.ParticleModel;
using RefraSin.ParticleModel.Particles;

namespace RefraSin.Compaction;

internal static class LinqExtensions
{
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        if (k < 0)
            throw new ArgumentException("k must be positive");

        if (k == 0)
            return
            [
                [],
            ];

        var elementsArray = elements.ToArray();

        return elementsArray.SelectMany(
            (e, i) => elementsArray.Skip(i + 1).Combinations(k - 1).Select(c => c.Append(e))
        );
    }

    public static IEnumerable<IMutableParticle<Node>> FlattenAgglomerates(
        this IEnumerable<IMutableParticle<Node>> source
    )
    {
        foreach (var body in source)
        {
            foreach (var particle in body.FlattenIfAgglomerate())
                yield return particle;
        }
    }

    public static IEnumerable<IMutableParticle<Node>> FlattenIfAgglomerate(
        this IMutableParticle<Node> body
    )
    {
        if (body is IAgglomerate agglomerate)
        {
            foreach (var element in agglomerate.Elements)
                yield return element;
        }
        else
            yield return body;
    }
}
