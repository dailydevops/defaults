namespace NetEvolve.Defaults.Tests.Unit;

using System;
using System.Collections.Generic;
using System.Globalization;

internal class SupportCopyrightTests
{
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportCopyright.targets"
    );

    [Test]
    public async Task Copyright_NotPreSet_SameStartAndCurrentYear_HasNoYearRange()
    {
        var currentYear = DateTime.UtcNow.Year.ToString(CultureInfo.InvariantCulture);
        var globalProperties = new Dictionary<string, string>
        {
            ["Company"] = "NetEvolve",
            ["CopyrightYearStart"] = currentYear,
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("Copyright")).IsEqualTo($"Copyright @ NetEvolve {currentYear}");
    }

    [Test]
    public async Task Copyright_NotPreSet_StartYearInPast_HasYearRangeSuffix()
    {
        var currentYear = DateTime.UtcNow.Year;
        var globalProperties = new Dictionary<string, string>
        {
            ["Company"] = "NetEvolve",
            ["CopyrightYearStart"] = "2020",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        var expected =
            currentYear > 2020
                ? $"Copyright @ NetEvolve 2020 - {currentYear.ToString(CultureInfo.InvariantCulture)}"
                : "Copyright @ NetEvolve 2020";
        await Assert.That(evaluated.GetProperty("Copyright")).IsEqualTo(expected);
    }

    [Test]
    public async Task Copyright_PreSetValue_IsPreserved()
    {
        var globalProperties = new Dictionary<string, string> { ["Copyright"] = "Copyright @ Custom 1999" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("Copyright")).IsEqualTo("Copyright @ Custom 1999");
    }

    [Test]
    public async Task Copyright_NoCopyrightYearStartPreSet_DefaultsToCurrentYear()
    {
        var currentYear = DateTime.UtcNow.Year.ToString(CultureInfo.InvariantCulture);
        var globalProperties = new Dictionary<string, string> { ["Company"] = "NetEvolve" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("Copyright")).IsEqualTo($"Copyright @ NetEvolve {currentYear}");
    }
}
