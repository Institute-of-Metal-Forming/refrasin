module RefraSin.TEPSolver.EquationSystem.Lagrangian

open RefraSin.ProcessModel
open RefraSin.TEPSolver
open RefraSin.TEPSolver.StepVectors

let DissipationEquality
    (conditions: IProcessConditions)
    (currentState: SolutionState)
    (currentEstimation: StepVector)
    : float =
    let dissipation =
        currentState.Nodes
        |> Seq.map (fun n -> -n.GibbsEnergyGradient.Normal * currentEstimation[n].NormalDisplacement)
        |> Seq.sum

    let dissipationFunction =
        (currentState.Nodes
         |> Seq.map (fun n ->
             (n.SurfaceDistance.ToUpper * currentEstimation[n].FluxToUpper ** 2
              / n.SurfaceDiffusionCoefficient.ToUpper
              + n.SurfaceDistance.ToLower * currentEstimation[n.Lower].FluxToUpper ** 2
                / n.SurfaceDiffusionCoefficient.ToLower)
             / (n.Particle.Material.MolarVolume
                * n.Particle.Material.EquilibriumVacancyConcentration))
         |> Seq.sum)
        * conditions.GasConstant
        * conditions.Temperature
        / 2.0

    dissipation - dissipationFunction

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
