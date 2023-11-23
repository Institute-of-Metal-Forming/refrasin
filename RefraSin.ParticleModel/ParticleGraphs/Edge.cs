using RefraSin.Coordinates;

namespace RefraSin.ParticleModel.ParticleGraphs;

record Edge(Guid Start, Guid End, Angle Angle) : IEdge { }