namespace NetEvolve.Defaults.Tests.Unit;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;

internal static class MSBuildProjectFixture
{
    // MSBuild resolves an undefined $(VAR) from the process environment. The test host itself may be
    // running under a real CI system (GitHub Actions, etc.), so every CI-detection-related variable
    // referenced by SupportDetectContinuousIntegration.props/.targets must be neutralized by default,
    // otherwise ambient CI env vars leak into evaluations that don't explicitly set them.
    private static readonly string[] CiRelatedEnvironmentVariableNames =
    [
        "CI",
        "TF_BUILD",
        "BUILD_SOURCEBRANCH",
        "GITHUB_ACTIONS",
        "GITHUB_REF",
        "GITLAB_CI",
        "CI_COMMIT_BRANCH",
        "CI_COMMIT_TAG",
        "CI_MERGE_REQUEST_IID",
        "CI_EXTERNAL_PULL_REQUEST_IID",
        "TEAMCITY_VERSION",
        "TEAMCITY_BUILD_BRANCH",
        "BUILD_COMMAND",
        "APPVEYOR",
        "APPVEYOR_PULL_REQUEST_NUMBER",
        "APPVEYOR_REPO_TAG_NAME",
        "APPVEYOR_REPO_BRANCH",
        "TRAVIS",
        "TRAVIS_PULL_REQUEST",
        "TRAVIS_BRANCH",
        "CIRCLECI",
        "CIRCLE_PR_NUMBER",
        "CIRCLE_TAG",
        "CIRCLE_BRANCH",
        "CODEBUILD_BUILD_ID",
        "AWS_REGION",
        "JENKINS_URL",
        "BUILD_ID",
        "BUILD_URL",
        "PROJECT_ID",
        "JB_SPACE_API_URL",
        "BUDDY_EXECUTION_PULL_REQUEST_NO",
        "BUDDY_EXECUTION_TAG",
        "BUDDY_EXECUTION_BRANCH",
    ];

    private static readonly string RepoRoot = FindRepoRoot();

    public static readonly string BuildMultiTargetingDirectory = Path.Combine(
        RepoRoot,
        "src",
        "NetEvolve.Defaults",
        "buildMultiTargeting"
    );

    public static readonly string BuildDirectory = Path.Combine(RepoRoot, "src", "NetEvolve.Defaults", "build");

    public static string InBuildMultiTargeting(string fileName) => Path.Combine(BuildMultiTargetingDirectory, fileName);

    public static string InBuild(string fileName) => Path.Combine(BuildDirectory, fileName);

    public static EvaluatedProject Evaluate(
        string projectName,
        IEnumerable<string> importPaths,
        IDictionary<string, string>? globalProperties = null,
        IDictionary<string, string>? preSetProperties = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectName);
        ArgumentNullException.ThrowIfNull(importPaths);

        var tempDirectory = new DirectoryInfo(
            Path.Combine(Path.GetTempPath(), "netevolve-defaults-tests-" + Guid.NewGuid().ToString("N"))
        );
        tempDirectory.Create();

        var projectPath = Path.Combine(tempDirectory.FullName, projectName + ".csproj");
        File.WriteAllText(projectPath, BuildProjectContent(importPaths, preSetProperties));

        var effectiveGlobalProperties = BuildEffectiveGlobalProperties(globalProperties);
        var projectCollection = new ProjectCollection(effectiveGlobalProperties);

        Project project;
        try
        {
            project = new Project(projectPath, effectiveGlobalProperties, toolsVersion: null, projectCollection);
        }
        catch
        {
            projectCollection.Dispose();
            TryDeleteDirectory(tempDirectory);
            throw;
        }

        return new EvaluatedProject(project, projectCollection, tempDirectory);
    }

    private static string BuildProjectContent(
        IEnumerable<string> importPaths,
        IDictionary<string, string>? preSetProperties
    )
    {
        var content = new StringBuilder();
        _ = content.AppendLine("<Project>");

        // Properties declared here behave like a real consuming project's own <PropertyGroup> entries -
        // i.e. plain, freely re-assignable properties - as opposed to MSBuild "global properties" (passed via
        // the globalProperties parameter below), which cannot be overridden by any later property assignment,
        // conditioned or not, once evaluation starts.
        if (preSetProperties is { Count: > 0 })
        {
            _ = content.AppendLine("  <PropertyGroup>");
            foreach (var property in preSetProperties)
            {
                _ = content
                    .Append("    <")
                    .Append(property.Key)
                    .Append('>')
                    .Append(property.Value)
                    .Append("</")
                    .Append(property.Key)
                    .AppendLine(">");
            }
            _ = content.AppendLine("  </PropertyGroup>");
        }

        foreach (var importPath in importPaths)
        {
            _ = content.Append("  <Import Project=\"").Append(importPath).AppendLine("\" />");
        }
        _ = content.AppendLine("</Project>");

        return content.ToString();
    }

    private static Dictionary<string, string> BuildEffectiveGlobalProperties(
        IDictionary<string, string>? globalProperties
    )
    {
        var effectiveGlobalProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in CiRelatedEnvironmentVariableNames)
        {
            effectiveGlobalProperties[name] = string.Empty;
        }

        if (globalProperties is not null)
        {
            foreach (var property in globalProperties)
            {
                effectiveGlobalProperties[property.Key] = property.Value;
            }
        }

        return effectiveGlobalProperties;
    }

    private static void TryDeleteDirectory(DirectoryInfo directory)
    {
        try
        {
            directory.Delete(recursive: true);
        }
        catch (IOException)
        {
            // Best effort cleanup, temp directories are periodically cleaned by the OS anyway.
        }
        catch (UnauthorizedAccessException)
        {
            // Best effort cleanup, temp directories are periodically cleaned by the OS anyway.
        }
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Defaults.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("Could not locate repository root containing Defaults.slnx.");
    }

    internal sealed class EvaluatedProject : IDisposable
    {
        private readonly ProjectCollection _projectCollection;
        private readonly DirectoryInfo _tempDirectory;
        private bool _disposed;

        internal EvaluatedProject(Project project, ProjectCollection projectCollection, DirectoryInfo tempDirectory)
        {
            Project = project;
            _projectCollection = projectCollection;
            _tempDirectory = tempDirectory;
        }

        public Project Project { get; }

        public string GetProperty(string name) => Project.GetPropertyValue(name);

        public IReadOnlyList<string> GetItemIncludes(string itemType) =>
            [.. Project.GetItems(itemType).Select(item => item.EvaluatedInclude)];

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _projectCollection.UnloadProject(Project);
            _projectCollection.Dispose();
            TryDeleteDirectory(_tempDirectory);
        }
    }
}
