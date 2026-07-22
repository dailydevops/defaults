namespace NetEvolve.Defaults.Analyzer.Tests.Unit;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NetEvolve.Defaults.Analyzer.Diagnostics;

public class OldProjectSupportDiagnosticAnalyzerTests
{
    [Test]
    public async Task Analyze_NeitherKeySet_NoDiagnostics()
    {
        var globalOptions = new Dictionary<string, string?>();

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            new OldProjectSupportDiagnosticAnalyzer(),
            globalOptions
        );

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Analyze_OnlyDirEngineeringSet_ReportsSingleDiagnostic()
    {
        var globalOptions = new Dictionary<string, string?> { ["build_property.direngineering"] = "true" };

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            new OldProjectSupportDiagnosticAnalyzer(),
            globalOptions
        );

        await Assert.That(diagnostics).HasCount().EqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo("OLD0001");
    }

    [Test]
    public async Task Analyze_OnlyDirEngineeringSettingsSet_ReportsSingleDiagnostic()
    {
        var globalOptions = new Dictionary<string, string?> { ["build_property.direngineeringsettings"] = "true" };

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            new OldProjectSupportDiagnosticAnalyzer(),
            globalOptions
        );

        await Assert.That(diagnostics).HasCount().EqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo("OLD0001");
    }

    [Test]
    public async Task Analyze_DirEngineeringWhitespaceOnly_NoDiagnostics()
    {
        var globalOptions = new Dictionary<string, string?> { ["build_property.direngineering"] = "   " };

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            new OldProjectSupportDiagnosticAnalyzer(),
            globalOptions
        );

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Analyze_DirEngineeringSettingsWhitespaceOnly_NoDiagnostics()
    {
        var globalOptions = new Dictionary<string, string?> { ["build_property.direngineeringsettings"] = "   " };

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            new OldProjectSupportDiagnosticAnalyzer(),
            globalOptions
        );

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task BothKeysConfigured_ReportsSingleDiagnostic()
    {
        var globalOptions = new Dictionary<string, string?>
        {
            ["build_property.direngineering"] = "true",
            ["build_property.direngineeringsettings"] = "true",
        };

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(
            new OldProjectSupportDiagnosticAnalyzer(),
            globalOptions
        );

        await Assert.That(diagnostics).HasCount().EqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo("OLD0001");
    }

    [Test]
    public async Task SupportedDiagnostics_ReturnsSingleOLD0001Descriptor()
    {
        var analyzer = new OldProjectSupportDiagnosticAnalyzer();

        var supportedDiagnostics = analyzer.SupportedDiagnostics;

        await Assert.That(supportedDiagnostics).HasCount().EqualTo(1);
        await Assert.That(supportedDiagnostics[0].Id).IsEqualTo("OLD0001");
    }

    [Test]
    public async Task Initialize_ContextNull_ThrowsArgumentNullException()
    {
        var analyzer = new OldProjectSupportDiagnosticAnalyzer();

        await Assert.That(() => analyzer.Initialize(null!)).Throws<ArgumentNullException>();
    }
}
