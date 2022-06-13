namespace RefraSin.Analysis

open System
open System.Runtime.CompilerServices
open MathNet.Numerics
open MathNet.Numerics.Interpolation
open MathNet.Numerics.Statistics

module IInterpolationSeq =
    let aggregatedSeries raster aggregationFunction (interpolations: IInterpolation seq) : ValueTimeSeries =
        let aggregate t =
            interpolations
            |> Seq.map (fun i -> i.Interpolate(t))
            |> aggregationFunction

        let values = raster |> Seq.map aggregate
        Seq.zip raster values

    let MeanSeries (raster: float seq) (interpolations: IInterpolation seq) : ValueTimeSeries =
        interpolations
        |> aggregatedSeries raster Seq.average

    let MedianSeries (raster: float seq) (interpolations: IInterpolation seq) : ValueTimeSeries =
        interpolations
        |> aggregatedSeries raster Statistics.Median

    let StandardDeviationSeries (raster: float seq) (interpolations: IInterpolation seq) : ValueTimeSeries =
        interpolations
        |> aggregatedSeries raster Statistics.StandardDeviation

    let MinSeries (raster: float seq) (interpolations: IInterpolation seq) : ValueTimeSeries =
        interpolations |> aggregatedSeries raster Seq.min

    let MaxSeries (raster: float seq) (interpolations: IInterpolation seq) : ValueTimeSeries =
        interpolations |> aggregatedSeries raster Seq.max

    let LowerQuartileSeries (raster: float seq) (interpolations: IInterpolation seq) : ValueTimeSeries =
        interpolations
        |> aggregatedSeries raster Statistics.LowerQuartile

    let UpperQuartileSeries (raster: float seq) (interpolations: IInterpolation seq) : ValueTimeSeries =
        interpolations
        |> aggregatedSeries raster Statistics.UpperQuartile

    let SkewnessSeries (raster: float seq) (interpolations: IInterpolation seq) : ValueTimeSeries =
        interpolations
        |> aggregatedSeries raster Statistics.Skewness

    let AggregatedSeries
        (aggregationFunction: double seq -> double)
        (raster: float seq)
        (interpolations: IInterpolation seq)
        : ValueTimeSeries =
        interpolations
        |> aggregatedSeries raster aggregationFunction

[<Extension>]
module IInterpolationSeqExtensions =

    [<Extension>]
    let MeanSeries (interpolations: IInterpolation seq, raster: float seq) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.MeanSeries raster

    [<Extension>]
    let MedianSeries (interpolations: IInterpolation seq, raster: float seq) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.MedianSeries raster

    [<Extension>]
    let StandardDeviationSeries (interpolations: IInterpolation seq, raster: float seq) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.StandardDeviationSeries raster

    [<Extension>]
    let MinSeries (interpolations: IInterpolation seq, raster: float seq) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.MinSeries raster

    [<Extension>]
    let MaxSeries (interpolations: IInterpolation seq, raster: float seq) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.MaxSeries raster

    [<Extension>]
    let LowerQuartileSeries (interpolations: IInterpolation seq, raster: float seq) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.LowerQuartileSeries raster

    [<Extension>]
    let UpperQuartileSeries (interpolations: IInterpolation seq, raster: float seq) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.UpperQuartileSeries raster

    [<Extension>]
    let SkewnessSeries (interpolations: IInterpolation seq, raster: float seq) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.SkewnessSeries raster

    [<Extension>]
    let AggregatedSeries
        (
            interpolations: IInterpolation seq,
            aggregationFunction: Func<double seq, double>,
            raster: float seq
        ) : ValueTimeSeries =
        interpolations
        |> IInterpolationSeq.AggregatedSeries aggregationFunction.Invoke raster
