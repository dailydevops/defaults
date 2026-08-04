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

        _ = await Assert.That(recommended).Contains("Meziantou.Analyzer");
        _ = await Assert.That(recommended).Contains("Microsoft.CodeAnalysis.BannedApiAnalyzers");
        _ = await Assert.That(recommended).Contains("Microsoft.CodeAnalysis.NetAnalyzers");
        _ = await Assert.That(recommended).Contains("Microsoft.VisualStudio.Threading.Analyzers");
        _ = await Assert.That(recommended).Contains("NetEvolve.Analyzer");
        _ = await Assert.That(recommended).Contains("NetEvolve.Defaults");
        _ = await Assert.That(recommended).Contains("Roslynator.Analyzers");
        _ = await Assert.That(recommended).Contains("Roslynator.Formatting.Analyzers");
        _ = await Assert.That(recommended).Contains("Roslynator.CodeAnalysis.Analyzers");
        _ = await Assert.That(recommended).Contains("Roslynator.CodeFixes");
        _ = await Assert.That(recommended).Contains("Roslynator.Refactorings");
        _ = await Assert.That(recommended).Contains("SonarAnalyzer.CSharp");
        _ = await Assert.That(recommended.Count).IsEqualTo(12);
    }
}
