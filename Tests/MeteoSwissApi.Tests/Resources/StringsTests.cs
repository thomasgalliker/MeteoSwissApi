using System.Resources;
using System.Resources.Checks;

namespace MeteoSwissApi.Tests.Resources
{
    public class StringsTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public StringsTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public static TheoryData<IResxCheck> ResxChecks =>
            new()
            {
                new CompletenessCheck(),
                new PunctuationConsistencyCheck(),
                new PlaceholderConsistencyCheck(),
            };

        [Theory]
        [MemberData(nameof(ResxChecks))]
        public void ShouldAnalyzeStringResources(IResxCheck check)
        {
            // Arrange
            var resourcesDirectory = Path.Combine(
                FindRepositoryRoot(),
                "MeteoSwissApi",
                "Resources",
                "Strings");

            var resxAnalyzer = ResxAnalyzer
                .ForResource(resourcesDirectory)
                .WithChecks(checks => checks.Add(check))
                .Build();

            // Act
            var result = resxAnalyzer.Analyze();

            // Assert
            this.testOutputHelper.WriteLine(result.Report);
            result.Succeeded.Should().BeTrue(result.Report);
        }

        private static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "MeteoSwissApi.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Could not locate the MeteoSwissApi repository root.");
        }
    }
}
