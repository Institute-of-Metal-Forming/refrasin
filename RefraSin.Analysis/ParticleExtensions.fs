namespace RefraSin.Analysis

open System.Runtime.CompilerServices
open RefraSin.ParticleModel.Particles
open RefraSin.Analysis.ParticleAnalysis

type ParticleExtensions =
    [<Extension>]
    static member Volume(self: IParticle<_>) = Volume self
    
    [<Extension>]
    static member EquivalentRadius(self: IParticle<_>) = EquivalentRadius self

    [<Extension>]
    static member Perimeter(self: IParticle<_>) = Perimeter self
    
    [<Extension>]
    static member Circularity(self: IParticle<_>) = Circularity self
    
    [<Extension>]
    static member TotalInterfaceEnergy(self: IParticle<_>) = TotalInterfaceEnergy self
