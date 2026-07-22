namespace NetEvolve.Defaults.Tests.Unit;

using System;
using System.Collections.Generic;

internal class SupportGeneralTests
{
    private static readonly string PropsFile = MSBuildProjectFixture.InBuildMultiTargeting("SupportGeneral.props");
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting("SupportGeneral.targets");
    private static readonly string TestProjectsTargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportTestProjects.targets"
    );

    [Test]
    [Arguments("Foo.Tests.Unit", true)]
    [Arguments("Foo.Tests.Integration", true)]
    [Arguments("NetEvolve.Defaults.Tests.Architecture", true)]
    [Arguments("Foo", false)]
    [Arguments("Foo.Testsx", false)]
    [Arguments("FooTests", false)]
    [Arguments("Foo.Xample", false)]
    public async Task IsTestableProject_DerivedFromProjectName(string projectName, bool expected)
    {
        using var evaluated = MSBuildProjectFixture.Evaluate(projectName, [PropsFile]);

        await Assert.That(evaluated.GetProperty("IsTestableProject")).IsEqualTo(expected ? "true" : "false");
    }

    [Test]
    [Arguments("Foo.Xample", true)]
    [Arguments("Xample.Foo", true)]
    [Arguments("MyXampleThing", false)]
    [Arguments("Foo", false)]
    public async Task IsXampleProject_DerivedFromProjectName(string projectName, bool expected)
    {
        using var evaluated = MSBuildProjectFixture.Evaluate(projectName, [PropsFile]);

        await Assert.That(evaluated.GetProperty("IsXampleProject")).IsEqualTo(expected ? "true" : "false");
    }

    [Test]
    public async Task DefaultProperties_AreSetAsDocumented()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile]);

        await Assert.That(evaluated.GetProperty("Nullable")).IsEqualTo("enable");
        await Assert.That(evaluated.GetProperty("ImplicitUsings")).IsEqualTo("enable");
        await Assert.That(evaluated.GetProperty("WarningLevel")).IsEqualTo("9999");
        await Assert.That(evaluated.GetProperty("AnalysisLevel")).IsEqualTo("latest");
        await Assert.That(evaluated.GetProperty("AnalysisMode")).IsEqualTo("All");
        await Assert.That(evaluated.GetProperty("EnableNETAnalyzers")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("Features")).IsEqualTo("strict");
    }

    [Test]
    public async Task UnconditionalProperties_OverrideUserPreSetValues()
    {
        // These are set via preSetProperties (a plain <PropertyGroup> before the <Import>, exactly like a real
        // consuming project's own properties), not via MSBuild "global properties" - global properties can never
        // be overridden by a later property assignment regardless of Condition, which would make this scenario
        // untestable via the globalProperties channel.
        var preSetProperties = new Dictionary<string, string>
        {
            ["Nullable"] = "disable",
            ["ImplicitUsings"] = "disable",
            ["WarningLevel"] = "1",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], preSetProperties: preSetProperties);

        await Assert.That(evaluated.GetProperty("Nullable")).IsEqualTo("enable");
        await Assert.That(evaluated.GetProperty("ImplicitUsings")).IsEqualTo("enable");
        await Assert.That(evaluated.GetProperty("WarningLevel")).IsEqualTo("9999");
    }

    [Test]
    public async Task RootNamespace_UserPreSetValue_IsPreserved()
    {
        var globalProperties = new Dictionary<string, string> { ["RootNamespace"] = "Custom.Namespace" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("RootNamespace")).IsEqualTo("Custom.Namespace");
    }

    [Test]
    public async Task RootNamespace_NotPreSet_DefaultsToProjectName()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo.Bar", [PropsFile]);

        await Assert.That(evaluated.GetProperty("RootNamespace")).IsEqualTo("Foo.Bar");
    }

    [Test]
    public async Task AssemblyName_UserPreSetValue_IsPreserved()
    {
        var globalProperties = new Dictionary<string, string> { ["AssemblyName"] = "Custom.Assembly" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("AssemblyName")).IsEqualTo("Custom.Assembly");
    }

    [Test]
    public async Task AssemblyName_NotPreSet_DefaultsToProjectName()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo.Bar", [PropsFile]);

        await Assert.That(evaluated.GetProperty("AssemblyName")).IsEqualTo("Foo.Bar");
    }

    [Test]
    public async Task LangVersion_PreviewPreSetValue_IsPreserved()
    {
        var globalProperties = new Dictionary<string, string> { ["LangVersion"] = "preview" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("LangVersion")).IsEqualTo("preview");
    }

    [Test]
    public async Task LangVersion_NonPreviewPreSetValue_IsOverriddenToLatest()
    {
        var preSetProperties = new Dictionary<string, string> { ["LangVersion"] = "10.0" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], preSetProperties: preSetProperties);

        await Assert.That(evaluated.GetProperty("LangVersion")).IsEqualTo("latest");
    }

    [Test]
    [Arguments("Debug")]
    [Arguments("")]
    public async Task CheckEolTargetFramework_NotRelease_IsNotForced(string configuration)
    {
        var globalProperties = new Dictionary<string, string> { ["Configuration"] = configuration };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("CheckEolTargetFramework")).IsEqualTo("");
    }

    [Test]
    public async Task CheckEolTargetFramework_Release_IsForcedFalse()
    {
        var globalProperties = new Dictionary<string, string> { ["Configuration"] = "Release" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("CheckEolTargetFramework")).IsEqualTo("false");
    }

    [Test]
    [Arguments("Debug")]
    [Arguments("")]
    public async Task TreatWarningsAsErrors_NotRelease_IsNotSet(string configuration)
    {
        var globalProperties = new Dictionary<string, string> { ["Configuration"] = configuration };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("TreatWarningsAsErrors")).IsEqualTo("");
        await Assert.That(evaluated.GetProperty("MSBuildTreatWarningsAsErrors")).IsEqualTo("");
    }

    [Test]
    public async Task TreatWarningsAsErrors_Release_IsForcedTrue()
    {
        var globalProperties = new Dictionary<string, string> { ["Configuration"] = "Release" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("TreatWarningsAsErrors")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("MSBuildTreatWarningsAsErrors")).IsEqualTo("true");
    }

    [Test]
    public async Task NoWarn_NonTestableProject_DoesNotContainTestOnlySuppressions()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile]);

        var noWarn = evaluated.GetProperty("NoWarn");

        await Assert.That(noWarn).Contains("CA1810");
        await Assert.That(noWarn).Contains("CA1031");
        await Assert.That(noWarn).DoesNotContain("CS8604");
        await Assert.That(noWarn).DoesNotContain("CA2007");
    }

    [Test]
    public async Task NoWarn_TestableProject_ContainsTestOnlySuppressions()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo.Tests.Unit", [PropsFile]);

        var noWarn = evaluated.GetProperty("NoWarn");

        await Assert.That(noWarn).Contains("CA1810");
        await Assert.That(noWarn).Contains("CA1031");
        await Assert.That(noWarn).Contains("CS8604");
        await Assert.That(noWarn).Contains("CA2007");
        await Assert.That(noWarn).Contains("S4144");
    }

    [Test]
    public async Task IsPackableAndIsTestProject_NonTestable_NonXample_ProducesPackableLibrary()
    {
        // SupportGeneral.targets tests IsTestableProject/IsXampleProject with an exact '== false' comparison
        // (not '!= true'), so both must be explicitly seeded with the literal string "false" here.
        var globalProperties = new Dictionary<string, string>
        {
            ["IsTestableProject"] = "false",
            ["IsXampleProject"] = "false",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("false");

        var internalsVisibleTo = evaluated.GetItemIncludes("InternalsVisibleTo");
        await Assert
            .That(internalsVisibleTo)
            .IsEquivalentTo(
                ["Foo.Tests.Architecture", "Foo.Tests.Unit", "Foo.Tests.Integration"],
                TUnit.Assertions.Enums.CollectionOrdering.Any
            );
    }

    [Test]
    public async Task IsPackableAndIsTestProject_XampleProject_IsNeverPackedAndNotATestProject()
    {
        var globalProperties = new Dictionary<string, string> { ["IsXampleProject"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Xample.Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("WarnOnPackingNonPackableProject")).IsEqualTo("false");
    }

    [Test]
    public async Task TestableProject_NotXample_ImportsSupportTestProjects_BecomesTestProject()
    {
        // Mirrors the real conditional import order in buildMultiTargeting/NetEvolve.Defaults.targets,
        // where SupportTestProjects.targets is imported after SupportGeneral.targets when IsTestableProject == true.
        var globalProperties = new Dictionary<string, string> { ["IsTestableProject"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate(
            "Foo.Tests.Unit",
            [TargetsFile, TestProjectsTargetsFile],
            globalProperties
        );

        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("IsPublishable")).IsEqualTo("false");
    }

    [Test]
    public async Task TestableAndXampleProject_SupportTestProjectsOverridesIsTestProjectToTrue()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["IsTestableProject"] = "true",
            ["IsXampleProject"] = "true",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate(
            "Foo.Tests.Unit",
            [TargetsFile, TestProjectsTargetsFile],
            globalProperties
        );

        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("true");
    }
}
