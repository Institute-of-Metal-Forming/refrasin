namespace RefraSin.Analysis

open System.Runtime.CompilerServices
open IParticleExtensions
open RefraSin.Core.ParticleModel

type ParticleTimeSeries = ((double * IParticle) seq)

module ParticleTimeSeries =
    let Times (timeSeries: ParticleTimeSeries) : float seq = timeSeries |> Seq.map fst

    let Particles (timeSeries: ParticleTimeSeries) : IParticle seq = timeSeries |> Seq.map snd

    let RelativeNeckRadiusSeries (timeSeries: ParticleTimeSeries) : ValuesTimeSeries =
        let relativeNeckRadii p =
            let meanRadius = MeanRadius p

            p.Necks
            |> Seq.map (fun n -> n.Radius / meanRadius)

        timeSeries
        |> Seq.map (fun (t, p) -> (t, p |> relativeNeckRadii))


[<Extension>]
module ParticleTimeSeriesExtensions =
    [<Extension>]
    let Times (timeSeries: ParticleTimeSeries) : float seq = ParticleTimeSeries.Times timeSeries

    [<Extension>]
    let Particles (timeSeries: ParticleTimeSeries) : IParticle seq = ParticleTimeSeries.Particles timeSeries

    [<Extension>]
    let RelativeNeckRadiusSeries (timeSeries: ParticleTimeSeries) : ValuesTimeSeries =
        ParticleTimeSeries.RelativeNeckRadiusSeries timeSeries
