namespace MeteoSwissApi.Tests.Utils
{
    internal sealed class TestHelper
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

        /// <summary>
        /// Gets the icons in <paramref name="range"/> and returns those that exist.
        /// An icon id fails if <paramref name="downloadFunc"/> throws or returns the transparent icon,
        /// which the icon mappings return for icon ids without icon.
        /// </summary>
        internal static async Task<(int IconId, Stream Stream)[]> TryGetIconsAsync(int[] range, Func<int, Task<Stream>> downloadFunc)
        {
            var iconDownloadTasks = range.Select(iconId =>
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        var memoryStream = new MemoryStream();
                        using (var stream = await downloadFunc(iconId))
                        {
                            await stream.CopyToAsync(memoryStream);
                        }

                        memoryStream.Position = 0;
                        return IsTransparentIcon(memoryStream)
                            ? (IconId: iconId, Stream: null)
                            : (IconId: iconId, Stream: (Stream)memoryStream);
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

        /// <summary>
        /// Returns <c>true</c> if <paramref name="iconStream"/> contains the transparent 1x1 icon.
        /// The stream position is restored.
        /// </summary>
        internal static bool IsTransparentIcon(Stream iconStream)
        {
            var position = iconStream.Position;
            using (var streamReader = new StreamReader(iconStream, System.Text.Encoding.UTF8, true, 1024, leaveOpen: true))
            {
                var content = streamReader.ReadToEnd();
                iconStream.Position = position;
                return content.Contains("width=\"1px\" height=\"1px\"");
            }
        }
    }
}