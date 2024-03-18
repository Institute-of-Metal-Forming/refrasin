using MathNet.Numerics.LinearAlgebra;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.ProcessModel.Sintering;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static RefraSin.TEPSolver.EquationSystem.Helper;
using NeckNode = RefraSin.TEPSolver.ParticleModel.NeckNode;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;
using JacobianRow = System.Collections.Generic.IEnumerable<(int colIndex, double value)>;
using JacobianRows = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<(int colIndex, double value)>>;

namespace RefraSin.TEPSolver.EquationSystem;

public static class Jacobian
{
    public static Matrix<double> BorderBlock(
        ISinteringConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        var rows = YieldBorderBlockRows(conditions, currentState, stepVector).ToArray();
        var startIndex = stepVector.StepVectorMap.BorderStart;
        var size = stepVector.StepVectorMap.BorderLength;
        return Matrix<double>.Build.SparseOfIndexed(
            size,
            size,
            rows.SelectMany((r, i) => r.Select(c => (i, c.colIndex - startIndex, c.value)))
        );
    }

    private static JacobianRows YieldBorderBlockRows(
        ISinteringConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    ) =>
        Join(
            YieldContactsEquations(conditions, currentState.Contacts, stepVector),
            YieldGlobalEquations(conditions, currentState, stepVector)
        );

    public static JacobianRows YieldContactsEquations(
        ISinteringConditions conditions,
        IEnumerable<ParticleContact> contacts,
        StepVector stepVector
    ) =>
        contacts.SelectMany(contact =>
            Join(
                YieldContactNodesEquations(conditions, stepVector, contact),
                YieldContactAuxiliaryDerivatives(stepVector, contact)
            )
        );

