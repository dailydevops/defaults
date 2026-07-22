namespace NetEvolve.Defaults.Analyzer.Tests.Unit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NetEvolve.Defaults.Analyzer.Diagnostics;
using TUnit.Assertions.Enums;

public class IsPackableProjectDiagnosticAnalyzerTests
{
    private static Dictionary<string, string?> CreateValidOptions() =>
        new(StringComparer.Ordinal)
        {
            ["build_property.ispackable"] = "true",
            ["build_property.packageid"] = "Some.Package.Id",
            ["build_property.title"] = "Some Title",
            ["build_property.description"] = "Some Description",
            ["build_property.packagetags"] = "tag1;tag2",
            ["build_property.packageprojecturl"] = "https://example.com/project",
            ["build_property.repositoryurl"] = "https://example.com/repo",
            ["build_property.authors"] = "Some Author",
            ["build_property.company"] = "Some Company",
            ["build_property.copyrightyearstart"] = "2024",
        };

    [Test]
    public async Task Analyze_IsPackableNotSet_NoDiagnostics()
    {
        var options = new Dictionary<string, string?>(StringComparer.Ordinal);

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Analyze_IsPackableFalse_NoDiagnostics()
    {
        var options = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["build_property.ispackable"] = "false",
        };

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Analyze_IsPackableUnparseable_TreatedAsNotPackable_NoDiagnostics()
    {
        var options = new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["build_property.ispackable"] = "notabool",
        };

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Analyze_IsPackableTrue_AllKeysValid_NoDiagnostics()
    {
        var options = CreateValidOptions();

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    [Arguments("build_property.packageid", "NED0001")]
    [Arguments("build_property.title", "NED0002")]
    [Arguments("build_property.description", "NED0003")]
    [Arguments("build_property.packagetags", "NED0004")]
    [Arguments("build_property.packageprojecturl", "NED0005")]
    [Arguments("build_property.repositoryurl", "NED0006")]
    [Arguments("build_property.authors", "NED0007")]
    [Arguments("build_property.company", "NED0008")]
    public async Task Analyze_SingleKeyMissing_FiresExpectedDiagnostic(string key, string expectedId)
    {
        var options = CreateValidOptions();
        _ = options.Remove(key);

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics.Length).IsEqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo(expectedId);
    }

    [Test]
    [Arguments("build_property.packageid", "NED0001")]
    [Arguments("build_property.title", "NED0002")]
    [Arguments("build_property.description", "NED0003")]
    [Arguments("build_property.packagetags", "NED0004")]
    [Arguments("build_property.packageprojecturl", "NED0005")]
    [Arguments("build_property.repositoryurl", "NED0006")]
    [Arguments("build_property.authors", "NED0007")]
    [Arguments("build_property.company", "NED0008")]
    public async Task Analyze_SingleKeyWhitespaceOnly_FiresExpectedDiagnostic(string key, string expectedId)
    {
        var options = CreateValidOptions();
        options[key] = "   ";

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics.Length).IsEqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo(expectedId);
    }

    [Test]
    public async Task Analyze_CopyrightYearStart_Missing_FiresNed0009()
    {
        var options = CreateValidOptions();
        _ = options.Remove("build_property.copyrightyearstart");

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics.Length).IsEqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo("NED0009");
    }

    [Test]
    public async Task Analyze_CopyrightYearStart_NonNumeric_FiresNed0009()
    {
        var options = CreateValidOptions();
        options["build_property.copyrightyearstart"] = "abc";

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics.Length).IsEqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo("NED0009");
    }

    [Test]
    public async Task Analyze_CopyrightYearStart_BoundaryLow_1900_DoesNotFire()
    {
        var options = CreateValidOptions();
        options["build_property.copyrightyearstart"] = "1900";

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Analyze_CopyrightYearStart_BoundaryHigh_9999_DoesNotFire()
    {
        var options = CreateValidOptions();
        options["build_property.copyrightyearstart"] = "9999";

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Analyze_CopyrightYearStart_JustBelowLow_1899_Fires()
    {
        var options = CreateValidOptions();
        options["build_property.copyrightyearstart"] = "1899";

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics.Length).IsEqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo("NED0009");
    }

    [Test]
    public async Task Analyze_CopyrightYearStart_JustAboveHigh_10000_Fires()
    {
        var options = CreateValidOptions();
        options["build_property.copyrightyearstart"] = "10000";

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics.Length).IsEqualTo(1);
        await Assert.That(diagnostics[0].Id).IsEqualTo("NED0009");
    }

    [Test]
    public async Task Analyze_CopyrightYearStart_SurroundingWhitespace_TrimmedByTryParse_DoesNotFire()
    {
        var options = CreateValidOptions();
        options["build_property.copyrightyearstart"] = "  2024  ";

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics).IsEmpty();
    }

    [Test]
    public async Task Analyze_CopyrightYearStart_Missing_DefaultSeverityIsInfo()
    {
        var options = CreateValidOptions();
        _ = options.Remove("build_property.copyrightyearstart");

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        await Assert.That(diagnostics.Length).IsEqualTo(1);
        await Assert.That(diagnostics[0].Severity).IsEqualTo(DiagnosticSeverity.Info);
    }

    [Test]
    public async Task Analyze_MultipleKeysMissing_FiresOnlyCorrespondingDiagnostics()
    {
        var options = CreateValidOptions();
        _ = options.Remove("build_property.packageid");
        _ = options.Remove("build_property.title");

        var diagnostics = await AnalyzerTestHelper
            .GetDiagnosticsAsync(new IsPackableProjectDiagnosticAnalyzer(), options)
            .ConfigureAwait(false);

        var ids = diagnostics.Select(d => d.Id).OrderBy(id => id, StringComparer.Ordinal).ToArray();

        await Assert.That(ids).IsEquivalentTo(["NED0001", "NED0002"]);
    }

    [Test]
    public async Task SupportedDiagnostics_ReturnsExpectedDescriptorsInOrder()
    {
        var analyzer = new IsPackableProjectDiagnosticAnalyzer();

        var ids = analyzer.SupportedDiagnostics.Select(d => d.Id).ToArray();

        await Assert
            .That(ids)
            .IsEquivalentTo(
                ["NED0001", "NED0002", "NED0003", "NED0004", "NED0005", "NED0006", "NED0007", "NED0008", "NED0009"],
                CollectionOrdering.Matching
            );
    }

    [Test]
    public async Task Initialize_ContextNull_ThrowsArgumentNullException()
    {
        var analyzer = new IsPackableProjectDiagnosticAnalyzer();

        await Assert.That(() => analyzer.Initialize(null!)).Throws<ArgumentNullException>();
    }
}
