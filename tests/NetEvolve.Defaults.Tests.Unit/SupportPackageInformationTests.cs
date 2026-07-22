namespace NetEvolve.Defaults.Tests.Unit;

using System.Collections.Generic;

internal class SupportPackageInformationTests
{
    private static readonly string PropsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportPackageInformation.props"
    );
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportPackageInformation.targets"
    );

    [Test]
    public async Task Props_NonTestable_NotInsideVisualStudio_GeneratesPackageOnBuild()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile]);

        await Assert.That(evaluated.GetProperty("GeneratePackageOnBuild")).IsEqualTo("true");
    }

    [Test]
    public async Task Props_TestableProject_DoesNotGeneratePackageOnBuild()
    {
        var globalProperties = new Dictionary<string, string> { ["IsTestableProject"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("GeneratePackageOnBuild")).IsEqualTo("");
    }

    [Test]
    public async Task Props_InsideVisualStudio_DoesNotGeneratePackageOnBuild()
    {
        var globalProperties = new Dictionary<string, string> { ["BuildingInsideVisualStudio"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("GeneratePackageOnBuild")).IsEqualTo("");
    }

    [Test]
    public async Task Targets_NonTestable_NonXample_DefaultPackageId_UsesProjectName()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo.Bar", [TargetsFile]);

        await Assert.That(evaluated.GetProperty("PackageId")).IsEqualTo("Foo.Bar");
        await Assert.That(evaluated.GetProperty("Title")).IsEqualTo("Foo.Bar");
        await Assert.That(evaluated.GetProperty("PublishRepositoryUrl")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("RepositoryType")).IsEqualTo("git");
        await Assert.That(evaluated.GetProperty("PackageLicenseExpression")).IsEqualTo("MIT");
        await Assert.That(evaluated.GetProperty("PackageTags")).Contains("netevolve");
    }

    [Test]
    public async Task Targets_TestableProject_SkipsPackageInformationEntirely()
    {
        var globalProperties = new Dictionary<string, string> { ["IsTestableProject"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo.Bar", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("PackageId")).IsEqualTo("");
        await Assert.That(evaluated.GetProperty("PublishRepositoryUrl")).IsEqualTo("");
    }

    [Test]
    public async Task Targets_XampleProject_SkipsPackageInformationEntirely()
    {
        var globalProperties = new Dictionary<string, string> { ["IsXampleProject"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo.Bar", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("PackageId")).IsEqualTo("");
    }

    [Test]
    public async Task Targets_PackageId_PreSetValue_IsPreserved()
    {
        var globalProperties = new Dictionary<string, string> { ["PackageId"] = "Custom.Package" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo.Bar", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("PackageId")).IsEqualTo("Custom.Package");
    }

    [Test]
    public async Task Targets_Company_NotPreSet_DefaultsToNetEvolve()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        await Assert.That(evaluated.GetProperty("Company")).IsEqualTo("NetEvolve");
    }

    [Test]
    public async Task Targets_Company_PreSetAndDifferentFromAuthors_IsPreserved()
    {
        var globalProperties = new Dictionary<string, string> { ["Company"] = "Acme" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("Company")).IsEqualTo("Acme");
    }

    [Test]
    public async Task Targets_Company_EqualToAuthors_IsOverriddenToNetEvolve()
    {
        var preSetProperties = new Dictionary<string, string> { ["Company"] = "SameValue", ["Authors"] = "SameValue" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], preSetProperties: preSetProperties);

        await Assert.That(evaluated.GetProperty("Company")).IsEqualTo("NetEvolve");
    }

    [Test]
    public async Task Targets_Authors_NotPreSet_DefaultsToNetEvolveAndSamtrion()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        await Assert.That(evaluated.GetProperty("Authors")).IsEqualTo("NetEvolve, samtrion");
    }

    [Test]
    public async Task Targets_Authors_EqualToAssemblyName_IsOverridden()
    {
        var preSetProperties = new Dictionary<string, string> { ["Authors"] = "Foo", ["AssemblyName"] = "Foo" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], preSetProperties: preSetProperties);

        await Assert.That(evaluated.GetProperty("Authors")).IsEqualTo("NetEvolve, samtrion");
    }

    [Test]
    public async Task Targets_Authors_DifferentFromAssemblyName_IsPreserved()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["Authors"] = "Custom Author",
            ["AssemblyName"] = "Something.Else",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("Authors")).IsEqualTo("Custom Author");
    }

    [Test]
    public async Task Targets_NoLicenseOrReadmeOnDisk_ItemGroupProducesNoPackageFiles()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        await Assert.That(evaluated.GetItemIncludes("None")).IsEmpty();
    }
}
