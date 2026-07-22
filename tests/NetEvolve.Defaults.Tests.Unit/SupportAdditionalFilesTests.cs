namespace NetEvolve.Defaults.Tests.Unit;

using System.Collections.Generic;

internal class SupportAdditionalFilesTests
{
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportAdditionalFiles.targets"
    );

    [Test]
    public async Task TargetFrameworksSet_IsCrossTargetingProjectIsTrue()
    {
        var globalProperties = new Dictionary<string, string> { ["TargetFrameworks"] = "net8.0;net9.0" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsCrossTargetingProject")).IsEqualTo("true");
    }

    [Test]
    public async Task TargetFrameworksNotSet_IsCrossTargetingProject_ObservedBehaviorDiffersFromDocumentedIntent_StaysEmptyNotFalse()
    {
        // Unlike IsTestableProject/IsXampleProject in SupportGeneral.props (which are seeded to an explicit
        // "false" default before the conditional override), IsCrossTargetingProject here has no unconditioned
        // default. When TargetFrameworks is unset the property is simply never assigned, so it evaluates to the
        // empty string rather than the string "false".
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        await Assert.That(evaluated.GetProperty("IsCrossTargetingProject")).IsEqualTo("");
    }
}
