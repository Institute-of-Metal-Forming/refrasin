module RefraSin.TEPSolver.EquationSystem.Helper

open System

let CheckFiniteness value = if Double.IsFinite(value) then value else failwith "infinite value encountered"

let CheckFinitenesses values = values |> Seq.map CheckFiniteness
