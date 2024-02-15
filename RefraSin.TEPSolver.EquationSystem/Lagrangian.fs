module RefraSin.TEPSolver.EquationSystem.Lagrangian

open RefraSin.ParticleModel
open RefraSin.ProcessModel
open RefraSin.TEPSolver
open RefraSin.TEPSolver.ParticleModel
open RefraSin.TEPSolver.StepVectors
open System.Linq

let StateVelocityDerivativeNormal
    (conditions: IProcessConditions)
    (currentEstimation: StepVector)
    (node: NodeBase)
    : float =

    let gibbsTerm = -node.GibbsEnergyGradient.Normal * (1.0 + currentEstimation.Lambda1)

    let requiredConstraintsTerm =
        node.VolumeGradient.Normal * currentEstimation[node].LambdaVolume

    let contactTerm =
        match node with
        | :? ContactNodeBase as cn ->
            cn.ContactDistanceGradient.Normal * currentEstimation[cn].LambdaContactDistance
            + cn.ContactDirectionGradient.Normal
              * currentEstimation[cn].LambdaContactDirection
        | _ -> 0.0

    gibbsTerm + contactTerm + requiredConstraintsTerm

let StateVelocityDerivativeTangential
    (conditions: IProcessConditions)
    (currentEstimation: StepVector)
    (node: ContactNodeBase)
    : float =

    let gibbsTerm = -node.GibbsEnergyGradient.Normal * (1.0 + currentEstimation.Lambda1)

    let requiredConstraintsTerm =
        node.VolumeGradient.Normal * currentEstimation[node].LambdaVolume

    let contactTerm =
        node.ContactDistanceGradient.Normal
        * currentEstimation[node].LambdaContactDistance
        + node.ContactDirectionGradient.Normal
          * currentEstimation[node].LambdaContactDirection

    gibbsTerm + contactTerm + requiredConstraintsTerm

let FluxDerivative (conditions: IProcessConditions) (currentEstimation: StepVector) (node: NodeBase) : float =
    let dissipationTerm =
        2.0 * conditions.GasConstant * conditions.Temperature
        / (node.Particle.Material.MolarVolume
           * node.Particle.Material.EquilibriumVacancyConcentration)
        * node.SurfaceDistance.ToUpper
        * currentEstimation[node].FluxToUpper
        / node.SurfaceDiffusionCoefficient.ToUpper
        * currentEstimation.Lambda1

    let thisRequiredConstraintsTerm = currentEstimation[node].LambdaVolume
    let upperRequiredConstraintsTerm = currentEstimation[node.Upper].LambdaVolume

    -dissipationTerm - thisRequiredConstraintsTerm + upperRequiredConstraintsTerm

let RequiredConstraint (conditions: IProcessConditions) (currentEstimation: StepVector) (node: NodeBase) : float =
    let normalVolumeTerm = node.VolumeGradient.Normal * currentEstimation[node].NormalDisplacement

    let tangentialVolumeTerm =
        match node with
        | :? ContactNodeBase as cn -> node.VolumeGradient.Tangential * currentEstimation[cn].TangentialDisplacement
        | _ -> 0.0

    let fluxTerm = currentEstimation[node].FluxToUpper - currentEstimation[node.Lower].FluxToUpper

    normalVolumeTerm + tangentialVolumeTerm - fluxTerm

let ContactNodeEquations
    (conditions: IProcessConditions)
    (currentEstimation: StepVector)
    (contact: ParticleContact)
    (involvedNodes: ContactNodeBase seq)
    : float seq =
    seq {
        for n in involvedNodes do
            yield StateVelocityDerivativeTangential conditions currentEstimation n
            yield StateVelocityDerivativeTangential conditions currentEstimation n.ContactedNode

            yield! ContactConstraints conditions currentEstimation contact n

            yield
                currentEstimation[n].LambdaContactDistance
                - currentEstimation[n.ContactedNode].LambdaContactDistance

            yield
                currentEstimation[n].LambdaContactDirection
                - currentEstimation[n.ContactedNode].LambdaContactDirection
    }

let NodeEquations
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float seq =

    seq {
        for n in currentState.Nodes do
            yield StateVelocityDerivativeNormal conditions currentEstimation n
            yield FluxDerivative conditions currentEstimation n
            yield RequiredConstraint conditions currentEstimation n
    }

let ContactEquations
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float seq =

    seq {
        for c in currentState.Contacts do
            let involvedNodes =
                query {
                    for n in c.From.Nodes.OfType<ContactNodeBase>() do
                        where (n.ContactedParticleId = c.To.Id)
                        select n
                }

            yield! ContactNodeEquations conditions currentEstimation c involvedNodes

            yield ContactDistanceDerivative conditions currentEstimation c involvedNodes
            yield ContactDistanceDerivative conditions currentEstimation c involvedNodes
            yield ContactDistanceDerivative conditions currentEstimation c involvedNodes
    }

let DissipationEquality
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float =

    let dissipationNormal =
        query {
            for n in currentState.Nodes do
                sumBy (-n.GibbsEnergyGradient.Normal * currentEstimation[n].NormalDisplacement)
        }

    let dissipationTangential =
        query {
            for n in currentState.Nodes.OfType<ContactNodeBase>() do
                sumBy (-n.GibbsEnergyGradient.Tangential * currentEstimation[n].TangentialDisplacement)
        }

    let dissipationFunction =
        query {
            for p in currentState.Particles do
                sumBy (
                    query {
                        for n in p.Nodes do
                            sumBy (
                                n.SurfaceDistance.ToUpper * currentEstimation[n].FluxToUpper ** 2
                                / n.SurfaceDiffusionCoefficient.ToUpper
                            )
                    }
                    / (p.Material.MolarVolume * p.Material.EquilibriumVacancyConcentration)
                )
        }
        * conditions.GasConstant
        * conditions.Temperature

    dissipationNormal + dissipationTangential - dissipationFunction

let YieldEquations
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float seq =
    seq {
        yield! NodeEquations conditions currentState currentEstimation
        yield! ContactEquations conditions currentState currentEstimation
        yield DissipationEquality conditions currentState currentEstimation
    }
