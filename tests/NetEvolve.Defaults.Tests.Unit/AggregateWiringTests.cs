namespace NetEvolve.Defaults.Tests.Unit;

using System.Collections.Generic;

/// <summary>
/// Exercises the full real-world import chain (build/NetEvolve.Defaults.props|targets -&gt;
/// buildMultiTargeting/NetEvolve.Defaults.props|targets -&gt; Support*.props|targets) the way a consuming
/// project actually references this package, to make sure the individual Support* files documented and
/// tested in isolation elsewhere are wired together correctly.
/// </summary>
internal class AggregateWiringTests
{
    private static readonly string BuildProps = MSBuildProjectFixture.InBuild("NetEvolve.Defaults.props");
    private static readonly string BuildTargets = MSBuildProjectFixture.InBuild("NetEvolve.Defaults.targets");

    [Test]
    public async Task RegularProject_EndToEnd_IsPackableAndNotATestProject()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [BuildProps, BuildTargets]);

        await Assert.That(evaluated.GetProperty("IsTestableProject")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsXampleProject")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("WarningLevel")).IsEqualTo("9999");
        await Assert.That(evaluated.GetProperty("Copyright")).Contains("Copyright @");
        await Assert.That(evaluated.GetProperty("PackageId")).IsEqualTo("Foo");
    }

    [Test]
    public async Task TestProject_EndToEnd_IsNotPackableAndIsATestProject()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo.Tests.Unit", [BuildProps, BuildTargets]);

        await Assert.That(evaluated.GetProperty("IsTestableProject")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("NoWarn")).Contains("CS8604");
        await Assert.That(evaluated.GetProperty("PackageId")).IsEqualTo("");
    }

    [Test]
    public async Task XampleProject_EndToEnd_IsNotPackableAndNotATestProject()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Xample.Foo", [BuildProps, BuildTargets]);

        await Assert.That(evaluated.GetProperty("IsXampleProject")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("IsPackable")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("IsTestProject")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("WarnOnPackingNonPackableProject")).IsEqualTo("false");
    }

    [Test]
    public async Task DisableSupportPackageInformation_SkipsPackageInformationImports()
    {
        var globalProperties = new Dictionary<string, string> { ["DisableSupportPackageInformation"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [BuildProps, BuildTargets], globalProperties);

        await Assert.That(evaluated.GetProperty("PackageId")).IsEqualTo("");
        await Assert.That(evaluated.GetProperty("GeneratePackageOnBuild")).IsEqualTo("");
    }

    [Test]
    public async Task CiEnvironment_EndToEnd_ForcesDeterministicBuildProperties()
    {
        var globalProperties = new Dictionary<string, string> { ["CI"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [BuildProps, BuildTargets], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("Deterministic")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("ContinuousIntegrationBuild")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("RestorePackagesWithLockFile")).IsEqualTo("false");
    }
}
