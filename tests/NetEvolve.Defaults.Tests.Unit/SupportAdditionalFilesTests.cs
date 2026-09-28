namespace NetEvolve.Defaults.Tests.Unit;

using System;
using System.Collections.Generic;
using System.IO;

internal class SupportAdditionalFilesTests
{
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportAdditionalFiles.targets"
    );

    private static string Template(string fileName) =>
        Path.Combine(MSBuildProjectFixture.BuildMultiTargetingDirectory, "..", "configurations", fileName);

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
    [Arguments(".editorconfig")]
    [Arguments(".csharpierrc.yaml")]
    public async Task TargetFrameworksNotSet_UpdateEditorConfig_CopiesFile(string fileName)
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);
        await File.WriteAllTextAsync(Path.Combine(evaluated.Directory, "Directory.Packages.props"), "<Project />")
            .ConfigureAwait(false);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert
                .That(await File.ReadAllTextAsync(Path.Combine(evaluated.Directory, fileName)).ConfigureAwait(false))
                .IsEqualTo(await File.ReadAllTextAsync(Template($"template{fileName}")).ConfigureAwait(false));
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

    [Test]
    [Arguments(".editorconfig")]
    [Arguments(".csharpierrc.yaml")]
    public async Task UpdateEditorConfig_DestinationDiffers_IsReplacedWithoutLeavingTemporaryFiles(string fileName)
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);
        await File.WriteAllTextAsync(Path.Combine(evaluated.Directory, "Directory.Packages.props"), "<Project />")
            .ConfigureAwait(false);
        var destination = Path.Combine(evaluated.Directory, fileName);
        await File.WriteAllTextAsync(destination, "root = true").ConfigureAwait(false);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert
                .That(await File.ReadAllTextAsync(destination).ConfigureAwait(false))
                .IsEqualTo(await File.ReadAllTextAsync(Template($"template{fileName}")).ConfigureAwait(false));
            _ = await Assert.That(Directory.GetFiles(evaluated.Directory, "*.tmp")).IsEmpty();
        }
    }

    [Test]
    [Arguments(".editorconfig")]
    [Arguments(".csharpierrc.yaml")]
    public async Task UpdateEditorConfig_DestinationEqualsTemplate_IsNotRewritten(string fileName)
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);
        await File.WriteAllTextAsync(Path.Combine(evaluated.Directory, "Directory.Packages.props"), "<Project />")
            .ConfigureAwait(false);
        var destination = Path.Combine(evaluated.Directory, fileName);
        File.Copy(Template($"template{fileName}"), destination);
        var lastWriteTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(destination, lastWriteTime);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert.That(File.GetLastWriteTimeUtc(destination)).IsEqualTo(lastWriteTime);
        }
    }

    [Test]
    [Arguments(".editorconfig")]
    [Arguments(".csharpierrc.yaml")]
    public async Task DisableSupportAdditionalFiles_UpdateEditorConfig_DoesNotRun(string fileName)
    {
        var globalProperties = new Dictionary<string, string> { ["DisableSupportAdditionalFiles"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);
        await File.WriteAllTextAsync(Path.Combine(evaluated.Directory, "Directory.Packages.props"), "<Project />")
            .ConfigureAwait(false);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert.That(File.Exists(Path.Combine(evaluated.Directory, fileName))).IsFalse();
        }
    }
}
