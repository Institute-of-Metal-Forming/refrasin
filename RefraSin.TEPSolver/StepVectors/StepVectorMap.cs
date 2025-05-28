using System.Diagnostics.CodeAnalysis;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;

namespace RefraSin.TEPSolver.StepVectors;

public class StepVectorMap
{
    public StepVectorMap(EquationSystem equationSystem)
    {
        int index = 0;

        foreach (var item in equationSystem.Items)
        {
            if (item is IGlobalItem globalItem)
            {
                var key = (globalItem.GetType(), Guid.Empty);
                _itemIndexMap.Add(key, index);
            }
            else if (item is IParticleItem particleItem)
            {
                var key = (particleItem.GetType(), particleItem.Particle.Id);
                _itemIndexMap.Add(key, index);
            }
            else if (item is INodeItem nodeItem)
            {
                var key = (nodeItem.GetType(), nodeItem.Node.Id);
                _itemIndexMap.Add(key, index);
            }
            else if (item is INodeContactItem nodeContactItem)
            {
                var key = (nodeContactItem.GetType(), nodeContactItem.NodeContact.Id);
                _itemIndexMap.Add(key, index);
            }
            else if (item is IParticleContactItem particleContactItem)
            {
                var key = (particleContactItem.GetType(), particleContactItem.ParticleContact.Id);
                _itemIndexMap.Add(key, index);
            }
            else
                throw new ArgumentException($"Invalid item type: {item.GetType()}");

            index++;
        }
    }

    private readonly Dictionary<(Type, Guid), int> _itemIndexMap = new();

    public int ItemIndex<TItem>()
        where TItem : IGlobalItem => _itemIndexMap[(typeof(TItem), Guid.Empty)];

    public int ItemIndex<TItem>(Particle particle)
        where TItem : IParticleItem => _itemIndexMap[(typeof(TItem), particle.Id)];

    public int ItemIndex<TItem>(NodeBase node)
        where TItem : INodeItem => _itemIndexMap[(typeof(TItem), node.Id)];

    public int ItemIndex<TItem>(ContactPair<NodeBase> nodeContact)
        where TItem : INodeContactItem => _itemIndexMap[(typeof(TItem), nodeContact.Id)];

    public int ItemIndex<TItem>(ContactPair<Particle> particleContact)
        where TItem : IParticleContactItem => _itemIndexMap[(typeof(TItem), particleContact.Id)];

    public int ItemIndex(ISystemItem item)
    {
        if (item is IGlobalItem globalItem)
            return _itemIndexMap[(globalItem.GetType(), Guid.Empty)];
        if (item is IParticleItem particleItem)
            return _itemIndexMap[(particleItem.GetType(), particleItem.Particle.Id)];
        if (item is INodeItem nodeItem)
            return _itemIndexMap[(nodeItem.GetType(), nodeItem.Node.Id)];
        if (item is INodeContactItem nodeContactItem)
            return _itemIndexMap[(nodeContactItem.GetType(), nodeContactItem.NodeContact.Id)];
        if (item is IParticleContactItem particleContactItem)
            return _itemIndexMap[
                (particleContactItem.GetType(), particleContactItem.ParticleContact.Id)
            ];
        throw new ArgumentException($"Invalid item type: {item.GetType()}");
    }

    public bool HasItem<TItem>()
        where TItem : IParticleItem => _itemIndexMap.ContainsKey((typeof(TItem), Guid.Empty));

    public bool HasItem<TItem>(Particle particle)
        where TItem : IParticleItem => _itemIndexMap.ContainsKey((typeof(TItem), particle.Id));

    public bool HasItem<TItem>(ContactPair<Particle> particleContact)
        where TItem : IParticleContactItem =>
        _itemIndexMap.ContainsKey((typeof(TItem), particleContact.Id));

    public bool HasItem<TItem>(NodeBase node)
        where TItem : INodeItem => _itemIndexMap.ContainsKey((typeof(TItem), node.Id));

    public bool HasItem<TItem>(ContactPair<NodeBase> nodeContact)
        where TItem : INodeContactItem =>
        _itemIndexMap.ContainsKey((typeof(TItem), nodeContact.Id));
}
