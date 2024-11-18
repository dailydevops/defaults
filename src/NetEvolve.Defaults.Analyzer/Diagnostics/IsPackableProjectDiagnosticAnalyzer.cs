namespace NetEvolve.Defaults.Analyzer.Diagnostics;

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
[ExcludeFromCodeCoverage]
internal sealed class IsPackableProjectDiagnosticAnalyzer : DiagnosticAnalyzer
{
    internal static readonly DiagnosticDescriptor _ruleNED0001 = Helper.CreateUsageDescriptor(
        "NED0001",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );
    internal static readonly DiagnosticDescriptor _ruleNED0002 = Helper.CreateUsageDescriptor(
        "NED0002",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );
    internal static readonly DiagnosticDescriptor _ruleNED0003 = Helper.CreateUsageDescriptor(
        "NED0003",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );
    internal static readonly DiagnosticDescriptor _ruleNED0004 = Helper.CreateUsageDescriptor(
        "NED0004",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );
    internal static readonly DiagnosticDescriptor _ruleNED0005 = Helper.CreateUsageDescriptor(
        "NED0005",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );
    internal static readonly DiagnosticDescriptor _ruleNED0006 = Helper.CreateUsageDescriptor(
        "NED0006",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );
    internal static readonly DiagnosticDescriptor _ruleNED0007 = Helper.CreateUsageDescriptor(
        "NED0007",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );
    internal static readonly DiagnosticDescriptor _ruleNED0008 = Helper.CreateUsageDescriptor(
        "NED0008",
        customTags: WellKnownDiagnosticTags.CompilationEnd
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [
            _ruleNED0001,
            _ruleNED0002,
            _ruleNED0003,
            _ruleNED0004,
            _ruleNED0005,
            _ruleNED0006,
            _ruleNED0007,
            _ruleNED0008,
        ];

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

            if (!IsPackable(options))
            {
                // If the project is not packable, we don't need to analyze it
                return;
            }

            AnalyzeIsKeySet(ctx, options, _ruleNED0001, "build_property.packageid");
            AnalyzeIsKeySet(ctx, options, _ruleNED0002, "build_property.title");
            AnalyzeIsKeySet(ctx, options, _ruleNED0003, "build_property.description");
            AnalyzeIsKeySet(ctx, options, _ruleNED0004, "build_property.packagetags");
            AnalyzeIsKeySet(ctx, options, _ruleNED0005, "build_property.packageprojecturl");
            AnalyzeIsKeySet(ctx, options, _ruleNED0006, "build_property.repositoryurl");
            AnalyzeIsKeySet(ctx, options, _ruleNED0007, "build_property.authors");
            AnalyzeIsKeySet(ctx, options, _ruleNED0008, "build_property.company");
        });
    }

    private static void AnalyzeIsKeySet(
        CompilationAnalysisContext context,
        AnalyzerConfigOptions options,
        DiagnosticDescriptor descriptor,
        string key
    )
    {
        if (KeyHasNoValidValue(options, key))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, null));
        }
    }

    private static bool KeyHasNoValidValue(AnalyzerConfigOptions options, string key)
    {
        var isConfigured = options.TryGetValue(key, out var configuredValue);

        return !isConfigured || string.IsNullOrWhiteSpace(configuredValue);
    }

    private static bool IsPackable(AnalyzerConfigOptions options) =>
        options.TryGetValue("build_property.ispackable", out var isPackableValue)
        && bool.TryParse(isPackableValue, out var isPackable)
        && isPackable;
}
