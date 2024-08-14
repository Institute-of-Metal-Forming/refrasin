module RefraSin.Analysis.ParticleAnalysis

open System.Collections.Generic
open type RefraSin.Coordinates.Constants
open type System.Math
open type RefraSin.Coordinates.Angle
open RefraSin.Coordinates.Helpers
open RefraSin.Coordinates.Polar
open RefraSin.ParticleModel.Particles
open RefraSin.ParticleModel.Nodes

let Volume (particle: IParticle<_>) : float =
    particle.Nodes |> Seq.sumBy (_.Volume.ToUpper)

let EquivalentRadius (particle: IParticle<_>) : float = Volume particle / Pi |> Sqrt

let Perimeter (particle: IParticle<_>) : float =
    particle.Nodes |> Seq.sumBy (_.SurfaceDistance.ToUpper)

let Circularity (particle: IParticle<_>) : float =
    TwoPi * EquivalentRadius particle / Perimeter particle
    
let TotalInterfaceEnergy (particle: IParticle<'TNode> when 'TNode :> INodeMaterialProperties) : float =
    particle.Nodes |> Seq.sumBy (fun n -> n.SurfaceDistance.ToUpper * n.InterfaceEnergy.ToUpper)
    
let ConvexHull (particle: IParticle<'TNode>) : IPolarPoint seq =
    let outermostNode = particle.Nodes |> Seq.maxBy (_.Coordinates.R)
    let nodes = particle.Nodes[outermostNode.Id, outermostNode.Lower.Id]
    
    let hullNodes = LinkedList<'TNode>()
    
    for next in nodes do
        if hullNodes.Count < 2 then
            hullNodes.AddLast(next) |> ignore
        
        while not (next.Id.Equals(hullNodes.Last.Value.Id)) do
            let head = hullNodes.Last.Value
            let prev = hullNodes.Last.Previous.Value
            
            let angleToPrev = head.Coordinates.AngleTo(prev.Coordinates, false)         
            let angleToNext = head.Coordinates.AngleTo(next.Coordinates, false)
            
            let distanceToPrev = CosLaw.C(head.Coordinates.R, prev.Coordinates.R, angleToPrev)
            let direction = CosLaw.Gamma(prev.Coordinates.R, distanceToPrev, head.Coordinates.R)
            
            let borderlineRadius = SinLaw.A(prev.Coordinates.R, direction, Straight - direction - angleToPrev - angleToNext)
            
            if next.Coordinates.R <= borderlineRadius then
                hullNodes.AddLast(next) |> ignore
            else
                hullNodes.RemoveLast()
            
    hullNodes |> Seq.map (_.Coordinates)
