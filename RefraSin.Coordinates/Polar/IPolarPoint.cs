using RefraSin.Coordinates.Helpers;

namespace RefraSin.Coordinates.Polar;

public interface IPolarPoint : IPoint, IPolarCoordinates, IIsClose<IPolarPoint>;
