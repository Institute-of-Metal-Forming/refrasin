using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
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

    public double QuantityValue(IQuantity quantity) => this[StepVectorMap.QuantityIndex(quantity)];

    public double ConstraintLambdaValue<TConstraint>()
        where TConstraint : IGlobalConstraint => this[StepVectorMap.ConstraintIndex<TConstraint>()];

    public double ConstraintLambdaValue<TConstraint>(Particle particle)
        where TConstraint : IParticleConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(particle)];

    public double ConstraintLambdaValue<TConstraint>(NodeBase node)
        where TConstraint : INodeConstraint =>
        this[StepVectorMap.ConstraintIndex<TConstraint>(node)];

    public double ConstraintLambdaValue(IConstraint constraint) =>
        this[StepVectorMap.ConstraintIndex(constraint)];

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
