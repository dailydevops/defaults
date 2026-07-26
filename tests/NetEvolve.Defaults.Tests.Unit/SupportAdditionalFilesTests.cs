namespace NetEvolve.Defaults.Tests.Unit;

using System.Collections.Generic;
using System.IO;

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

        _ = await Assert.That(evaluated.GetProperty("IsCrossTargetingProject")).IsEqualTo("true");
    }

    [Test]
    public async Task TargetFrameworksNotSet_IsCrossTargetingProject_DefaultsToFalse()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        _ = await Assert.That(evaluated.GetProperty("IsCrossTargetingProject")).IsEqualTo("false");
    }

    [Test]
    public async Task TargetFrameworksNotSet_UpdateEditorConfig_CopiesEditorConfig()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);
        await File.WriteAllTextAsync(Path.Combine(evaluated.Directory, "Directory.Packages.props"), "<Project />")
            .ConfigureAwait(false);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert.That(File.Exists(Path.Combine(evaluated.Directory, ".editorconfig"))).IsTrue();
        }
    }

    [Test]
    public async Task TargetFrameworksSet_CrossTargetingInnerBuild_UpdateEditorConfig_DoesNotRun()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["TargetFrameworks"] = "net8.0;net9.0",
            ["IsCrossTargetingBuild"] = "false",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);
        await File.WriteAllTextAsync(Path.Combine(evaluated.Directory, "Directory.Packages.props"), "<Project />")
            .ConfigureAwait(false);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert.That(File.Exists(Path.Combine(evaluated.Directory, ".editorconfig"))).IsFalse();
        }
    }
}
