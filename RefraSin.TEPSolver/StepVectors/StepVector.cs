using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.VisualBasic.CompilerServices;
using RefraSin.ParticleModel;
using RefraSin.ParticleModel.Nodes;
using RefraSin.ParticleModel.Particles;
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
        node.Type == NodeType.Neck ? this[StepVectorMap.TangentialDisplacement(node)] : 0;

    public double LambdaContactDistance(INode node) =>
        this[StepVectorMap.LambdaContactDistance(node)];

    public double LambdaContactDirection(INode node) =>
        this[StepVectorMap.LambdaContactDirection(node)];

    public double LambdaContactRotation(ParticleContact contact) =>
        this[StepVectorMap.LambdaContactRotation(contact)];

    public double RadialDisplacement(ParticleContact contact) =>
        this[StepVectorMap.RadialDisplacement(contact)];

    public double AngleDisplacement(ParticleContact contact) =>
        this[StepVectorMap.AngleDisplacement(contact)];

    public double RotationDisplacement(ParticleContact contact) =>
        this[StepVectorMap.RotationDisplacement(contact)];

    public double LambdaDissipation(double value) => this[StepVectorMap.LambdaDissipation()] = value;

    public double NormalDisplacement(INode node, double value) => this[StepVectorMap.NormalDisplacement(node)] = value;

    public double FluxToUpper(INode node, double value) => this[StepVectorMap.FluxToUpper(node)] = value;

    public double LambdaVolume(INode node, double value) => this[StepVectorMap.LambdaVolume(node)] = value;

    public double TangentialDisplacement(INode node, double value) =>
        this[StepVectorMap.TangentialDisplacement(node)] = value;

    public double LambdaContactDistance(INode node, double value) =>
        this[StepVectorMap.LambdaContactDistance(node)] = value;

    public double LambdaContactDirection(INode node, double value) =>
        this[StepVectorMap.LambdaContactDirection(node)] = value;

    public double LambdaContactRotation(ParticleContact contact, double value) =>
        this[StepVectorMap.LambdaContactRotation(contact)] = value;

    public double RadialDisplacement(ParticleContact contact, double value) =>
        this[StepVectorMap.RadialDisplacement(contact)] = value;

    public double AngleDisplacement(ParticleContact contact, double value) =>
        this[StepVectorMap.AngleDisplacement(contact)] = value;

    public double RotationDisplacement(ParticleContact contact, double value) =>
        this[StepVectorMap.RotationDisplacement(contact)] = value;

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

    public double[] ParticleBlock(IParticle particle)
    {
        var block = StepVectorMap[particle];

        return Values[block.start..(block.start + block.length)];
    }

    public double[] BorderBlock() => Values[StepVectorMap.BorderStart..];

    public void UpdateParticleBlock(IParticle particle, double[] data)
    {
        var block = StepVectorMap[particle];

        if (data.Length != block.length)
            throw new InvalidOperationException(
                "'data' must have exactly the length of the particle block."
            );

        data.CopyTo(Values, block.start);
    }

    public void UpdateBorderBlock(double[] data)
    {
        data.CopyTo(Values, StepVectorMap.BorderStart);
    }
}