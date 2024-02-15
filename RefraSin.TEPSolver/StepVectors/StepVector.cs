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

    public double Lambda1 => this[StepVectorMap[GlobalUnknown.Lambda1]];

    public double NormalDisplacement(INode node) => this[StepVectorMap[node.Id, NodeUnknown.NormalDisplacement]];

    public double FluxToUpper(INode node) => this[StepVectorMap[node.Id, NodeUnknown.FluxToUpper]];

    public double LambdaVolume(INode node) => this[StepVectorMap[node.Id, NodeUnknown.LambdaVolume]];

    public double TangentialDisplacement(IContactNode node) => this[StepVectorMap[node.Id, NodeUnknown.TangentialDisplacement]];

    public double LambdaContactDistance(IContactNode node) => this[StepVectorMap[node.Id, NodeUnknown.LambdaContactDistance]];

    public double LambdaContactDirection(IContactNode node) => this[StepVectorMap[node.Id, NodeUnknown.LambdaContactDirection]];

    public double RadialDisplacement(IParticleContact contact) =>
        this[StepVectorMap[contact.From.Id, contact.To.Id, ContactUnknown.RadialDisplacement]];

    public double RadialDisplacement(IParticle from, IParticle to) =>
        this[StepVectorMap[from.Id, to.Id, ContactUnknown.RadialDisplacement]];

    public double AngleDisplacement(IParticleContact contact) =>
        this[StepVectorMap[contact.From.Id, contact.To.Id, ContactUnknown.AngleDisplacement]];

    public double AngleDisplacement(IParticle from, IParticle to) =>
        this[StepVectorMap[from.Id, to.Id, ContactUnknown.AngleDisplacement]];

    public double RotationDisplacement(IParticleContact contact) =>
        this[StepVectorMap[contact.From.Id, contact.To.Id, ContactUnknown.RotationDisplacement]];

    public double RotationDisplacement(IParticle from, IParticle to) =>
        this[StepVectorMap[from.Id, to.Id, ContactUnknown.RotationDisplacement]];

    public static StepVector operator +(StepVector leftSide, StepVector rightSide) => new((DenseVector)leftSide + rightSide, leftSide.StepVectorMap);
    public static StepVector operator -(StepVector leftSide, StepVector rightSide) => new((DenseVector)leftSide - rightSide, leftSide.StepVectorMap);
    public static StepVector operator *(StepVector leftSide, double rightSide) => new((DenseVector)leftSide * rightSide, leftSide.StepVectorMap);
    public static StepVector operator *(double leftSide, StepVector rightSide) => rightSide * leftSide;
    public static StepVector operator /(StepVector leftSide, double rightSide) => new((DenseVector)leftSide / rightSide, leftSide.StepVectorMap);

    public StepVector Copy() => new(Build.DenseOfVector(this), StepVectorMap);
}