namespace NetEvolve.Defaults.Analyzer.Diagnostics;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[ExcludeFromCodeCoverage]
internal sealed class OldProjectSupportDiagnosticAnalyzer : DiagnosticAnalyzer
{
    internal static readonly DiagnosticDescriptor _ruleOLD0001 = Helper.CreateUsageDescriptor(
        "OLD0001",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [_ruleOLD0001];

    public override void Initialize(AnalysisContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        // prefer single thread for debugging in development
        if (!Debugger.IsAttached)
        {
            context.EnableConcurrentExecution();
        }

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationAction(ctx =>
        {
            var options = ctx.Options.AnalyzerConfigOptionsProvider.GlobalOptions;

            if (
                IsConfigured(options, "build_property.direngineering")
                || IsConfigured(options, "build_property.direngineeringsettings")
            )
            {
                ctx.ReportDiagnostic(Diagnostic.Create(_ruleOLD0001, null));
            }
        });
    }

    private static bool IsConfigured(AnalyzerConfigOptions options, string key)
    {
        var isConfigured = options.TryGetValue(key, out var configuredValue);

        return isConfigured && !string.IsNullOrWhiteSpace(configuredValue);
    }
}
