namespace NetEvolve.Defaults.Tests.Unit;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Evaluation;

internal static class MSBuildProjectFixture
{
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

        File.WriteAllText(projectPath, content.ToString());

        var effectiveGlobalProperties = globalProperties ?? new Dictionary<string, string>();
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
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Defaults.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException("Could not locate repository root containing Defaults.sln.");
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
