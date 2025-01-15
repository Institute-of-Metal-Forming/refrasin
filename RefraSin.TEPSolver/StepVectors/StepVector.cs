using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using RefraSin.Graphs;
using RefraSin.TEPSolver.ParticleModel;

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

    public double LambdaDissipation() => this[StepVectorMap.LambdaDissipation()];

    public double NormalDisplacement(INode node) => this[StepVectorMap.NormalDisplacement(node)];

    public double FluxToUpper(INode node) => this[StepVectorMap.FluxToUpper(node)];

    public double LambdaVolume(INode node) => this[StepVectorMap.LambdaVolume(node)];

    public double TangentialDisplacement(INode node) =>
        this[StepVectorMap.TangentialDisplacement(node)];

    public double LambdaContactX(INode node) => this[StepVectorMap.LambdaContactX(node)];

    public double LambdaContactY(INode node) => this[StepVectorMap.LambdaContactY(node)];

    public double ParticleDisplacementX(Particle particle) =>
        this[StepVectorMap.ParticleDisplacementX(particle)];

    public double ParticleDisplacementY(Particle particle) =>
        this[StepVectorMap.ParticleDisplacementY(particle)];

    public void LambdaDissipation(double value) => this[StepVectorMap.LambdaDissipation()] = value;

    public void NormalDisplacement(INode node, double value) =>
        this[StepVectorMap.NormalDisplacement(node)] = value;

    public void FluxToUpper(INode node, double value) =>
        this[StepVectorMap.FluxToUpper(node)] = value;

    public void LambdaVolume(INode node, double value) =>
        this[StepVectorMap.LambdaVolume(node)] = value;

    public void TangentialDisplacement(INode node, double value) =>
        this[StepVectorMap.TangentialDisplacement(node)] = value;

    public void LambdaContactX(INode node, double value) =>
        this[StepVectorMap.LambdaContactX(node)] = value;

    public void LambdaContactY(INode node, double value) =>
        this[StepVectorMap.LambdaContactY(node)] = value;

    public void ParticleDisplacementX(Particle particle, double value) =>
        this[StepVectorMap.ParticleDisplacementX(particle)] = value;

    public void ParticleDisplacementY(Particle particle, double value) =>
        this[StepVectorMap.ParticleDisplacementY(particle)] = value;

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
