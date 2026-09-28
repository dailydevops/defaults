namespace NetEvolve.Defaults.Tests.Unit;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;

/// <summary>
/// Covers the distribution of the pinned <c>.csharpierrc.yaml</c>, which CSharpier prefers over
/// <c>.editorconfig</c>, and guards it against drifting from <c>template.editorconfig</c>.
/// </summary>
internal partial class CSharpierRcDistributionTests
{
    private const string FileName = ".csharpierrc.yaml";

    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportAdditionalFiles.targets"
    );

    private static readonly string ConfigurationsDirectory = Path.GetFullPath(
        Path.Combine(MSBuildProjectFixture.BuildMultiTargetingDirectory, "..", "configurations")
    );

    private static readonly string Template = Path.Combine(ConfigurationsDirectory, "template.csharpierrc.yaml");

    private static readonly string EditorConfigTemplate = Path.Combine(
        ConfigurationsDirectory,
        "template.editorconfig"
    );

    [Test]
    public async Task UpdateEditorConfig_Missing_WritesTemplateToRoot()
    {
        using var evaluated = await EvaluateAsync().ConfigureAwait(false);
        var destination = Path.Combine(evaluated.Directory, FileName);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert
                .That(await File.ReadAllTextAsync(destination).ConfigureAwait(false))
                .IsEqualTo(await File.ReadAllTextAsync(Template).ConfigureAwait(false));
        }
    }

    [Test]
    public async Task UpdateEditorConfig_Differs_IsReplacedWithoutLeavingTemporaryFiles()
    {
        using var evaluated = await EvaluateAsync().ConfigureAwait(false);
        var destination = Path.Combine(evaluated.Directory, FileName);
        await File.WriteAllTextAsync(destination, "printWidth: 80").ConfigureAwait(false);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert
                .That(await File.ReadAllTextAsync(destination).ConfigureAwait(false))
                .IsEqualTo(await File.ReadAllTextAsync(Template).ConfigureAwait(false));
            _ = await Assert.That(Directory.GetFiles(evaluated.Directory, "*.tmp")).IsEmpty();
        }
    }

    [Test]
    public async Task UpdateEditorConfig_EqualsTemplate_IsNotRewritten()
    {
        using var evaluated = await EvaluateAsync().ConfigureAwait(false);
        var destination = Path.Combine(evaluated.Directory, FileName);
        File.Copy(Template, destination);
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
    public async Task DisableSupportAdditionalFiles_IsNotWritten()
    {
        using var evaluated = await EvaluateAsync(
                new Dictionary<string, string> { ["DisableSupportAdditionalFiles"] = "true" }
            )
            .ConfigureAwait(false);

        var success = evaluated.BuildTarget("UpdateEditorConfig");

        using (Assert.Multiple())
        {
            _ = await Assert.That(success).IsTrue();
            _ = await Assert.That(File.Exists(Path.Combine(evaluated.Directory, FileName))).IsFalse();
        }
    }

    [Test]
    public async Task PackageProject_PacksTemplateIntoConfigurationsFolder()
    {
        var projectPath = Path.Combine(ConfigurationsDirectory, "..", "NetEvolve.Defaults.csproj");
        using var collection = new ProjectCollection();
        var project = collection.LoadProject(projectPath);

        var packagePaths = project
            .GetItems("None")
            .Where(i =>
                string.Equals(i.GetMetadataValue("FullPath"), Template, StringComparison.OrdinalIgnoreCase)
                && string.Equals(i.GetMetadataValue("Pack"), "true", StringComparison.OrdinalIgnoreCase)
            )
            .Select(i => i.GetMetadataValue("PackagePath").Replace('\\', '/').TrimEnd('/'))
            .ToList();

        _ = await Assert.That(packagePaths).IsEquivalentTo(["configurations"]);
    }

    [Test]
    public async Task Template_IsConsistentWithEditorConfigTemplate()
    {
        var (root, xml) = ParseCSharpierRc(await File.ReadAllLinesAsync(Template).ConfigureAwait(false));
        var sections = ParseEditorConfig(await File.ReadAllLinesAsync(EditorConfigTemplate).ConfigureAwait(false));

        var cs = Effective(sections, "cs");
        using (Assert.Multiple())
        {
            _ = await Assert.That(root["printWidth"]).IsEqualTo(cs["max_line_length"]);
            _ = await Assert.That(root["indentSize"]).IsEqualTo(cs["indent_size"]);
            _ = await Assert.That(root["useTabs"]).IsEqualTo(UseTabs(cs));
            _ = await Assert.That(root["endOfLine"]).IsEqualTo(cs["end_of_line"]);
            _ = await Assert.That(xml["formatter"]).IsEqualTo("xml");

            foreach (var extension in Extensions(xml["files"]))
            {
                var settings = Effective(sections, extension);
                _ = await Assert.That(xml["printWidth"]).IsEqualTo(settings["max_line_length"]);
                _ = await Assert.That(xml["indentSize"]).IsEqualTo(settings["indent_size"]);
                _ = await Assert.That(xml["useTabs"]).IsEqualTo(UseTabs(settings));
                _ = await Assert.That(xml["endOfLine"]).IsEqualTo(settings["end_of_line"]);
            }
        }
    }

    private static async Task<MSBuildProjectFixture.EvaluatedProject> EvaluateAsync(
        IDictionary<string, string>? globalProperties = null
    )
    {
        var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);
        await File.WriteAllTextAsync(Path.Combine(evaluated.Directory, "Directory.Packages.props"), "<Project />")
            .ConfigureAwait(false);
        return evaluated;
    }

    // Minimal reader for the flat layout of template.csharpierrc.yaml: top-level scalars plus a single override.
    private static (Dictionary<string, string> Root, Dictionary<string, string> Override) ParseCSharpierRc(
        string[] lines
    )
    {
        var root = new Dictionary<string, string>(StringComparer.Ordinal);
        var overrides = new List<Dictionary<string, string>>();
        foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l) && !l.TrimStart().StartsWith('#')))
        {
            var trimmed = line.Trim();
            var isNewOverride = trimmed.StartsWith("- ", StringComparison.Ordinal);
            if (isNewOverride)
            {
                overrides.Add(new Dictionary<string, string>(StringComparer.Ordinal));
                trimmed = trimmed[2..];
            }

            var separator = trimmed.IndexOf(':', StringComparison.Ordinal);
            var key = trimmed[..separator].Trim();
            var value = trimmed[(separator + 1)..].Trim().Trim('"');
            if (string.Equals(key, "overrides", StringComparison.Ordinal))
            {
                continue;
            }

            var target = char.IsWhiteSpace(line[0]) ? overrides[^1] : root;
            target[key] = value;
        }

        return (root, overrides.Single());
    }

    private static List<(string[] Extensions, Dictionary<string, string> Settings)> ParseEditorConfig(string[] lines)
    {
        var sections = new List<(string[] Extensions, Dictionary<string, string> Settings)>();
        Dictionary<string, string>? current = null;
        foreach (var line in lines.Select(l => l.Trim()).Where(l => l.Length > 0 && l[0] is not ('#' or ';')))
        {
            if (line[0] == '[')
            {
                current = new Dictionary<string, string>(StringComparer.Ordinal);
                sections.Add((SectionExtensions(line[1..^1]), current));
            }
            else if (current is not null)
            {
                var separator = line.IndexOf('=', StringComparison.Ordinal);
                current[line[..separator].Trim()] = line[(separator + 1)..].Trim();
            }
        }

        return sections;
    }

    private static Dictionary<string, string> Effective(
        List<(string[] Extensions, Dictionary<string, string> Settings)> sections,
        string extension
    )
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (
            var (_, settings) in sections.Where(s => s.Extensions.Contains("*") || s.Extensions.Contains(extension))
        )
        {
            foreach (var setting in settings)
            {
                result[setting.Key] = setting.Value;
            }
        }

        return result;
    }

    private static string UseTabs(Dictionary<string, string> settings) =>
        string.Equals(settings["indent_style"], "tab", StringComparison.Ordinal) ? "true" : "false";

    // Only `*` and plain `*.ext` / `*.{a,b}` sections apply to every file of an extension; others match nothing here.
    private static string[] SectionExtensions(string glob) =>
        glob switch
        {
            "*" => ["*"],
            _ when SimpleGlob().IsMatch(glob) => Extensions(glob),
            _ => [],
        };

    private static string[] Extensions(string glob) =>
        glob.TrimStart('*', '.').Trim('{', '}').Split(',', StringSplitOptions.TrimEntries);

    [GeneratedRegex(@"^\*\.(\{[\w.,]+\}|[\w.]+)$", RegexOptions.ExplicitCapture, matchTimeoutMilliseconds: 1000)]
    private static partial Regex SimpleGlob();
}
