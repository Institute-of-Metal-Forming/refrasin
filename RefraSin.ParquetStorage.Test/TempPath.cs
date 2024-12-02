using System.Globalization;

namespace RefraSin.ParquetStorage.Test;

public static class TempPath
{
    public static string CreateTempDir()
    {
        var test = TestContext.CurrentContext.Test;

        var tmpDir = Path.GetTempPath();

        var path = Path.Combine(
            tmpDir,
            "RefraSin.ParquetStorage.Test",
            test.ClassName ?? "",
            test.MethodName ?? "",
            // test.ID,
            DateTime.Now.ToString("o", CultureInfo.InvariantCulture)
        );

        Directory.CreateDirectory(path);
        return path;
    }
}
