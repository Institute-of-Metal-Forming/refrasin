using System.Drawing;
using RefraSin.Graphs;
using RefraSin.TEPSolver.EquationSystem;
using RefraSin.TEPSolver.ParticleModel;

namespace RefraSin.TEPSolver.StepVectors;

public class StepVectorMap
{
    public StepVectorMap(EquationSystem.EquationSystem equationSystem)
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
            else
                throw new ArgumentException($"Invalid quantity type: {constraint.GetType()}");

            index++;
        }

        TotalLength = index;
    }

    public int TotalLength { get; }

    private Dictionary<Type, int> _globalQuantityIndexMap = new();
    private Dictionary<(Type, Particle), int> _particleQuantityIndexMap = new();
    private Dictionary<(Type, NodeBase), int> _nodeQuantityIndexMap = new();

    public int QuantityIndex<TQuantity>()
        where TQuantity : IGlobalQuantity => _globalQuantityIndexMap[typeof(TQuantity)];

    public int QuantityIndex<TQuantity>(Particle particle)
        where TQuantity : IParticleQuantity =>
        _particleQuantityIndexMap[(typeof(TQuantity), particle)];

    public int QuantityIndex<TQuantity>(NodeBase node)
        where TQuantity : INodeQuantity => _nodeQuantityIndexMap[(typeof(TQuantity), node)];

    private Dictionary<Type, IQuantity> _globalQuantityInstanceMap = new();
    private Dictionary<(Type, Particle), IQuantity> _particleQuantityInstanceMap = new();
    private Dictionary<(Type, NodeBase), IQuantity> _nodeQuantityInstanceMap = new();

    public TQuantity QuantityInstance<TQuantity>()
        where TQuantity : IGlobalQuantity =>
        (TQuantity)_globalQuantityInstanceMap[typeof(TQuantity)];

    public TQuantity QuantityInstance<TQuantity>(Particle particle)
        where TQuantity : IParticleQuantity =>
        (TQuantity)_particleQuantityInstanceMap[(typeof(TQuantity), particle)];

    public TQuantity QuantityInstance<TQuantity>(NodeBase node)
        where TQuantity : INodeQuantity =>
        (TQuantity)_nodeQuantityInstanceMap[(typeof(TQuantity), node)];

    private Dictionary<Type, int> _globalConstraintIndexMap = new();
    private Dictionary<(Type, Particle), int> _particleConstraintIndexMap = new();
    private Dictionary<(Type, NodeBase), int> _nodeConstraintIndexMap = new();

    public int ConstraintIndex<TConstraint>()
        where TConstraint : IGlobalConstraint => _globalConstraintIndexMap[typeof(TConstraint)];

    public int ConstraintIndex<TConstraint>(Particle particle)
        where TConstraint : IParticleConstraint =>
        _particleConstraintIndexMap[(typeof(TConstraint), particle)];

    public int ConstraintIndex<TConstraint>(NodeBase node)
        where TConstraint : INodeConstraint => _nodeConstraintIndexMap[(typeof(TConstraint), node)];

    private Dictionary<Type, IConstraint> _globalConstraintInstanceMap = new();
    private Dictionary<(Type, Particle), IConstraint> _particleConstraintInstanceMap = new();
    private Dictionary<(Type, NodeBase), IConstraint> _nodeConstraintInstanceMap = new();

    public TConstraint ConstraintInstance<TConstraint>()
        where TConstraint : IGlobalConstraint =>
        (TConstraint)_globalConstraintInstanceMap[typeof(TConstraint)];

    public TConstraint ConstraintInstance<TConstraint>(Particle particle)
        where TConstraint : IParticleConstraint =>
        (TConstraint)_particleConstraintInstanceMap[(typeof(TConstraint), particle)];

    public TConstraint ConstraintInstance<TConstraint>(NodeBase node)
        where TConstraint : INodeConstraint =>
        (TConstraint)_nodeConstraintInstanceMap[(typeof(TConstraint), node)];
}
