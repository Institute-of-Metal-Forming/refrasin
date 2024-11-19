module RefraSin.Analysis.ParticleSystemAnalysis

open System
open RefraSin.Coordinates
open RefraSin.Coordinates.Absolute
open RefraSin.Coordinates.Polar
open RefraSin.ParticleModel.Nodes
open RefraSin.ParticleModel.Particles
open RefraSin.ParticleModel.System

let SolidVolumeFraction (state: IParticleSystem<_, _>) : float = 0

let VoidVolumeFraction (state: IParticleSystem<_, _>) : float = 1.0 - SolidVolumeFraction state

let CenterHull (state: IParticleSystem<_, _>) : IPolarPoint seq =
    let centers = [ for p in state.Particles -> p.Coordinates.Absolute ]

    let centroid =
        List.fold (fun (c: AbsolutePoint) -> c.Centroid) centers[0] centers[1..]

    let system = PolarCoordinateSystem(centroid)

    let polarCenters =
        [ for p in centers -> PolarPoint(p, system) ] |> List.sortBy (_.Phi.Radians)

    let dummyParticle =
        Particle(
            Guid.Empty,
            centroid,
            Angle.Zero,
            Guid.Empty,
            (fun particle ->
                Seq.map2
                    (fun c (p: IParticle<_>) ->
                        ParticleNode(p.Id, (particle :?> IParticle<IParticleNode>), c, NodeType.Surface))
                    polarCenters
                    state.Particles)
        )

    ParticleAnalysis.ConvexHull dummyParticle

let CenterHullParticle (state: IParticleSystem<_, _>) : Particle<ParticleNode> =
    let hullPoints = [ for p in CenterHull state -> p.Absolute ]

    let centroid =
        List.fold (fun (c: AbsolutePoint) -> c.Centroid) hullPoints[0] hullPoints[1..]

    let system = PolarCoordinateSystem(centroid)

    Particle(
        Guid.NewGuid(),
        centroid,
        Angle.Zero,
        Guid.Empty,
        (fun particle ->
            Seq.map2
                (fun (c: AbsolutePoint) (p: IParticle<_>) ->
                    ParticleNode(
                        p.Id,
                        (particle :?> IParticle<IParticleNode>),
                        PolarPoint(c, system),
                        NodeType.Surface
                    ))
                hullPoints
                state.Particles)
    )
