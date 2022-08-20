using System.IO;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace MeteoSwissApi.Tests.Utils
{
    internal class TestHelper
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly string outputFolder;

        public TestHelper(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            this.outputFolder = Path.GetFullPath($"./TestOutput");
            if (!Directory.Exists(this.outputFolder))
            {
                Directory.CreateDirectory(this.outputFolder);
            }
        }

        internal void WriteFile(Stream bitmapStream, string fileExtension = "png", [CallerMemberName] string callerMemberName = null)
        {
            if (bitmapStream == null)
            {
                this.testOutputHelper.WriteLine($"bitmapStream is null; Testfile cannot be written!");
                return;
            }

            var outputFilePath = Path.Combine(this.outputFolder, $"{callerMemberName}.{fileExtension}");
            using (var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                bitmapStream.CopyTo(fileStream);
            }

            this.testOutputHelper.WriteLine($"Testfile successfully written to: {outputFilePath}");
        }
    }
}