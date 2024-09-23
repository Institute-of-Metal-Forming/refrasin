module RefraSin.Plotting.ProcessPlot

open Plotly.NET
open Plotly.NET.StyleParam
open RefraSin.Coordinates.Absolute
open RefraSin.ParticleModel.Particles
open RefraSin.ProcessModel
open RefraSin.Analysis

let PlotTimeSteps (states: ISystemState<_,_> seq) =
    let times = [for s in states -> s.Time]
    let indices = [1 .. times.Length + 1]
    let timeSteps = List.mapi (fun i t1 -> t1 - times[i]) times.Tail
    
    Chart.Line(x=indices, y=timeSteps) |> Chart.withXAxisStyle(TitleText="# of Step", AxisType=AxisType.Category) |> Chart.withYAxisStyle(TitleText="Time Step Width")
    
let PlotShrinkages (states: ISystemState<_,_> seq) =
    let times = [for s in states -> s.Time]
    let shrinkages = states |> SinteringAnalysis.Shrinkages |> Seq.map (fun s -> s * 100.0) |> Seq.toList
    
    Chart.Line(x=times, y=shrinkages) |> Chart.withXAxisStyle(TitleText="Time", AxisType=AxisType.Category) |> Chart.withYAxisStyle(TitleText="Shrinkage in %")
    

let PlotParticleCenter (states: IParticle<_> seq) =
    let statesArray = states |> Seq.toArray
    let id = statesArray[0].Id
    for s in states do if s.Id <> id then failwith("all states must represent the same particle (have same id)")
    
    let absoluteCoordinates = [for s in statesArray -> s.Coordinates.Absolute]
    let x = [for c in absoluteCoordinates -> c.X]
    let y = [for c in absoluteCoordinates -> c.Y]
    
    Chart.Line(x=x, y=y, Name="Center of " + statesArray[0].Id.ToString().Substring(0,8)) |> Commons.ApplyPlainSpaceDefaultPlotProperties
    

    

let PlotParticleRotation (states: IParticle<_> seq) =
    let statesArray = states |> Seq.toArray
    let id = statesArray[0].Id
    let coords = seq {
        for s in states do
            if s.Id <> id then failwith("all states must represent the same particle (have same id)")
            
            let absoluteCoordinates = s.Coordinates.Absolute
            let lineEnd = absoluteCoordinates + AbsoluteVector(1,0).RotateBy(s.RotationAngle)
            
            yield (absoluteCoordinates.X, absoluteCoordinates.Y)
            yield (lineEnd.X, lineEnd.Y)
            yield (nan, nan)
    }
            
    Chart.Line(xy=coords, Name="Rotation of " + statesArray[0].Id.ToString().Substring(0,8)) |> Commons.ApplyPlainSpaceDefaultPlotProperties
        
let plotForAllParticles (states: ISystemState<_,_> seq) plotFunction : GenericChart =
    let statesArray = states |> Seq.toArray
    let particleIds = [for p in statesArray[0].Particles -> p.Id]
    
    seq{
        for id in particleIds do
            let particleStates = [for s in statesArray -> s.Particles[id]]
            yield plotFunction particleStates
            
    } |> Chart.combine |> Commons.ApplyPlainSpaceDefaultPlotProperties
    
let PlotParticleCenters (states: ISystemState<_,_> seq) : GenericChart =
    plotForAllParticles states PlotParticleCenter
    
let PlotParticleRotations (states: ISystemState<_,_> seq) : GenericChart =
    plotForAllParticles states PlotParticleRotation
