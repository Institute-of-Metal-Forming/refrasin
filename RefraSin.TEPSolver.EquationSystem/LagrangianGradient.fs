namespace RefraSin.TEPSolver.EquationSystem

open RefraSin.TEPSolver.StepVectors
open MathNet.Numerics.LinearAlgebra

type LagrangianGradient() =
    interface ILagrangianGradient with
        member this.EvaluateAt(conditions, currentState, currentEstimation) =
            let evaluation =
                Lagrangian.FullEquationSet conditions currentState currentEstimation
                |> Helper.CheckFinitenesses
                |> Array.ofSeq
                

            StepVector(evaluation, currentEstimation.StepVectorMap)
