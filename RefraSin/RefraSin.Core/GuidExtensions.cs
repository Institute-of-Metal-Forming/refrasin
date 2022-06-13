using System;

namespace RefraSin.Core
{
    /// <summary>
    /// Static class providing extension methods for <see cref="Guid"/>.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Gives a shortended string representation of this <see cref="Guid"/>.
        /// Uses <paramref name="self"/>.<see cref="Guid.ToString(string)"/> with format "N" and shortens with <see cref="string.Substring(int,int)"/> with paramters 0 and <paramref name="length"/>.
        /// </summary>
        /// <param name="self">self</param>
        /// <param name="length">length of the resulting string, default: 8</param>
        /// <returns></returns>
        public static string ToShortString(this Guid self, int length = 8)
        {
            return self.ToString("N").Substring(0, length);
        }
    }
}