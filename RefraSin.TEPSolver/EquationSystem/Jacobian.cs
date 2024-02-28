using MathNet.Numerics.LinearAlgebra;
using RefraSin.Coordinates;
using RefraSin.ParticleModel;
using RefraSin.ProcessModel;
using RefraSin.TEPSolver.ParticleModel;
using RefraSin.TEPSolver.StepVectors;
using static System.Math;
using static RefraSin.Coordinates.Constants;
using static RefraSin.TEPSolver.EquationSystem.Helper;
using Particle = RefraSin.TEPSolver.ParticleModel.Particle;
using ParticleContact = RefraSin.TEPSolver.ParticleModel.ParticleContact;

namespace RefraSin.TEPSolver.EquationSystem;

public static class Jacobian
{
    public static Matrix<double> BorderBlock(
        IProcessConditions conditions,
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

    private static IEnumerable<IEnumerable<(int colIndex, double value)>> YieldBorderBlockRows(
        IProcessConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    ) =>
        Join(
            YieldContactsEquations(conditions, currentState.Contacts, stepVector),
            YieldGlobalEquations(conditions, currentState, stepVector)
        );

    public static IEnumerable<IEnumerable<(int colIndex, double value)>> YieldContactsEquations(
        IProcessConditions conditions,
        IEnumerable<ParticleContact> contacts,
        StepVector stepVector
    ) =>
        contacts.SelectMany(contact =>
        {
            return Join(
                YieldContactNodesEquations(conditions, stepVector, contact),
                YieldContactAuxiliaryDerivatives(stepVector, contact)
            );
        });

    private static IEnumerable<(int colIndex, double value)> ParticleRadialDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Select(node =>
            (stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDistance], 1.0)
        );

