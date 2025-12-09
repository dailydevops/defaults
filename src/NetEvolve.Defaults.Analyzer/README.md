# NetEvolve.Defaults.Analyzer

NetEvolve.Defaults.Analyzer is a comprehensive diagnostic analyzer package that enforces consistent and high-quality NuGet package configurations across your projects. Built on the Roslyn compiler platform, it provides real-time analysis and validation of project file metadata, package properties, and build settings—ensuring that all packable projects meet enterprise-grade quality standards and best practices.

## Overview

This analyzer package is an integral part of the [NetEvolve.Defaults](https://github.com/dailydevops/defaults/tree/main/src/NetEvolve.Defaults) ecosystem and focuses on maintaining consistency and quality in NuGet package configurations. It automatically validates critical aspects of your projects including:

- **Package Metadata Validation**: Ensures all required package properties (PackageId, PackageVersion, PackageDescription, etc.) are properly configured
- **Repository Information**: Validates repository URL, issue tracker, and license metadata for discoverability
- **Author & Contact Details**: Enforces complete author information and contact guidelines for professional package metadata
- **Project Configuration Standards**: Verifies compliance with NetEvolve.Defaults configuration patterns and conventions
- **Build Target Verification**: Validates that deprecated build configurations are replaced with modern standards
- **Real-Time Diagnostics**: Provides immediate feedback during development with actionable error messages and fix suggestions

## Installation

### Using the .NET CLI

```bash
dotnet add package NetEvolve.Defaults.Analyzer
```

### Manual Package Reference

Add the following to your project file (`.csproj`, `.fsproj`, or `.vbproj`):

```xml
<PackageReference Include="NetEvolve.Defaults.Analyzer" Version="x.x.x" PrivateAssets="all" />
```

> **Note**: The `PrivateAssets="all"` attribute is recommended to prevent the analyzer from being included as a transitive dependency in projects that consume your package.

## Diagnostic Rules

The analyzer enforces the following diagnostic rules:

| Rule ID | Category | Severity | Description |
|---------|----------|----------|-------------|
| NED0001 | PackageMetadata | Error | Missing or incomplete `<PackageId>` tag configuration |
| NED0002 | PackageMetadata | Error | Missing or incomplete `<PackageDescription>` tag configuration |
| NED0003 | PackageMetadata | Error | Missing or incomplete package `<Authors>` configuration |
| NED0004 | PackageMetadata | Error | Missing or incomplete package license information |
| NED0005 | PackageMetadata | Error | Missing or incomplete repository URL configuration |
| NED0006 | PackageMetadata | Error | Missing or incomplete issue tracker URL configuration |
| NED0007 | ProjectConfiguration | Error | Missing or incomplete copyright notice configuration |
| NED0008 | ProjectConfiguration | Warning | Project does not follow recommended build configuration patterns |
| NED0009 | ProjectConfiguration | Warning | Missing recommended documentation XML file generation configuration |
| OLD0001 | DeprecatedConfiguration | Warning | Project uses deprecated NetEvolve build configuration patterns |

For detailed information about each rule, including remediation steps and examples, consult the [diagnostic documentation](https://github.com/dailydevops/defaults/tree/main/docs/usage).

## Recommended Configuration

### For NuGet Packages

To ensure optimal analyzer effectiveness for packable projects, configure your project file with:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    
    <!-- Package Metadata -->
    <PackageId>Your.Package.Namespace</PackageId>
    <PackageVersion>1.0.0</PackageVersion>
    <PackageDescription>Brief description of your package's purpose and functionality</PackageDescription>
    <Authors>Your Name;Other Contributors</Authors>
    <RepositoryUrl>https://github.com/owner/repo</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PackageProjectUrl>https://github.com/owner/repo</PackageProjectUrl>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <BugReportUrl>https://github.com/owner/repo/issues</BugReportUrl>
    <Copyright>Copyright (c) 2025 Your Organization. All rights reserved.</Copyright>
  </PropertyGroup>
</Project>
```

### For Non-Packable Projects

The analyzer intelligently handles non-packable projects (e.g., test projects, console applications) and provides context-appropriate diagnostics:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
</Project>
```

## Features

- **Zero Configuration**: Works out-of-the-box with sensible defaults; no additional setup required
- **Real-Time Feedback**: Integrated with Visual Studio and other Roslyn-compatible editors for instant diagnostics
- **Actionable Messages**: Every diagnostic includes clear descriptions and remediation guidance
- **Development Dependency**: Automatically excluded from package transitive dependencies
- **Performance Optimized**: Efficient compilation-end analysis minimizes build time impact
- **Multi-Targeting Support**: Works seamlessly with projects targeting multiple frameworks

## Integration with NetEvolve.Defaults

This analyzer is designed to complement the [NetEvolve.Defaults](https://github.com/dailydevops/defaults/tree/main/src/NetEvolve.Defaults) build configuration package. Together, they provide:

- Unified quality standards across your organization
- Automated enforcement of best practices and conventions
- Seamless integration with modern .NET development workflows
- Consistent NuGet package quality and discoverability

For complete project standardization, install both packages in your projects:

```bash
dotnet add package NetEvolve.Defaults
dotnet add package NetEvolve.Defaults.Analyzer
```

## Suppressing Diagnostics

If you need to suppress a specific diagnostic, use the standard Roslyn suppression mechanism:

```csharp
#pragma warning disable NED0001
// Code or configuration here
#pragma warning restore NED0001
```

Alternatively, add to your `.editorconfig`:

```ini
[*.csproj]
# Disable specific diagnostics for this project
dotnet_diagnostic.NED0001.severity = none
```

## Troubleshooting

### Analyzer not appearing in diagnostics

Ensure the package reference includes `PrivateAssets="all"`:

```xml
<PackageReference Include="NetEvolve.Defaults.Analyzer" Version="x.x.x" PrivateAssets="all" />
```

### False positives on non-packable projects

Verify that non-packable projects have `<IsPackable>false</IsPackable>` set in their project files.

### Build errors after installation

Clean and rebuild your solution:

```bash
dotnet clean
dotnet build
```

## Documentation

- [Diagnostic Rules Documentation](https://github.com/dailydevops/defaults/tree/main/docs/usage)
- [NetEvolve.Defaults Documentation](https://github.com/dailydevops/defaults/tree/main/src/NetEvolve.Defaults)
- [Microsoft Roslyn Analyzer Documentation](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview)

## License

This project is part of the NetEvolve ecosystem. Please refer to the [LICENSE](https://github.com/dailydevops/defaults/blob/main/LICENSE) file for licensing details.