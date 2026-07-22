namespace NetEvolve.Defaults.Tests.Unit;

using System.Collections.Generic;

internal class SupportTestProjectsTests
{
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportTestProjects.targets"
    );

    [Test]
    public async Task AlwaysForcesTestProjectShape()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("IsPublishable")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("false");
    }

    [Test]
    public async Task OverridesPreSetOpposingValuesUnconditionally()
    {
        var preSetProperties = new Dictionary<string, string>
        {
            ["IsTestProject"] = "false",
            ["IsPublishable"] = "true",
            ["IsPackable"] = "true",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], preSetProperties: preSetProperties);

        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("IsPublishable")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("false");
    }
}