    private static IEnumerable<(int colIndex, double value)> ParticleAngleDisplacementDerivative(
        StepVector stepVector,
        ParticleContact contact
    ) =>
        contact.FromNodes.Select(node =>
            (stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDirection], 1.0)
        );

    private static IEnumerable<(int colIndex, double value)> ParticleRotationDerivative(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        return Components();

        IEnumerable<(int, double)> Components()
        {
            foreach (var node in contact.FromNodes)
            {
                var angleDifference = (
                    node.ContactedNode.Coordinates.Phi - node.ContactedNode.ContactDirection
                ).Reduce(Angle.ReductionDomain.WithNegative);
                yield return (
                    stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDistance],
                    -node.ContactedNode.Coordinates.R
                  * Sin(stepVector.RotationDisplacement(contact) + angleDifference)
                );
                yield return (
                    stepVector.StepVectorMap[node, NodeUnknown.LambdaContactDirection],
                    node.ContactedNode.Coordinates.R
                  / contact.Distance
                  * Cos(stepVector.RotationDisplacement(contact) + angleDifference)
                );
            }

            yield return (
                stepVector.StepVectorMap[contact, ContactUnknown.RotationDisplacement],
                contact.FromNodes.Sum(node =>
                {
                    var angleDifference = (
                        node.ContactedNode.Coordinates.Phi - node.ContactedNode.ContactDirection
                    ).Reduce(Angle.ReductionDomain.WithNegative);
                    return -node.ContactedNode.Coordinates.R
                         * Cos(stepVector.RotationDisplacement(contact) + angleDifference)
                         - node.ContactedNode.Coordinates.R
                         / contact.Distance
                         * Sin(stepVector.RotationDisplacement(contact) + angleDifference);
                })
            );
        }
    }

    public static IEnumerable<IEnumerable<(int colIndex, double value)>> YieldGlobalEquations(
        IProcessConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        yield return DissipationEquality(conditions, currentState, stepVector);
    }

    private static IEnumerable<(int colIndex, double value)> StateVelocityDerivativeTangential(
        IProcessConditions conditions,
        StepVector stepVector,
        ContactNodeBase node
    )
    {
        return Components();

        IEnumerable<(int, double)> Components()
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
    }

    private static IEnumerable<(int colIndex, double value)> DissipationEquality(
        IProcessConditions conditions,
        SolutionState currentState,
        StepVector stepVector
    )
    {
        return Components();

        IEnumerable<(int, double)> Components()
        {
            foreach (var node in currentState.Nodes.OfType<ContactNodeBase>())
            {
                yield return (
                    stepVector.StepVectorMap[node, NodeUnknown.TangentialDisplacement],
                    -node.GibbsEnergyGradient.Tangential
                );
            }
        }
    }

    private static (
        IEnumerable<(int colIndex, double value)> distance,
        IEnumerable<(int colIndex, double value)> direction
        ) ContactConstraints(
            IProcessConditions conditions,
            StepVector stepVector,
            IParticleContact contact,
            ContactNodeBase node
        )
    {
        var angleDifference = (
            node.ContactedNode.Coordinates.Phi - node.ContactedNode.ContactDirection
        ).Reduce(Angle.ReductionDomain.WithNegative);

        IEnumerable<(int, double)> DistanceComponents()
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.TangentialDisplacement],
                -node.ContactDistanceGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap[node.ContactedNode, NodeUnknown.TangentialDisplacement],
                -node.ContactDistanceGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap[contact, ContactUnknown.RadialDisplacement],
                1.0
            );
            yield return (
                stepVector.StepVectorMap[contact, ContactUnknown.RotationDisplacement],
                -node.ContactedNode.Coordinates.R * Sin(angleDifference)
            );
        }

        IEnumerable<(int, double)> DirectionComponents()
        {
            yield return (
                stepVector.StepVectorMap[node, NodeUnknown.TangentialDisplacement],
                -node.ContactDirectionGradient.Tangential
            );
            yield return (
                stepVector.StepVectorMap[node.ContactedNode, NodeUnknown.TangentialDisplacement],
                -node.ContactDirectionGradient.Tangential
            );
            yield return (stepVector.StepVectorMap[contact, ContactUnknown.AngleDisplacement], 1.0);
            yield return (
                stepVector.StepVectorMap[contact, ContactUnknown.RotationDisplacement],
                -node.ContactedNode.Coordinates.R / node.ContactDistance * Cos(angleDifference)
            );
        }

        return (DistanceComponents(), DirectionComponents());
    }

    private static IEnumerable<
        IEnumerable<(int colIndex, double value)>
    > YieldContactNodesEquations(
        IProcessConditions conditions,
        StepVector stepVector,
        ParticleContact contact
    )
    {
        foreach (var contactNode in contact.FromNodes)
        {
            yield return StateVelocityDerivativeTangential(conditions, stepVector, contactNode);
            yield return StateVelocityDerivativeTangential(
                conditions,
                stepVector,
                contactNode.ContactedNode
            );

            var constraints = ContactConstraints(conditions, stepVector, contact, contactNode);
            yield return constraints.distance;
            yield return constraints.direction;

            // lambdas of contact must be equal for both connected nodes
            yield return new[]
            {
                (stepVector.StepVectorMap[contactNode, NodeUnknown.LambdaContactDistance], 1.0),
                (stepVector.StepVectorMap[contactNode.ContactedNode, NodeUnknown.LambdaContactDistance], -1.0)
            };
            yield return new[]
            {
                (stepVector.StepVectorMap[contactNode, NodeUnknown.LambdaContactDirection], 1.0),
                (stepVector.StepVectorMap[contactNode.ContactedNode, NodeUnknown.LambdaContactDirection], -1.0)
            };
        }
    }

    private static IEnumerable<
        IEnumerable<(int colIndex, double value)>
    > YieldContactAuxiliaryDerivatives(
        StepVector stepVector,
        ParticleContact contact
    )
    {
        yield return ParticleRadialDisplacementDerivative(stepVector, contact);
        yield return ParticleAngleDisplacementDerivative(stepVector, contact);
        yield return ParticleRotationDerivative(stepVector, contact);
    }

    public static Matrix<double> ParticleBlock(
        IProcessConditions conditions,
        Particle particle,
        StepVector stepVector
    )
    {
        var rows = YieldParticleBlockEquations(conditions, particle, stepVector).Select(r => r.ToArray()).ToArray();
        var (startIndex, size) = stepVector.StepVectorMap[particle];
        return Matrix<double>.Build.SparseOfIndexed(
            size,
            size,
            rows.SelectMany((r, i) => r.Select(c => (i, c.colIndex - startIndex, c.value)))
        );
    }

    private static IEnumerable<
        IEnumerable<(int colIndex, double value)>
    > YieldParticleBlockEquations(
        IProcessConditions conditions,
        Particle particle,
        StepVector stepVector
    ) => YieldNodeEquations(conditions, particle.Nodes, stepVector);

    private static IEnumerable<IEnumerable<(int colIndex, double value)>> YieldNodeEquations(
        IProcessConditions conditions,
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

    private static IEnumerable<(int colIndex, double value)> StateVelocityDerivativeNormal(
        IProcessConditions conditions,
        StepVector stepVector,
        NodeBase node
    ) =>
        new[]
        {
            (stepVector.StepVectorMap[node, NodeUnknown.LambdaVolume], node.VolumeGradient.Normal),
        };

    private static IEnumerable<(int colIndex, double value)> FluxDerivative(
        IProcessConditions conditions,
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

        return new[]
        {
            (stepVector.StepVectorMap[node, NodeUnknown.FluxToUpper], fluxToUpper),
            (stepVector.StepVectorMap[node, NodeUnknown.LambdaVolume], -1),
            (stepVector.StepVectorMap[node.Upper, NodeUnknown.LambdaVolume], 1),
        };
    }

    private static IEnumerable<(int colIndex, double value)> RequiredConstraint(
        IProcessConditions conditions,
        StepVector stepVector,
        NodeBase node
    ) =>
        new[]
        {
            (
                stepVector.StepVectorMap[node, NodeUnknown.NormalDisplacement],
                node.VolumeGradient.Normal
            ),
            (stepVector.StepVectorMap[node, NodeUnknown.FluxToUpper], -1),
            (stepVector.StepVectorMap[node.Lower, NodeUnknown.FluxToUpper], 1),
        };
}