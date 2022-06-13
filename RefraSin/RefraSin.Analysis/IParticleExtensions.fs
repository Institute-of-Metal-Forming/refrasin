namespace RefraSin.Analysis

open System.Runtime.CompilerServices
open RefraSin.Core.ParticleModel

[<Extension>]
module IParticleExtensions =

    [<Extension>]
    let MeanRadius (p: IParticle) : float =
        p.SurfaceNodes
        |> Seq.averageBy (fun n -> n.Coordinates.R)
