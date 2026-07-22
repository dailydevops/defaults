namespace NetEvolve.Defaults.Analyzer.Tests.Unit;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

internal static class AnalyzerTestHelper
{
    private const string DefaultSource = "namespace Sample; internal sealed class Sample { }";

    internal static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        DiagnosticAnalyzer analyzer,
        IReadOnlyDictionary<string, string?>? globalOptions = null,
        string source = DefaultSource
    )
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            "AnalyzerTestAssembly",
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var optionsProvider = new TestAnalyzerConfigOptionsProvider(
            new TestAnalyzerConfigOptions(globalOptions ?? new Dictionary<string, string?>())
        );
        var analyzerOptions = new AnalyzerOptions([], optionsProvider);

        var withAnalyzers = compilation.WithAnalyzers([analyzer], analyzerOptions);

        var diagnostics = await withAnalyzers.GetAnalyzerDiagnosticsAsync();

        return diagnostics;
    }

    private sealed class TestAnalyzerConfigOptionsProvider(AnalyzerConfigOptions globalOptions)
        : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GlobalOptions { get; } = globalOptions;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => GlobalOptions;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => GlobalOptions;
    }

    private sealed class TestAnalyzerConfigOptions(IReadOnlyDictionary<string, string?> values) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value)
        {
            if (values.TryGetValue(key, out var found) && found is not null)
            {
                value = found;

                return true;
            }

            value = string.Empty;

            return false;
        }
    }
}
