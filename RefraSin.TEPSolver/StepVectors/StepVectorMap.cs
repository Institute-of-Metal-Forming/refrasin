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

        foreach (var quantity in equationSystem.Quantities)
        {
            if (quantity is IGlobalQuantity globalQuantity)
            {
                var key = globalQuantity.GetType();
                _globalQuantityIndexMap.Add(key, index);
                _globalQuantityInstanceMap.Add(key, globalQuantity);
            }
            else if (quantity is IParticleQuantity particleQuantity)
            {
                var key = (particleQuantity.GetType(), particleQuantity.Particle);
                _particleQuantityIndexMap.Add(key, index);
                _particleQuantityInstanceMap.Add(key, particleQuantity);
            }
            else if (quantity is INodeQuantity nodeQuantity)
            {
                var key = (nodeQuantity.GetType(), nodeQuantity.Node);
                _nodeQuantityIndexMap.Add(key, index);
                _nodeQuantityInstanceMap.Add(key, nodeQuantity);
            }
            else if (quantity is INodeContactQuantity nodeContactQuantity)
            {
                var key = (nodeContactQuantity.GetType(), nodeContactQuantity.NodeContact);
                _nodeContactQuantityIndexMap.Add(key, index);
                _nodeContactQuantityInstanceMap.Add(key, nodeContactQuantity);
            }
            else if (quantity is IParticleContactQuantity particleContactQuantity)
            {
                var key = (
                    particleContactQuantity.GetType(),
                    particleContactQuantity.ParticleContact
                );
                _particleContactQuantityIndexMap.Add(key, index);
                _particleContactQuantityInstanceMap.Add(key, particleContactQuantity);
            }
            else
                throw new ArgumentException($"Invalid quantity type: {quantity.GetType()}");

            index++;
        }

        foreach (var constraint in equationSystem.Constraints)
        {
            if (constraint is IGlobalConstraint globalConstraint)
            {
                var key = globalConstraint.GetType();
                _globalConstraintIndexMap.Add(key, index);
                _globalConstraintInstanceMap.Add(key, globalConstraint);
            }
            else if (constraint is IParticleConstraint particleConstraint)
            {
                var key = (particleConstraint.GetType(), particleConstraint.Particle);
                _particleConstraintIndexMap.Add(key, index);
                _particleConstraintInstanceMap.Add(key, particleConstraint);
            }
            else if (constraint is INodeConstraint nodeConstraint)
            {
                var key = (nodeConstraint.GetType(), nodeConstraint.Node);
                _nodeConstraintIndexMap.Add(key, index);
                _nodeConstraintInstanceMap.Add(key, nodeConstraint);
            }
            else if (constraint is INodeContactConstraint nodeContactConstraint)
            {
                var key = (nodeContactConstraint.GetType(), nodeContactConstraint.NodeContact);
                _nodeContactConstraintIndexMap.Add(key, index);
                _nodeContactConstraintInstanceMap.Add(key, nodeContactConstraint);
            }
            else if (constraint is IParticleContactConstraint particleContactConstraint)
            {
                var key = (
                    particleContactConstraint.GetType(),
                    particleContactConstraint.ParticleContact
                );
                _particleContactConstraintIndexMap.Add(key, index);
                _particleContactConstraintInstanceMap.Add(key, particleContactConstraint);
            }
            else
                throw new ArgumentException($"Invalid quantity type: {constraint.GetType()}");

            index++;
        }

        TotalLength = index;
    }

    public int TotalLength { get; }

    private readonly Dictionary<Type, int> _globalQuantityIndexMap = new();

    private readonly Dictionary<(Type, Particle), int> _particleQuantityIndexMap = new(
        new MapEqualityComparer<Particle>()
    );

    private readonly Dictionary<(Type, NodeBase), int> _nodeQuantityIndexMap = new(
        new MapEqualityComparer<NodeBase>()
    );

    private readonly Dictionary<(Type, ContactPair<NodeBase>), int> _nodeContactQuantityIndexMap =
        new(new MapEqualityComparer<ContactPair<NodeBase>>());

    private readonly Dictionary<
        (Type, ContactPair<Particle>),
        int
    > _particleContactQuantityIndexMap = new(new MapEqualityComparer<ContactPair<Particle>>());

    public int QuantityIndex<TQuantity>()
        where TQuantity : IGlobalQuantity => _globalQuantityIndexMap[typeof(TQuantity)];

    public int QuantityIndex<TQuantity>(Particle particle)
        where TQuantity : IParticleQuantity =>
        _particleQuantityIndexMap[(typeof(TQuantity), particle)];

    public int QuantityIndex<TQuantity>(NodeBase node)
        where TQuantity : INodeQuantity => _nodeQuantityIndexMap[(typeof(TQuantity), node)];

    public int QuantityIndex<TQuantity>(ContactPair<NodeBase> nodeContact)
        where TQuantity : INodeContactQuantity =>
        _nodeContactQuantityIndexMap[(typeof(TQuantity), nodeContact)];

    public int QuantityIndex<TQuantity>(ContactPair<Particle> particleContact)
        where TQuantity : IParticleContactQuantity =>
        _particleContactQuantityIndexMap[(typeof(TQuantity), particleContact)];

    public int QuantityIndex(IQuantity quantity)
    {
        if (quantity is IGlobalQuantity globalQuantity)
            return _globalQuantityIndexMap[globalQuantity.GetType()];
        if (quantity is IParticleQuantity particleQuantity)
            return _particleQuantityIndexMap[
                (particleQuantity.GetType(), particleQuantity.Particle)
            ];
        if (quantity is INodeQuantity nodeQuantity)
            return _nodeQuantityIndexMap[(nodeQuantity.GetType(), nodeQuantity.Node)];
        if (quantity is INodeContactQuantity nodeContactQuantity)
            return _nodeContactQuantityIndexMap[
                (nodeContactQuantity.GetType(), nodeContactQuantity.NodeContact)
            ];
        if (quantity is IParticleContactQuantity particleContactQuantity)
            return _particleContactQuantityIndexMap[
                (particleContactQuantity.GetType(), particleContactQuantity.ParticleContact)
            ];
        throw new ArgumentException($"Invalid quantity type: {quantity.GetType()}");
    }

    public bool HasQuantity<TQuantity>(Particle particle)
        where TQuantity : IParticleQuantity =>
        _particleQuantityIndexMap.ContainsKey((typeof(TQuantity), particle));

    public bool HasQuantity<TQuantity>(ContactPair<Particle> particleContact)
        where TQuantity : IParticleContactQuantity =>
        _particleContactQuantityIndexMap.ContainsKey((typeof(TQuantity), particleContact));

    public bool HasQuantity<TQuantity>(NodeBase node)
        where TQuantity : INodeQuantity =>
        _nodeQuantityIndexMap.ContainsKey((typeof(TQuantity), node));

    public bool HasQuantity<TQuantity>(ContactPair<NodeBase> nodeContact)
        where TQuantity : INodeContactQuantity =>
        _nodeContactQuantityIndexMap.ContainsKey((typeof(TQuantity), nodeContact));

    private readonly Dictionary<Type, IQuantity> _globalQuantityInstanceMap = new();

    private readonly Dictionary<(Type, Particle), IQuantity> _particleQuantityInstanceMap = new(
        new MapEqualityComparer<Particle>()
    );

    private readonly Dictionary<(Type, NodeBase), IQuantity> _nodeQuantityInstanceMap = new(
        new MapEqualityComparer<NodeBase>()
    );

    private readonly Dictionary<
        (Type, ContactPair<NodeBase>),
        IQuantity
    > _nodeContactQuantityInstanceMap = new(new MapEqualityComparer<ContactPair<NodeBase>>());

    private readonly Dictionary<
        (Type, ContactPair<Particle>),
        IQuantity
    > _particleContactQuantityInstanceMap = new(new MapEqualityComparer<ContactPair<Particle>>());

    public TQuantity QuantityInstance<TQuantity>()
        where TQuantity : IGlobalQuantity =>
        (TQuantity)_globalQuantityInstanceMap[typeof(TQuantity)];

    public TQuantity QuantityInstance<TQuantity>(Particle particle)
        where TQuantity : IParticleQuantity =>
        (TQuantity)_particleQuantityInstanceMap[(typeof(TQuantity), particle)];

    public TQuantity QuantityInstance<TQuantity>(NodeBase node)
        where TQuantity : INodeQuantity =>
        (TQuantity)_nodeQuantityInstanceMap[(typeof(TQuantity), node)];

    public TQuantity QuantityInstance<TQuantity>(ContactPair<NodeBase> nodeContact)
        where TQuantity : INodeContactQuantity =>
        (TQuantity)_nodeContactQuantityInstanceMap[(typeof(TQuantity), nodeContact)];

    public TQuantity QuantityInstance<TQuantity>(ContactPair<Particle> particleContact)
        where TQuantity : IParticleContactQuantity =>
        (TQuantity)_particleContactQuantityInstanceMap[(typeof(TQuantity), particleContact)];

    public IEnumerable<IQuantity> Quantities =>
        _globalQuantityInstanceMap
            .Values.Concat(_particleQuantityInstanceMap.Values)
            .Concat(_nodeQuantityInstanceMap.Values);

    private readonly Dictionary<Type, int> _globalConstraintIndexMap = new();

    private readonly Dictionary<(Type, Particle), int> _particleConstraintIndexMap = new(
        new MapEqualityComparer<Particle>()
    );

    private readonly Dictionary<(Type, NodeBase), int> _nodeConstraintIndexMap = new(
        new MapEqualityComparer<NodeBase>()
    );

    private readonly Dictionary<(Type, ContactPair<NodeBase>), int> _nodeContactConstraintIndexMap =
        new(new MapEqualityComparer<ContactPair<NodeBase>>());

    private readonly Dictionary<
        (Type, ContactPair<Particle>),
        int
    > _particleContactConstraintIndexMap = new(new MapEqualityComparer<ContactPair<Particle>>());

    public int ConstraintIndex<TConstraint>()
        where TConstraint : IGlobalConstraint => _globalConstraintIndexMap[typeof(TConstraint)];

    public int ConstraintIndex<TConstraint>(Particle particle)
        where TConstraint : IParticleConstraint =>
        _particleConstraintIndexMap[(typeof(TConstraint), particle)];

    public int ConstraintIndex<TConstraint>(NodeBase node)
        where TConstraint : INodeConstraint => _nodeConstraintIndexMap[(typeof(TConstraint), node)];

    public int ConstraintIndex<TConstraint>(ContactPair<NodeBase> nodeContact)
        where TConstraint : INodeContactConstraint =>
        _nodeContactConstraintIndexMap[(typeof(TConstraint), nodeContact)];

    public int ConstraintIndex<TConstraint>(ContactPair<Particle> particleContact)
        where TConstraint : IParticleContactConstraint =>
        _particleContactConstraintIndexMap[(typeof(TConstraint), particleContact)];

    public int ConstraintIndex(IConstraint constraint)
    {
        if (constraint is IGlobalConstraint globalConstraint)
            return _globalConstraintIndexMap[globalConstraint.GetType()];
        if (constraint is IParticleConstraint particleConstraint)
            return _particleConstraintIndexMap[
                (particleConstraint.GetType(), particleConstraint.Particle)
            ];
        if (constraint is INodeConstraint nodeConstraint)
            return _nodeConstraintIndexMap[(nodeConstraint.GetType(), nodeConstraint.Node)];
        if (constraint is INodeContactConstraint nodeContactConstraint)
            return _nodeContactConstraintIndexMap[
                (nodeContactConstraint.GetType(), nodeContactConstraint.NodeContact)
            ];
        if (constraint is IParticleContactConstraint particleContactConstraint)
            return _particleContactConstraintIndexMap[
                (particleContactConstraint.GetType(), particleContactConstraint.ParticleContact)
            ];
        throw new ArgumentException($"Invalid constraint type: {constraint.GetType()}");
    }

    private readonly Dictionary<Type, IConstraint> _globalConstraintInstanceMap = new();

    private readonly Dictionary<(Type, Particle), IConstraint> _particleConstraintInstanceMap = new(
        new MapEqualityComparer<Particle>()
    );

    private readonly Dictionary<(Type, NodeBase), IConstraint> _nodeConstraintInstanceMap = new(
        new MapEqualityComparer<NodeBase>()
    );

    private readonly Dictionary<
        (Type, ContactPair<NodeBase>),
        IConstraint
    > _nodeContactConstraintInstanceMap = new(new MapEqualityComparer<ContactPair<NodeBase>>());

    private readonly Dictionary<
        (Type, ContactPair<Particle>),
        IConstraint
    > _particleContactConstraintInstanceMap = new(new MapEqualityComparer<ContactPair<Particle>>());

    public TConstraint ConstraintInstance<TConstraint>()
        where TConstraint : IGlobalConstraint =>
        (TConstraint)_globalConstraintInstanceMap[typeof(TConstraint)];

    public TConstraint ConstraintInstance<TConstraint>(Particle particle)
        where TConstraint : IParticleConstraint =>
        (TConstraint)_particleConstraintInstanceMap[(typeof(TConstraint), particle)];

    public TConstraint ConstraintInstance<TConstraint>(NodeBase node)
        where TConstraint : INodeConstraint =>
        (TConstraint)_nodeConstraintInstanceMap[(typeof(TConstraint), node)];

    public TConstraint ConstraintInstance<TConstraint>(ContactPair<NodeBase> nodeContact)
        where TConstraint : INodeContactConstraint =>
        (TConstraint)_nodeContactConstraintInstanceMap[(typeof(TConstraint), nodeContact)];

    public TConstraint ConstraintInstance<TConstraint>(ContactPair<Particle> particleContact)
        where TConstraint : IParticleContactConstraint =>
        (TConstraint)_particleContactConstraintInstanceMap[(typeof(TConstraint), particleContact)];

    public IEnumerable<IConstraint> Constraints =>
        _globalConstraintInstanceMap
            .Values.Concat(_particleConstraintInstanceMap.Values)
            .Concat(_nodeConstraintInstanceMap.Values);

    private class MapEqualityComparer<T> : EqualityComparer<(Type, T)>
        where T : IVertex
    {
        public override bool Equals((Type, T) x, (Type, T) y) =>
            x.Item1 == y.Item1 && x.Item2.Id == y.Item2.Id;

        public override int GetHashCode((Type, T) obj) =>
            HashCode.Combine(obj.Item1.GetHashCode(), obj.Item2.Id.GetHashCode());
    }
}
