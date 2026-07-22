namespace NetEvolve.Defaults.Tests.Unit;

internal class SupportMissingAnalyzersTests
{
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportMissingAnalyzers.targets"
    );

    [Test]
    public async Task RecommendedPackage_ContainsExpectedFixedList()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        var recommended = evaluated.GetItemIncludes("RecommendedPackage");

        await Assert.That(recommended).Contains("Meziantou.Analyzer");
        await Assert.That(recommended).Contains("Microsoft.CodeAnalysis.BannedApiAnalyzers");
        await Assert.That(recommended).Contains("Microsoft.CodeAnalysis.NetAnalyzers");
        await Assert.That(recommended).Contains("Microsoft.VisualStudio.Threading.Analyzers");
        await Assert.That(recommended).Contains("NetEvolve.Defaults");
        await Assert.That(recommended).Contains("Roslynator.Analyzers");
        await Assert.That(recommended).Contains("Roslynator.Formatting.Analyzers");
        await Assert.That(recommended).Contains("Roslynator.CodeAnalysis.Analyzers");
        await Assert.That(recommended).Contains("Roslynator.CodeFixes");
        await Assert.That(recommended).Contains("Roslynator.Refactorings");
        await Assert.That(recommended).Contains("SonarAnalyzer.CSharp");
        await Assert.That(recommended.Count).IsEqualTo(11);
    }
}