    private static JacobianRow ParticleRadialDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Select(node =>
            (stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDistance], 1.0)
        );

    private static JacobianRow ParticleAngleDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Select(node =>
            (stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDirection], 1.0)
        );

    private static JacobianRow ParticleRotationDerivative(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        foreach (var node in contact.FromNodes)
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDistance],
                node.ContactedNode.Coordinates.R
                    * Sin(node.ContactedNode.AngleDistanceFromContactDirection)
            );
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDirection],
                -node.ContactedNode.Coordinates.R
                    / contact.Distance
                    * Cos(node.ContactedNode.AngleDistanceFromContactDirection)
            );
        }
    }

    public static JacobianRows YieldGlobalEquations(
        ISinteringConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        yield return DissipationEquality(conditions, currentState, stepVector);
    }

    private static JacobianRow StateVelocityDerivativeTangential(
        ISinteringConditions conditions,
        StepVector stepVector,
        ContactNodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap[GlobalUnknown.Lambda1],
            -node.GibbsEnergyGradient.Tangential
        );
        yield return (
            stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDistance],
            node.ContactDistanceGradient.Tangential
        );
        yield return (
            stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDirection],
            node.ContactDirectionGradient.Tangential
        );
    }

    private static JacobianRow DissipationEquality(
        ISinteringConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    ) =>
        currentState
            .Nodes.OfType<NeckNode>()
            .Select(node =>
                (
                    stepVector.StepVectorMap[node, NodeUnknown.TangentialDisplacement],
                    -node.GibbsEnergyGradient.Tangential
                )
            );

    private static JacobianRow ContactConstraintDistance(
        ISinteringConditions conditions,
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    )
    {
        if (node is NeckNode)
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.TangentialDisplacement],
                -node.ContactDistanceGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap[node.ContactedNode, NodeUnknown.TangentialDisplacement],
                -node.ContactDistanceGradient.Tangential
            );
        }

        yield return (stepVector.StepVectorMap[contact, ContactUnknown.RadialDisplacement], 1.0);
        yield return (
            stepVector.StepVectorMap[contact, ContactUnknown.RotationDisplacement],
            node.ContactedNode.Coordinates.R
                * Sin(node.ContactedNode.AngleDistanceFromContactDirection)
        );
    }

    private static JacobianRow ContactConstraintDirection(
        ISinteringConditions conditions,
        StepVector stepVector,
        IParticleContact contact,
        ContactNodeBase node
    )
    {
        if (node is NeckNode)
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.TangentialDisplacement],
                -node.ContactDirectionGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap[node.ContactedNode, NodeUnknown.TangentialDisplacement],
                -node.ContactDirectionGradient.Tangential
            );
        }

        yield return (stepVector.StepVectorMap[contact, ContactUnknown.AngleDisplacement], 1.0);
        yield return (
            stepVector.StepVectorMap[contact, ContactUnknown.RotationDisplacement],
            -node.ContactedNode.Coordinates.R
                / contact.Distance
                * Cos(node.ContactedNode.AngleDistanceFromContactDirection)
        );
    }

    private static JacobianRows YieldContactNodesEquations(
        ISinteringConditions conditions,
        StepVector stepVector,
        ParticleContact contact
    )
    {
        foreach (var contactNode in contact.FromNodes)
        {
            if (contactNode is NeckNode)
            {
                yield return StateVelocityDerivativeTangential(conditions, stepVector, contactNode);
                yield return StateVelocityDerivativeTangential(
                    conditions,
                    stepVector,
                    contactNode.ContactedNode
                );
            }

            yield return ContactConstraintDistance(conditions, stepVector, contact, contactNode);
            yield return ContactConstraintDirection(conditions, stepVector, contact, contactNode);
        }
    }

    private static JacobianRows YieldContactAuxiliaryDerivatives(StepVector stepVector, ParticleContact contact)
    {
        yield return ParticleRadialDisplacementDerivative(stepVector, contact);
        yield return ParticleAngleDisplacementDerivative(stepVector, contact);
        yield return ParticleRotationDerivative(stepVector, contact);
    }

    public static Matrix<double> ParticleBlock(
        ISinteringConditions conditions,
        Particle particle,
        StepVector stepVector
    )
    {
        var rows = YieldParticleBlockEquations(conditions, particle, stepVector)
            .Select(r => r.ToArray())
            .ToArray();
        var (startIndex, size) = stepVector.StepVectorMap[particle];
        return Matrix<double>.Build.SparseOfIndexed(
            size,
            size,
            rows.SelectMany((r, i) => r.Select(c => (i, c.colIndex - startIndex, c.value)))
        );
    }

    private static JacobianRows YieldParticleBlockEquations(
        ISinteringConditions conditions,
        Particle particle,
        StepVector stepVector
    ) => YieldNodeEquations(conditions, particle.Nodes, stepVector);

    private static JacobianRows YieldNodeEquations(
        ISinteringConditions conditions,
        IEnumerable<NodeBase> nodes,
        StepVector stepVector
    )
    {
        foreach (var node in nodes)
        {
            yield return StateVelocityDerivativeNormal(conditions, stepVector, node);
            yield return FluxDerivative(conditions, stepVector, node);
            yield return RequiredConstraint(conditions, stepVector, node);
        }
    }

    private static JacobianRow StateVelocityDerivativeNormal(
        ISinteringConditions conditions,
        StepVector stepVector,
        NodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap[node, NodeUnknown.LambdaVolume],
            node.VolumeGradient.Normal
        );
    }

    private static JacobianRow FluxDerivative(
        ISinteringConditions conditions,
        StepVector stepVector,
        NodeBase node
    )
    {
        var fluxToUpper =
            -2
            * conditions.GasConstant
            * conditions.Temperature
            / (
                node.Particle.Material.MolarVolume
                * node.Particle.Material.EquilibriumVacancyConcentration
            )
            * node.SurfaceDistance.ToUpper
            / node.SurfaceDiffusionCoefficient.ToUpper
            * stepVector.Lambda1;

        yield return (stepVector.StepVectorMap[node, NodeUnknown.FluxToUpper], fluxToUpper);
        yield return (stepVector.StepVectorMap[node, NodeUnknown.LambdaVolume], -1);
        yield return (stepVector.StepVectorMap[node.Upper, NodeUnknown.LambdaVolume], 1);
    }

    private static JacobianRow RequiredConstraint(
        ISinteringConditions conditions,
        StepVector stepVector,
        NodeBase node
    )
    {
        yield return (
            stepVector.StepVectorMap[node, NodeUnknown.NormalDisplacement],
            node.VolumeGradient.Normal
        );
        yield return (stepVector.StepVectorMap[node, NodeUnknown.FluxToUpper], -1);
        yield return (stepVector.StepVectorMap[node.Lower, NodeUnknown.FluxToUpper], 1);
    }
}
