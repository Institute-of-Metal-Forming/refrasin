using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RefraSin.ParticleModel;

namespace RefraSin.TEPSolver.StepVectors;

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

    public NodeView this[INode node] => new(this, node.Id);
    
    public ContactNodeView this[IContactNode node] => new(this, node.Id);

    public ContactView this[IParticleContact contact] => new(this, contact.From.Id, contact.To.Id);
    public ContactView this[IParticle from, IParticle to] => new(this, from.Id, to.Id);

    public double Lambda1 => this[StepVectorMap[GlobalUnknown.Lambda1]];

    public static StepVector operator +(StepVector leftSide, StepVector rightSide) => new((DenseVector)leftSide + rightSide, leftSide.StepVectorMap);
    public static StepVector operator -(StepVector leftSide, StepVector rightSide) => new((DenseVector)leftSide - rightSide, leftSide.StepVectorMap);
    public static StepVector operator *(StepVector leftSide, double rightSide) => new((DenseVector)leftSide * rightSide, leftSide.StepVectorMap);
    public static StepVector operator *(double leftSide, StepVector rightSide) => rightSide * leftSide;
    public static StepVector operator /(StepVector leftSide, double rightSide) => new((DenseVector)leftSide / rightSide, leftSide.StepVectorMap);

    public StepVector Copy() => new(Build.DenseOfVector(this), StepVectorMap);
}