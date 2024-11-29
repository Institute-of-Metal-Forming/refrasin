using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Storage;
using CSparseMatrix = CSparse.Double.SparseMatrix;
using CSparseLU = CSparse.Double.Factorization.SparseLU;
using ColumnOrdering =CSparse.ColumnOrdering;

namespace RefraSin.Numerics.LinearSolvers;

/// <summary>
/// Sparse LU solver using the CSparse.NET library.
/// Given matrix is converted to CSparse's matrix type without copying.
/// The given matrix is assumed to be sparse and symmetric (so that CRS and CSC formats are equivalent).
/// </summary>
public record SparseLUSolver(ColumnOrdering ColumnOrdering = ColumnOrdering.MinimumDegreeAtA, double PivotingTolerance = 1.0 ) : ILinearSolver
{
    /// <inheritdoc />
    public Vector<double> Solve(Matrix<double> matrix, Vector<double> rightSide)
    {
        var storage = matrix.Storage as SparseCompressedRowMatrixStorage<double> ?? throw new ArgumentException("matrix must be sparse");
        var cSparseMatrix = new CSparseMatrix(matrix.RowCount, matrix.ColumnCount, storage.Values, storage.ColumnIndices, storage.RowPointers);

        var lu = CSparseLU.Create(cSparseMatrix, ColumnOrdering, PivotingTolerance);
        
        var result = new double[rightSide.Count];
        lu.Solve(rightSide.AsArray() ?? throw new ArgumentException("right side vector must be dense"), result);
        
        return new DenseVector(result);
    }
}
