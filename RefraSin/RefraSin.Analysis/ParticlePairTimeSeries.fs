namespace RefraSin.Analysis

open System.Runtime.CompilerServices
open RefraSin.Core.ParticleModel
open ValueTimeSeries

type ParticlePairTimeSeries = ((double * (IParticle * IParticle)) seq)

[<Extension>]
module ParticlePairTimeSeries =
    let Times (timeSeries: ValueTimeSeries) : float seq = timeSeries |> Seq.map fst

    let ParticlePairs (timeSeries: ParticlePairTimeSeries) : (IParticle * IParticle) seq = timeSeries |> Seq.map snd

    let ParticleDistanceSeries (timeSeries: ParticlePairTimeSeries) : ValueTimeSeries =
        let distance (p1: IParticle, p2: IParticle) =
            p1.CenterCoordinates.Absolute.DistanceTo(p2.CenterCoordinates.Absolute)

        timeSeries
        |> Seq.map (fun (t, ps) -> (t, ps |> distance))

    let ShrinkageSeries (timeSeries: ParticlePairTimeSeries) : ValueTimeSeries =
        let distances = timeSeries |> ParticleDistanceSeries
        let initDistance = distances |> Values |> Seq.head
        let shrinkage d = 1.0 - d / initDistance

        distances
        |> Seq.map (fun (t, d) -> (t, d |> shrinkage))

[<Extension>]
module ParticlePairTimeSeriesExtensions =
    [<Extension>]
    let Times (timeSeries: ValueTimeSeries) : float seq = ParticlePairTimeSeries.Times timeSeries

    [<Extension>]
    let ParticlePairs (timeSeries: ParticlePairTimeSeries) : (IParticle * IParticle) seq =
        ParticlePairTimeSeries.ParticlePairs timeSeries

    [<Extension>]
    let ParticleDistanceSeries (timeSeries: ParticlePairTimeSeries) : ValueTimeSeries =
        ParticlePairTimeSeries.ParticleDistanceSeries timeSeries

    [<Extension>]
    let ShrinkageSeries (timeSeries: ParticlePairTimeSeries) : ValueTimeSeries =
        ParticlePairTimeSeries.ShrinkageSeries timeSeries
