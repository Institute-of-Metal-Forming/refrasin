module RefraSin.Analysis.SinteringAnalysis

open RefraSin.ProcessModel

let Shrinkage (initialState: ISystemState<_,_>) (currentState: ISystemState<_,_>) : float =
    let initialVolume = ParticleSystemAnalysis.CenterHullParticle initialState |> ParticleAnalysis.Volume
    let currentVolume = ParticleSystemAnalysis.CenterHullParticle currentState |> ParticleAnalysis.Volume
    
    1.0 - currentVolume / initialVolume
    

let Shrinkages (states: ISystemState<_,_> seq) : float seq =
    let volumes = [ for s in states -> ParticleSystemAnalysis.CenterHullParticle s |> ParticleAnalysis.Volume]
    
    [ for v in volumes -> 1.0 - v / volumes[0] ]
