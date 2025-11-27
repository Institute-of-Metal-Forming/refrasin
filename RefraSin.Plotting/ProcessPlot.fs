module RefraSin.Plotting.ProcessPlot

open Plotly.NET
open Plotly.NET.StyleParam
open RefraSin.Coordinates.Absolute
open RefraSin.ParticleModel.Particles
open RefraSin.ProcessModel
open RefraSin.Analysis

let PlotTimeSteps (states: ISystemState<_, _> seq) =
    let times = [ for s in states -> s.Time ]
    let indices = [ 1 .. times.Length + 1 ]
    let timeSteps = List.mapi (fun i t1 -> t1 - times[i]) times.Tail

    Chart.Line(x = indices, y = timeSteps)
    |> Commons.ApplyDefaultPlotProperties
    |> Chart.withXAxisStyle (TitleText = "# of Step", AxisType = AxisType.Category)
    |> Chart.withYAxisStyle (TitleText = "Time Step Width", AxisType = AxisType.Log)

let PlotShrinkagesByVolume (states: ISystemState<_, _> seq) =
    let times = [ for s in states -> s.Time ]

    let shrinkages =
        states
        |> SinteringAnalysis.ShrinkagesByVolume
        |> Seq.map (fun s -> s * 100.0)
        |> Seq.toList

    Chart.Line(x = times, y = shrinkages)
    |> Commons.ApplyDefaultPlotProperties
    |> Chart.withXAxisStyle (TitleText = "Time", AxisType = AxisType.Log)
    |> Chart.withYAxisStyle (TitleText = "Shrinkage in %", AxisType = AxisType.Log)

let PlotShrinkagesByDistance (states: ISystemState<_, _> seq) =
    let times = [ for s in states -> s.Time ]

    let shrinkages =
        states
        |> SinteringAnalysis.ShrinkagesByDistance
        |> Seq.map (fun s -> s * 100.0)
        |> Seq.toList

    Chart.Line(x = times, y = shrinkages)
    |> Commons.ApplyDefaultPlotProperties
    |> Chart.withXAxisStyle (TitleText = "Time", AxisType = AxisType.Log)
    |> Chart.withYAxisStyle (TitleText = "Shrinkage in %", AxisType = AxisType.Log)

let PlotNeckWidths (states: ISystemState<_, _> seq) =
    let times = [ for s in states -> s.Time ]
    let necks = states |> SinteringAnalysis.NeckWidths |> Seq.toList

    Chart.Line(x = times, y = necks)
    |> Commons.ApplyDefaultPlotProperties
    |> Chart.withXAxisStyle (TitleText = "Time", AxisType = AxisType.Log)
    |> Chart.withYAxisStyle (TitleText = "Neck Width", AxisType = AxisType.Log)


let PlotParticleCenter (states: IParticle<_> seq) =
    let statesArray = states |> Seq.toArray
    let id = statesArray[0].Id

    for s in states do
        if s.Id <> id then
            failwith ("all states must represent the same particle (have same id)")

    let absoluteCoordinates = [ for s in statesArray -> s.Coordinates.Absolute ]
    let x = [ for c in absoluteCoordinates -> c.X ]
    let y = [ for c in absoluteCoordinates -> c.Y ]

    Chart.Line(x = x, y = y, Name = "Center of " + statesArray[0].Id.ToString().Substring(0, 8))
    |> Commons.ApplyPlainSpaceDefaultPlotProperties

let PlotParticleRotation (states: IParticle<_> seq) =
    let statesArray = states |> Seq.toArray
    let id = statesArray[0].Id

    let coords =
        seq {
            for s in states do
                if s.Id <> id then
                    failwith ("all states must represent the same particle (have same id)")

                let absoluteCoordinates = s.Coordinates.Absolute
                let lineEnd = absoluteCoordinates + AbsoluteVector(1, 0).RotateBy(s.RotationAngle)

                yield (absoluteCoordinates.X, absoluteCoordinates.Y)
                yield (lineEnd.X, lineEnd.Y)
                yield (nan, nan)
        }

    Chart.Line(xy = coords, Name = "Rotation of " + statesArray[0].Id.ToString().Substring(0, 8))
    |> Commons.ApplyPlainSpaceDefaultPlotProperties

let plotForAllParticles (states: ISystemState<_, _> seq) plotFunction : GenericChart =
    let statesArray = states |> Seq.toArray
    let particleIds = [ for p in statesArray[0].Particles -> p.Id ]

    seq {
        for id in particleIds do
            let particleStates = [ for s in statesArray -> s.Particles[id] ]
            yield plotFunction particleStates

    }
    |> Chart.combine
    |> Commons.ApplyPlainSpaceDefaultPlotProperties

let PlotParticleCenters (states: ISystemState<_, _> seq) : GenericChart =
    plotForAllParticles states PlotParticleCenter

let PlotParticleRotations (states: ISystemState<_, _> seq) : GenericChart =
    plotForAllParticles states PlotParticleRotation

let PlotPorePressures (states: ISystemStateWithPores<_, _, _> seq) =
    let times = [ for s in states -> s.Time ]

    let pores =
        states
        |> Seq.map (fun s -> s.Pores |> Seq.map (_.Pressure) |> Seq.map abs |> Seq.toList)
        |> List.transpose

    seq {
        for pore in pores do
            Chart.Line(x = times, y = pore)

    }
    |> Chart.combine
    |> Commons.ApplyDefaultPlotProperties
    |> Chart.withXAxisStyle (TitleText = "Time", AxisType = AxisType.Log)
    |> Chart.withYAxisStyle (TitleText = "Pore Pressure", AxisType = AxisType.Log)

let PlotPoreDensities (states: ISystemStateWithPores<_, _, _> seq) =
    let times = [ for s in states -> s.Time ]

    let pores =
        states
        |> Seq.map (fun s -> s.Pores |> Seq.map (_.RelativeDensity) |> Seq.toList)
        |> List.transpose

    seq {
        for pore in pores do
            Chart.Line(x = times, y = pore)

    }
    |> Chart.combine
    |> Commons.ApplyDefaultPlotProperties
    |> Chart.withXAxisStyle (TitleText = "Time", AxisType = AxisType.Log)
    |> Chart.withYAxisStyle (TitleText = "Pore Density", AxisType = AxisType.Log)

let PlotPoreVolumes (states: ISystemStateWithPores<_, _, _> seq) =
    let times = [ for s in states -> s.Time ]

    let pores =
        states
        |> Seq.map (fun s -> s.Pores |> Seq.map (_.Volume) |> Seq.toList)
        |> List.transpose

    seq {
        for pore in pores do
            Chart.Line(x = times, y = pore)

    }
    |> Chart.combine
    |> Commons.ApplyDefaultPlotProperties
    |> Chart.withXAxisStyle (TitleText = "Time", AxisType = AxisType.Log)
    |> Chart.withYAxisStyle (TitleText = "Pore Volume", AxisType = AxisType.Log)
