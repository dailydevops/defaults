namespace NetEvolve.Defaults.Analyzer.Tests.Unit;

using System;
using System.Globalization;
using Microsoft.CodeAnalysis;

internal class HelperTests
{
    [Test]
    public async Task CreateUsageDescriptor_NullId_ThrowsArgumentNullException() =>
        await Assert.That(() => Helper.CreateUsageDescriptor(null!)).Throws<ArgumentNullException>();

    [Test]
    public async Task CreateUsageDescriptor_EmptyId_ThrowsArgumentNullException() =>
        await Assert.That(() => Helper.CreateUsageDescriptor(string.Empty)).Throws<ArgumentNullException>();

    [Test]
    public async Task CreateUsageDescriptor_WhitespaceId_ThrowsArgumentNullException() =>
        await Assert.That(() => Helper.CreateUsageDescriptor("   ")).Throws<ArgumentNullException>();

    [Test]
    public async Task CreateUsageDescriptor_ValidId_ReturnsExpectedDescriptor()
    {
        var descriptor = Helper.CreateUsageDescriptor("NED0001");

        await Assert.That(descriptor.Id).IsEqualTo("NED0001");
        await Assert.That(descriptor.Category).IsEqualTo(RuleCategory.Usage.ToString());
        await Assert.That(descriptor.DefaultSeverity).IsEqualTo(DiagnosticSeverity.Warning);
        await Assert.That(descriptor.IsEnabledByDefault).IsTrue();
        await Assert.That(descriptor.Title.ToString(CultureInfo.InvariantCulture)).IsNotEmpty();
        await Assert.That(descriptor.MessageFormat.ToString(CultureInfo.InvariantCulture)).IsNotEmpty();
        await Assert.That(descriptor.Description.ToString(CultureInfo.InvariantCulture)).IsNotEmpty();
    }

    [Test]
    public async Task CreateUsageDescriptor_CustomSeverity_PropagatesSeverity()
    {
        var descriptor = Helper.CreateUsageDescriptor("NED0009", DiagnosticSeverity.Info);

        await Assert.That(descriptor.DefaultSeverity).IsEqualTo(DiagnosticSeverity.Info);
    }

    [Test]
    public async Task CreateUsageDescriptor_IsEnabledByDefaultFalse_PropagatesValue()
    {
        var descriptor = Helper.CreateUsageDescriptor("NED0001", DiagnosticSeverity.Warning, isEnabledByDefault: false);

        await Assert.That(descriptor.IsEnabledByDefault).IsFalse();
    }

    [Test]
    public async Task CreateUsageDescriptor_CustomTags_PropagatesAllTags()
    {
        var descriptor = Helper.CreateUsageDescriptor(
            "OLD0001",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            "compilationEnd",
            "custom"
        );

        await Assert.That(descriptor.CustomTags).IsEquivalentTo(["compilationEnd", "custom"]);
    }

    [Test]
    public async Task CreateUsageDescriptor_HelpLinkUri_IsLowercasedAndFormatted()
    {
        var descriptor = Helper.CreateUsageDescriptor("NED0001");

        await Assert
            .That(descriptor.HelpLinkUri)
            .IsEqualTo("https://github.com/dailydevops/defaults/blob/main/docs/usage/ned0001.md");
    }

    [Test]
    public async Task CreateUsageDescriptor_HelpLinkUri_MixedCaseId_IsFullyLowercased()
    {
        var descriptor = Helper.CreateUsageDescriptor("NeD0002");

        await Assert
            .That(descriptor.HelpLinkUri)
            .IsEqualTo("https://github.com/dailydevops/defaults/blob/main/docs/usage/ned0002.md");
    }

    [Test]
    [Arguments(RuleCategory.Design, (byte)0)]
    [Arguments(RuleCategory.Naming, (byte)1)]
    [Arguments(RuleCategory.Style, (byte)2)]
    [Arguments(RuleCategory.Usage, (byte)3)]
    [Arguments(RuleCategory.Performance, (byte)4)]
    [Arguments(RuleCategory.Security, (byte)5)]
    public async Task RuleCategory_UnderlyingValues_MatchExpected(RuleCategory category, byte expected) =>
        await Assert.That((byte)category).IsEqualTo(expected);

    [Test]
    public async Task RuleCategory_HasExactlySixMembers()
    {
        await Assert.That(Enum.GetNames<RuleCategory>()).Count().IsEqualTo(6);
        await Assert.That(Enum.GetValues<RuleCategory>()).Count().IsEqualTo(6);
    }
}
