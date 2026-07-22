namespace NetEvolve.Defaults.Tests.Unit;

using System.Collections.Generic;
using Microsoft.Build.Evaluation;

internal class SupportAssemblyAttributesTests
{
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportAssemblyAttributes.targets"
    );

    [Test]
    public async Task AlwaysAddsClsCompliantAttribute()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        await Assert.That(HasAttribute(evaluated.Project, "System.CLSCompliantAttribute")).IsTrue();
    }

    [Test]
    public async Task NonTestable_NonXample_DoesNotAddExcludeFromCodeCoverage()
    {
        var globalProperties = new Dictionary<string, string> { ["TargetFramework"] = "net8.0" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert
            .That(HasAttribute(evaluated.Project, "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute"))
            .IsFalse();
    }

    [Test]
    public async Task TestableProject_CompatibleFramework_AddsExcludeFromCodeCoverage()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["IsTestableProject"] = "true",
            ["TargetFramework"] = "net8.0",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert
            .That(HasAttribute(evaluated.Project, "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute"))
            .IsTrue();
    }

    [Test]
    public async Task XampleProject_CompatibleFramework_AddsExcludeFromCodeCoverage()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["IsXampleProject"] = "true",
            ["TargetFramework"] = "net9.0",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert
            .That(HasAttribute(evaluated.Project, "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute"))
            .IsTrue();
    }

    [Test]
    public async Task TestableProject_IncompatibleFramework_DoesNotAddExcludeFromCodeCoverage()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["IsTestableProject"] = "true",
            ["TargetFramework"] = "netstandard2.0",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert
            .That(HasAttribute(evaluated.Project, "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute"))
            .IsFalse();
    }

    private static bool HasAttribute(Project project, string include) =>
        project
            .GetItems("AssemblyAttribute")
            .Any(item => string.Equals(item.EvaluatedInclude, include, StringComparison.Ordinal));
}
