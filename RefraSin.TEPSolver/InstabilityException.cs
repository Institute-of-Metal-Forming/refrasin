namespace RefraSin.TEPSolver;

public class InstabilityException : Exception
{
    public InstabilityException(Guid nodeId, int index) : base($"Instability detected above node {nodeId} at index {index}.") { }
}