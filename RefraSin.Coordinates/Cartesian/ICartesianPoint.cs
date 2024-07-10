using RefraSin.Coordinates.Helpers;

namespace RefraSin.Coordinates.Cartesian;

public interface ICartesianPoint : IPoint, ICartesianCoordinates, IIsClose<ICartesianPoint>;