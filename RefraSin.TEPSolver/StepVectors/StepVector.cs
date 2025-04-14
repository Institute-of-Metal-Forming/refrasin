using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RefraSin.ParticleModel;
using RefraSin.TEPSolver.Constraints;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.Quantities;

namespace RefraSin.TEPSolver.StepVectors;

public class StepVector : DenseVector
{
    /// <inheritdoc />
    public StepVector(double[] storage, StepVectorMap stepVectorMap)
        : base(storage)
    {
        StepVectorMap = stepVectorMap;
    }

    /// <inheritdoc />
    public StepVector(Vector<double> vector, StepVectorMap stepVectorMap)
        : base(vector.AsArray() ?? vector.ToArray())
    {
        StepVectorMap = stepVectorMap;
    }

    public StepVectorMap StepVectorMap { get; }

    public double QuantityValue<TQuantity>()
        where TQuantity : IGlobalQuantity => this[StepVectorMap.QuantityIndex<TQuantity>()];

    public double QuantityValue<TQuantity>(Particle particle)
        where TQuantity : IParticleQuantity =>
        this[StepVectorMap.QuantityIndex<TQuantity>(particle)];

    public double QuantityValue<TQuantity>(NodeBase node)
        where TQuantity : INodeQuantity => this[StepVectorMap.QuantityIndex<TQuantity>(node)];

    public double QuantityValue<TQuantity>(ContactPair<NodeBase> nodeContact)
        where TQuantity : INodeContactQuantity =>
        this[StepVectorMap.QuantityIndex<TQuantity>(nodeContact)];

    public double QuantityValue<TQuantity>(ContactPair<Particle> particleContact)
        where TQuantity : IParticleContactQuantity =>
        this[StepVectorMap.QuantityIndex<TQuantity>(particleContact)];

    public double QuantityValue(IQuantity quantity) => this[StepVectorMap.QuantityIndex(quantity)];

    public void SetQuantityValue<TQuantity>(double value)
        where TQuantity : IGlobalQuantity => this[StepVectorMap.QuantityIndex<TQuantity>()] = value;

    public void SetQuantityValue<TQuantity>(Particle particle, double value)
        where TQuantity : IParticleQuantity =>
        this[StepVectorMap.QuantityIndex<TQuantity>(particle)] = value;

    public void SetQuantityValue<TQuantity>(NodeBase node, double value)
        where TQuantity : INodeQuantity =>
        this[StepVectorMap.QuantityIndex<TQuantity>(node)] = value;

    public void SetQuantityValue<TQuantity>(ContactPair<NodeBase> nodeContact, double value)
        where TQuantity : INodeContactQuantity =>
        this[StepVectorMap.QuantityIndex<TQuantity>(nodeContact)] = value;

    public void SetQuantityValue<TQuantity>(ContactPair<Particle> particleContact, double value)
        where TQuantity : IParticleContactQuantity =>
        this[StepVectorMap.QuantityIndex<TQuantity>(particleContact)] = value;

    public void SetQuantityValue(IQuantity quantity, double value) =>
        this[StepVectorMap.QuantityIndex(quantity)] = value;

    public double ConstraintLambdaValue<TConstraint>()
        where TConstraint : IGlobalConstraint => this[StepVectorMap.ConstraintIndex<TConstraint>()];

    public double ConstraintLambdaValue<TConstraint>(Particle particle)
        where TConstraint : IParticleConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(particle)];

    public double ConstraintLambdaValue<TConstraint>(NodeBase node)
        where TConstraint : INodeConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(node)];

    public double ConstraintLambdaValue<TConstraint>(ContactPair<NodeBase> nodeContact)
        where TConstraint : INodeContactConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(nodeContact)];

    public double ConstraintLambdaValue<TConstraint>(ContactPair<Particle> particleContact)
        where TConstraint : IParticleContactConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(particleContact)];

    public double ConstraintLambdaValue(IConstraint constraint) =>
        this[StepVectorMap.ConstraintIndex(constraint)];

    public void SetConstraintLambdaValue<TConstraint>(double value)
        where TConstraint : IGlobalConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>()] = value;

    public void SetConstraintLambdaValue<TConstraint>(Particle particle, double value)
        where TConstraint : IParticleConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(particle)] = value;

    public void SetConstraintLambdaValue<TConstraint>(NodeBase node, double value)
        where TConstraint : INodeConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(node)] = value;

    public void SetConstraintLambdaValue<TConstraint>(
        ContactPair<NodeBase> nodeContact,
        double value
    )
        where TConstraint : INodeContactConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(nodeContact)] = value;

    public void SetConstraintLambdaValue<TConstraint>(
        ContactPair<Particle> particleContact,
        double value
    )
        where TConstraint : IParticleContactConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(particleContact)] = value;

    public void SetConstraintLambdaValue(IConstraint constraint, double value) =>
        this[StepVectorMap.ConstraintIndex(constraint)] = value;

    public static StepVector operator +(StepVector leftSide, StepVector rightSide) =>
        new((DenseVector)leftSide + rightSide, leftSide.StepVectorMap);

    public static StepVector operator -(StepVector leftSide, StepVector rightSide) =>
        new((DenseVector)leftSide - rightSide, leftSide.StepVectorMap);

    public static StepVector operator *(StepVector leftSide, double rightSide) =>
        new((DenseVector)leftSide * rightSide, leftSide.StepVectorMap);

    public static StepVector operator *(double leftSide, StepVector rightSide) =>
        rightSide * leftSide;

    public static StepVector operator /(StepVector leftSide, double rightSide) =>
        new((DenseVector)leftSide / rightSide, leftSide.StepVectorMap);

    public StepVector Copy() => new(Build.DenseOfVector(this), StepVectorMap);

    public void Update(double[] data)
    {
        data.CopyTo(Values, 0);
    }
}
