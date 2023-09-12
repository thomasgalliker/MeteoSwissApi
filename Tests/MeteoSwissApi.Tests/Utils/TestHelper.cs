using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        internal void WriteFile(Stream bitmapStream, [CallerMemberName] string fileName = null, string fileExtension = "png")
        {
            if (bitmapStream == null)
            {
                this.testOutputHelper.WriteLine($"bitmapStream is null; Testfile cannot be written!");
                return;
            }

            var outputFilePath = Path.Combine(this.outputFolder, $"{fileName}.{fileExtension}");
            using (var fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                bitmapStream.CopyTo(fileStream);
            }

            this.testOutputHelper.WriteLine($"Testfile successfully written to: {outputFilePath}");
        }

        internal static async Task<(int IconId, Stream Stream)[]> TryGetIconsAsync(int[] range, Func<int, Task<Stream>> downloadFunc)
        {
            var iconDownloadTasks = range.Select(iconId =>
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        return (IconId: iconId, Stream: await downloadFunc(iconId));
                    }
                    catch (Exception)
                    {
                        return (iconId, null);
                    }
                });
            }).ToArray();

            var downloadedIcons = (await Task.WhenAll(iconDownloadTasks))
                .Where(x => x.Stream != null)
                .ToArray();

            return downloadedIcons;
        }
    }
}