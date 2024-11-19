using System.Globalization;
using System.Reflection;

namespace RefraSin.TEPSolver.Test;

public static class TempPath
{
    public static string CreateTempDir()
    {
        var test = TestContext.CurrentContext.Test;

        var tmpDir = Path.GetTempPath();

        var path = Path.Combine(
            tmpDir,
            "RefraSin.TEPSolver.Test",
            test.ClassName ?? "",
            test.ID,
            DateTime.Now.ToString("o", CultureInfo.InvariantCulture)
        );

        Directory.CreateDirectory(path);
        return path;
    }
}
