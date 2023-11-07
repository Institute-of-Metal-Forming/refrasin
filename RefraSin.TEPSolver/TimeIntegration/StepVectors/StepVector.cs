using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.TimeIntegration.StepVectors;

public class StepVector : DenseVector
{
    /// <inheritdoc />
    public StepVector(double[] storage, StepVectorMap stepVectorMap) : base(storage)
    {
        StepVectorMap = stepVectorMap;
    }

    /// <inheritdoc />
    public StepVector(Vector<double> vector, StepVectorMap stepVectorMap) : base(vector.AsArray() ?? vector.ToArray())
    {
        StepVectorMap = stepVectorMap;
    }

    public StepVectorMap StepVectorMap { get; }

    public NodeView this[INodeSpec node] => new(this, node.Id);

    public ParticleView this[IParticleSpec particle] => new(this, particle.Id);

    public double Lambda1 => this[StepVectorMap.GetIndex(GlobalUnknown.Lambda1)];

    public static StepVector operator +(StepVector leftSide, StepVector rightSide) => new((DenseVector)leftSide + rightSide, leftSide.StepVectorMap);
    public static StepVector operator -(StepVector leftSide, StepVector rightSide) => new((DenseVector)leftSide - rightSide, leftSide.StepVectorMap);
    public static StepVector operator *(StepVector leftSide, double rightSide) => new((DenseVector)leftSide * rightSide, leftSide.StepVectorMap);
    public static StepVector operator *(double leftSide, StepVector rightSide) => rightSide * leftSide;
    public static StepVector operator /(StepVector leftSide, double rightSide) => new((DenseVector)leftSide / rightSide, leftSide.StepVectorMap);
}