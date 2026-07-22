namespace NetEvolve.Defaults.Tests.Unit;

using System.Collections.Generic;

internal class SupportNuGetAuditTests
{
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportNuGetAudit.targets"
    );

    [Test]
    public async Task Defaults_AreSetWhenNotPreSet()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile]);

        await Assert.That(evaluated.GetProperty("NuGetAudit")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("NuGetAuditMode")).IsEqualTo("all");
        await Assert.That(evaluated.GetProperty("NuGetAuditLevel")).IsEqualTo("low");
    }

    [Test]
    public async Task PreSetValues_ArePreserved()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["NuGetAudit"] = "false",
            ["NuGetAuditMode"] = "direct",
            ["NuGetAuditLevel"] = "high",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("NuGetAudit")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("NuGetAuditMode")).IsEqualTo("direct");
        await Assert.That(evaluated.GetProperty("NuGetAuditLevel")).IsEqualTo("high");
    }

    [Test]
    public async Task Debug_NonCiBuild_DoesNotAppendAuditWarningsAsErrors()
    {
        var globalProperties = new Dictionary<string, string> { ["Configuration"] = "Debug" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("WarningsAsErrors")).IsEqualTo("");
    }

    [Test]
    public async Task Release_SetsAuditWarningsAsErrors()
    {
        var globalProperties = new Dictionary<string, string> { ["Configuration"] = "Release" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        var warningsAsErrors = evaluated.GetProperty("WarningsAsErrors");
        await Assert.That(warningsAsErrors).Contains("NU1900");
        await Assert.That(warningsAsErrors).Contains("NU1904");
    }

    [Test]
    public async Task ContinuousIntegrationBuild_SetsAuditWarningsAsErrors_EvenInDebug()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["Configuration"] = "Debug",
            ["ContinuousIntegrationBuild"] = "true",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        var warningsAsErrors = evaluated.GetProperty("WarningsAsErrors");
        await Assert.That(warningsAsErrors).Contains("NU1900");
    }

    [Test]
    public async Task Release_WarningsAsErrors_PreservesPriorValueAndAppendsAuditCodes()
    {
        var globalProperties = new Dictionary<string, string> { ["Configuration"] = "Release" };
        var preSetProperties = new Dictionary<string, string> { ["WarningsAsErrors"] = "CS1591" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties, preSetProperties);

        var warningsAsErrors = evaluated.GetProperty("WarningsAsErrors");

        await Assert.That(warningsAsErrors).IsEqualTo("CS1591;NU1900;NU1901;NU1902;NU1903;NU1904");
    }
}
