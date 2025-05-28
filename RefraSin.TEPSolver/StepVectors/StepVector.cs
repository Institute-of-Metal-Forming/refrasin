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

    public double ItemValue<TItem>()
        where TItem : IGlobalItem => this[StepVectorMap.ItemIndex<TItem>()];

    public double ItemValue<TItem>(Particle particle)
        where TItem : IParticleItem => this[StepVectorMap.ItemIndex<TItem>(particle)];

    public double ItemValue<TItem>(NodeBase node)
        where TItem : INodeItem => this[StepVectorMap.ItemIndex<TItem>(node)];

    public double ItemValue<TItem>(Pore pore)
        where TItem : IPoreItem => this[StepVectorMap.ItemIndex<TItem>(pore)];

    public double ItemValue<TItem>(ContactPair<NodeBase> nodeContact)
        where TItem : INodeContactItem => this[StepVectorMap.ItemIndex<TItem>(nodeContact)];

    public double ItemValue<TItem>(ContactPair<Particle> particleContact)
        where TItem : IParticleContactItem => this[StepVectorMap.ItemIndex<TItem>(particleContact)];

    public double ItemValue(ISystemItem quantity) => this[StepVectorMap.ItemIndex(quantity)];

    public void SetItemValue<TItem>(double value)
        where TItem : IGlobalItem => this[StepVectorMap.ItemIndex<TItem>()] = value;

    public void SetItemValue<TItem>(Particle particle, double value)
        where TItem : IParticleItem => this[StepVectorMap.ItemIndex<TItem>(particle)] = value;

    public void SetItemValue<TItem>(NodeBase node, double value)
        where TItem : INodeItem => this[StepVectorMap.ItemIndex<TItem>(node)] = value;

    public void SetItemValue<TItem>(ContactPair<NodeBase> nodeContact, double value)
        where TItem : INodeContactItem => this[StepVectorMap.ItemIndex<TItem>(nodeContact)] = value;

    public void SetItemValue<TItem>(ContactPair<Particle> particleContact, double value)
        where TItem : IParticleContactItem =>
        this[StepVectorMap.ItemIndex<TItem>(particleContact)] = value;

    public void SetItemValue(ISystemItem quantity, double value) =>
        this[StepVectorMap.ItemIndex(quantity)] = value;

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
