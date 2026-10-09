using System.Reflection;

namespace MeteoSwissApi.Tests.Utils
{
    /// <summary>
    /// Loads captured API responses that are embedded in the test assembly (Resources/ApiResponses).
    /// </summary>
    internal static class Fixtures
    {
        private static readonly Assembly Assembly = typeof(Fixtures).Assembly;

        internal static string GetString(string fileName)
        {
            using var stream = GetStream(fileName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        internal static byte[] GetBytes(string fileName)
        {
            using var stream = GetStream(fileName);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        private static Stream GetStream(string fileName)
        {
            var resourceName = Assembly.GetManifestResourceNames()
                .Single(n => n.EndsWith(fileName, StringComparison.Ordinal));

            return Assembly.GetManifestResourceStream(resourceName)!;
        }
    }
}
