namespace NetEvolve.Defaults.Tests.Unit;

using System.Runtime.CompilerServices;
using Microsoft.Build.Locator;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        if (!MSBuildLocator.IsRegistered)
        {
            MSBuildLocator.RegisterDefaults();
        }
    }
}
