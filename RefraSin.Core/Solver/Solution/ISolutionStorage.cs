namespace RefraSin.Core.Solver;

public interface ISolutionStorage
{
    public void Store(ISolutionTimeStep timeStep);
}