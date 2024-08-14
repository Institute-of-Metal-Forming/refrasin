module RefraSin.Analysis.ParticleSystemAnalysis

open RefraSin.ParticleModel.System

let SolidVolumeFraction (state: IParticleSystem<_, _>) : float = 0

let VoidVolumeFraction (state: IParticleSystem<_, _>) : float = 1.0 - SolidVolumeFraction state
