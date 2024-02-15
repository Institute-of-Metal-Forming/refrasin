namespace RefraSin.TEPSolver.EquationSystem

open RefraSin.TEPSolver.ParticleModel
open RefraSin.TEPSolver.StepVectors
open System.Linq

type StepEstimator() =
    interface IStepEstimator with
        member this.EstimateStep(conditions, currentState) =
            let guess =
                seq {
                    // Node Unknowns
                    for n in currentState.Nodes do
                        yield n.GuessNormalDisplacement() // NormalDisplacement
                        yield n.GuessFluxToUpper() // FluxToUpper
                        yield 1.0 // LambdaVolume

                    // Contact Node Unknowns
                    for n in currentState.Nodes.OfType<ContactNodeBase>() do
                        yield 0.0 // TangentialDisplacement
                        yield 1.0 // LambdaContactDistance
                        yield 1.0 // LambdaContactDirection

                    // Contact Unknowns
                    for c in currentState.Contacts do
                        yield 0.0 // RadialDisplacement
                        yield 0.0 // AngleDisplacement
                        yield 0.0 // RotationDisplacement

                    // Global Unknowns
                    yield 1.0 // Lambda1
                }
                |> Array.ofSeq

            StepVector(guess, StepVectorMap(currentState))
