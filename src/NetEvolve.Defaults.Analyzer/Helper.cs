namespace NetEvolve.Defaults.Analyzer;

using System;
using Microsoft.CodeAnalysis;

internal static class Helper
{
    internal static DiagnosticDescriptor CreateUsageDescriptor(
        string id,
        DiagnosticSeverity defaultSeverity = DiagnosticSeverity.Warning,
        bool isEnabledByDefault = true,
        params string[] customTags
    ) => CreateDescriptor(id, RuleCategory.Usage, defaultSeverity, isEnabledByDefault, customTags);

    private static DiagnosticDescriptor CreateDescriptor(
        string id,
        RuleCategory category,
        DiagnosticSeverity defaultSeverity,
        bool isEnabledByDefault,
        params string[] customTags
    )
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        var rm = Strings.ResourceManager;

        return new DiagnosticDescriptor(
            id,
            new LocalizableResourceString($"{id}Title", rm, typeof(Strings)),
            new LocalizableResourceString($"{id}Message", rm, typeof(Strings)),
            category.ToString(),
            defaultSeverity,
            isEnabledByDefault,
            new LocalizableResourceString($"{id}Description", rm, typeof(Strings)),
            GetLink(category, id),
            customTags
        );
    }

#pragma warning disable CA1308 // Normalize strings to uppercase
    private static string GetLink(RuleCategory category, string? id) =>
        $"https://github.com/dailydevops/defaults/blob/main/docs/{category}/{id}.md".ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
}
