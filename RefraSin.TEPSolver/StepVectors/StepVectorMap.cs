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
                var key = (globalQuantity.GetType(), Guid.Empty);
                _quantityIndexMap.Add(key, index);
            }
            else if (quantity is IParticleQuantity particleQuantity)
            {
                var key = (particleQuantity.GetType(), particleQuantity.Particle.Id);
                _quantityIndexMap.Add(key, index);
            }
            else if (quantity is INodeQuantity nodeQuantity)
            {
                var key = (nodeQuantity.GetType(), nodeQuantity.Node.Id);
                _quantityIndexMap.Add(key, index);
            }
            else if (quantity is INodeContactQuantity nodeContactQuantity)
            {
                var key = (nodeContactQuantity.GetType(), nodeContactQuantity.NodeContact.Id);
                _quantityIndexMap.Add(key, index);
            }
            else if (quantity is IParticleContactQuantity particleContactQuantity)
            {
                var key = (
                    particleContactQuantity.GetType(),
                    particleContactQuantity.ParticleContact.Id
                );
                _quantityIndexMap.Add(key, index);
            }
            else
                throw new ArgumentException($"Invalid quantity type: {quantity.GetType()}");

            index++;
        }

        foreach (var constraint in equationSystem.Constraints)
        {
            if (constraint is IGlobalConstraint globalConstraint)
            {
                var key = (globalConstraint.GetType(), Guid.Empty);
                _constraintIndexMap.Add(key, index);
            }
            else if (constraint is IParticleConstraint particleConstraint)
            {
                var key = (particleConstraint.GetType(), particleConstraint.Particle.Id);
                _constraintIndexMap.Add(key, index);
            }
            else if (constraint is INodeConstraint nodeConstraint)
            {
                var key = (nodeConstraint.GetType(), nodeConstraint.Node.Id);
                _constraintIndexMap.Add(key, index);
            }
            else if (constraint is INodeContactConstraint nodeContactConstraint)
            {
                var key = (nodeContactConstraint.GetType(), nodeContactConstraint.NodeContact.Id);
                _constraintIndexMap.Add(key, index);
            }
            else if (constraint is IParticleContactConstraint particleContactConstraint)
            {
                var key = (
                    particleContactConstraint.GetType(),
                    particleContactConstraint.ParticleContact.Id
                );
                _constraintIndexMap.Add(key, index);
            }
            else
                throw new ArgumentException($"Invalid quantity type: {constraint.GetType()}");

            index++;
        }

        TotalLength = index;
    }

    public int TotalLength { get; }

    private readonly Dictionary<(Type, Guid), int> _quantityIndexMap = new();

    public int QuantityIndex<TQuantity>()
        where TQuantity : IGlobalQuantity => _quantityIndexMap[(typeof(TQuantity), Guid.Empty)];

    public int QuantityIndex<TQuantity>(Particle particle)
        where TQuantity : IParticleQuantity => _quantityIndexMap[(typeof(TQuantity), particle.Id)];

    public int QuantityIndex<TQuantity>(NodeBase node)
        where TQuantity : INodeQuantity => _quantityIndexMap[(typeof(TQuantity), node.Id)];

    public int QuantityIndex<TQuantity>(ContactPair<NodeBase> nodeContact)
        where TQuantity : INodeContactQuantity =>
        _quantityIndexMap[(typeof(TQuantity), nodeContact.Id)];

    public int QuantityIndex<TQuantity>(ContactPair<Particle> particleContact)
        where TQuantity : IParticleContactQuantity =>
        _quantityIndexMap[(typeof(TQuantity), particleContact.Id)];

    public int QuantityIndex(IQuantity quantity)
    {
        if (quantity is IGlobalQuantity globalQuantity)
            return _quantityIndexMap[(globalQuantity.GetType(), Guid.Empty)];
        if (quantity is IParticleQuantity particleQuantity)
            return _quantityIndexMap[(particleQuantity.GetType(), particleQuantity.Particle.Id)];
        if (quantity is INodeQuantity nodeQuantity)
            return _quantityIndexMap[(nodeQuantity.GetType(), nodeQuantity.Node.Id)];
        if (quantity is INodeContactQuantity nodeContactQuantity)
            return _quantityIndexMap[
                (nodeContactQuantity.GetType(), nodeContactQuantity.NodeContact.Id)
            ];
        if (quantity is IParticleContactQuantity particleContactQuantity)
            return _quantityIndexMap[
                (particleContactQuantity.GetType(), particleContactQuantity.ParticleContact.Id)
            ];
        throw new ArgumentException($"Invalid quantity type: {quantity.GetType()}");
    }

    public bool HasQuantity<TQuantity>()
        where TQuantity : IParticleQuantity =>
        _quantityIndexMap.ContainsKey((typeof(TQuantity), Guid.Empty));

    public bool HasQuantity<TQuantity>(Particle particle)
        where TQuantity : IParticleQuantity =>
        _quantityIndexMap.ContainsKey((typeof(TQuantity), particle.Id));

    public bool HasQuantity<TQuantity>(ContactPair<Particle> particleContact)
        where TQuantity : IParticleContactQuantity =>
        _quantityIndexMap.ContainsKey((typeof(TQuantity), particleContact.Id));

    public bool HasQuantity<TQuantity>(NodeBase node)
        where TQuantity : INodeQuantity =>
        _quantityIndexMap.ContainsKey((typeof(TQuantity), node.Id));

    public bool HasQuantity<TQuantity>(ContactPair<NodeBase> nodeContact)
        where TQuantity : INodeContactQuantity =>
        _quantityIndexMap.ContainsKey((typeof(TQuantity), nodeContact.Id));

    private readonly Dictionary<(Type, Guid), int> _constraintIndexMap = new();

    public int ConstraintIndex<TConstraint>()
        where TConstraint : IGlobalConstraint =>
        _constraintIndexMap[(typeof(TConstraint), Guid.Empty)];

    public int ConstraintIndex<TConstraint>(Particle particle)
        where TConstraint : IParticleConstraint =>
        _constraintIndexMap[(typeof(TConstraint), particle.Id)];

    public int ConstraintIndex<TConstraint>(NodeBase node)
        where TConstraint : INodeConstraint => _constraintIndexMap[(typeof(TConstraint), node.Id)];

    public int ConstraintIndex<TConstraint>(ContactPair<NodeBase> nodeContact)
        where TConstraint : INodeContactConstraint =>
        _constraintIndexMap[(typeof(TConstraint), nodeContact.Id)];

    public int ConstraintIndex<TConstraint>(ContactPair<Particle> particleContact)
        where TConstraint : IParticleContactConstraint =>
        _constraintIndexMap[(typeof(TConstraint), particleContact.Id)];

    public int ConstraintIndex(IConstraint constraint)
    {
        if (constraint is IGlobalConstraint globalConstraint)
            return _constraintIndexMap[(globalConstraint.GetType(), Guid.Empty)];
        if (constraint is IParticleConstraint particleConstraint)
            return _constraintIndexMap[
                (particleConstraint.GetType(), particleConstraint.Particle.Id)
            ];
        if (constraint is INodeConstraint nodeConstraint)
            return _constraintIndexMap[(nodeConstraint.GetType(), nodeConstraint.Node.Id)];
        if (constraint is INodeContactConstraint nodeContactConstraint)
            return _constraintIndexMap[
                (nodeContactConstraint.GetType(), nodeContactConstraint.NodeContact.Id)
            ];
        if (constraint is IParticleContactConstraint particleContactConstraint)
            return _constraintIndexMap[
                (particleContactConstraint.GetType(), particleContactConstraint.ParticleContact.Id)
            ];
        throw new ArgumentException($"Invalid constraint type: {constraint.GetType()}");
    }

    public bool HasConstraint<TConstraint>()
        where TConstraint : IParticleConstraint =>
        _constraintIndexMap.ContainsKey((typeof(TConstraint), Guid.Empty));

    public bool HasConstraint<TConstraint>(Particle particle)
        where TConstraint : IParticleConstraint =>
        _constraintIndexMap.ContainsKey((typeof(TConstraint), particle.Id));

    public bool HasConstraint<TConstraint>(ContactPair<Particle> particleContact)
        where TConstraint : IParticleContactConstraint =>
        _constraintIndexMap.ContainsKey((typeof(TConstraint), particleContact.Id));

    public bool HasConstraint<TConstraint>(NodeBase node)
        where TConstraint : INodeConstraint =>
        _constraintIndexMap.ContainsKey((typeof(TConstraint), node.Id));

    public bool HasConstraint<TConstraint>(ContactPair<NodeBase> nodeContact)
        where TConstraint : INodeContactConstraint =>
        _constraintIndexMap.ContainsKey((typeof(TConstraint), nodeContact.Id));
}
