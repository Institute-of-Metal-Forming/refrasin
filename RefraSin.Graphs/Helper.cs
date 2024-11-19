namespace RefraSin.Graphs;

static class Helper
{
    public static Guid MergeGuids(Guid id1, Guid id2)
    {
        var bytes1 = id1.ToByteArray();
        var bytes2 = id2.ToByteArray();
        var xoredBytes = new byte[16];

        for (var i = 0; i < 16; i++)
        {
            xoredBytes[i] = (byte)(bytes1[i] ^ bytes2[i]);
        }

        return new Guid(xoredBytes);
    }
}
