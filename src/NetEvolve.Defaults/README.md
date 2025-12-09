# NetEvolve.Defaults

NetEvolve.Defaults is a comprehensive build automation and configuration package that establishes consistent development standards across all NetEvolve projects. It provides standardized configuration files, build properties, analyzer settings, and development guidelines that enforce code quality, consistency, and best practices throughout your project lifecycle.

## Overview

This package encapsulates enterprise-grade configuration standards including:

- **Centralized Build Configuration**: Unified project settings and compilation options across multi-targeted projects
- **Code Analysis & Quality Enforcement**: Automated static analysis with Roslyn analyzers and code style enforcement
- **Language & Compiler Settings**: Modern C# language features with nullable reference types and implicit usings
- **Continuous Integration Support**: Automatic detection and optimization for CI/CD environments
- **NuGet Security Auditing**: Integrated vulnerability scanning for package dependencies
- **Editor Configuration**: Standardized coding styles and formatting rules (`.editorconfig`)
- **Documentation Generation**: Automatic XML documentation file generation for NuGet packages

## Installation

### Using the NuGet CLI

```bash
dotnet add package NetEvolve.Defaults
```

### Manual Package Reference

Add the following to your project file (`.csproj`, `.fsproj`, or `.vbproj`):

```xml
<PackageReference Include="NetEvolve.Defaults" Version="x.x.x" PrivateAssets="all" />
```

The `PrivateAssets="all"` attribute ensures that the defaults package configuration is only applied to your project and not propagated to consumers of your package.

## Project Configuration

NetEvolve.Defaults automatically configures your project based on its naming conventions and context. All settings can be customized by defining corresponding MSBuild properties in your project file or via the command line.

### Project Classification

The build system automatically categorizes projects based on their names:

#### Test Projects (`IsTestableProject`)

Projects containing `.Tests.` in their name are automatically identified as testable projects. These projects receive specialized configurations:

- Specific analyzer rules are relaxed for testing purposes
- Code coverage reporting is excluded for test projects
- Documentation generation can be disabled
- Warnings related to testing are suppressed

**Customization:**
```xml
<PropertyGroup>
  <IsTestableProject>true</IsTestableProject>
</PropertyGroup>
```

#### Example Projects (`IsXampleProject`)

Projects containing `.Xample` or starting with `Xample.` are identified as example/demonstration projects with similar relaxed configurations as test projects.

**Customization:**
```xml
<PropertyGroup>
  <IsXampleProject>true</IsXampleProject>
</PropertyGroup>
```

## Build & Compilation Settings

### Language and Runtime Configuration

| Property | Default | Description |
|----------|---------|-------------|
| `LangVersion` | `latest` | C# language version. Set to `preview` to use preview features. Automatically detected in CI environments. |
| `Nullable` | `enable` | Enables nullable reference types throughout the project. Highly recommended for C# 8.0+. |
| `ImplicitUsings` | `enable` | Automatically imports common namespaces (introduced in .NET 6). Reduces boilerplate code. |
| `WarningLevel` | `9999` | Enables all compiler warnings for maximum code quality. |
| `TreatWarningsAsErrors` | `true` (Release) | In Release builds, compiler warnings are treated as build errors. |
| `GenerateDocumentationFile` | `true` | Generates XML documentation file from code comments for API documentation. |
| `RootNamespace` | `$(MSBuildProjectName)` | Default root namespace. Matches the project name by default. |
| `AssemblyName` | `$(MSBuildProjectName)` | Assembly name. Matches the project name by default. |
| `NeutralLanguage` | `en` | Default language for satellite assemblies. English (en) is the standard. |

**Example:**
```xml
<PropertyGroup>
  <LangVersion>preview</LangVersion>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

### Code Analysis & Compilation

| Property | Default | Description |
|----------|---------|-------------|
| `AnalysisLevel` | `latest` | Roslyn analyzer version. Uses the latest available. |
| `AnalysisMode` | `All` | Analyzer mode: `All`, `Minimal`, or `Recommended`. `All` provides comprehensive analysis. |
| `EnableNETAnalyzers` | `true` | Enables built-in .NET analyzers for modern code pattern detection. |
| `CodeAnalysisTreatWarningsAsErrors` | `true` | Code analysis warnings are treated as errors in Release builds. |
| `EnforceCodeStyleInBuild` | `true` | Code style violations (EditorConfig rules) cause build failures. |
| `ReportAnalyzer` | `true` | Enables analyzer performance reporting during build. |
| `RunAnalyzersDuringBuild` | `true` | Analyzers run during build operations. |
| `RunAnalyzersDuringLiveAnalysis` | `true` | Analyzers run during IDE analysis for real-time feedback. |
| `Features` | `strict` | Enables strict mode for C# language features. |
| `ErrorLog` | `diagnostics-{ProjectName}-{TargetFramework}-{Configuration}.sarif` | SARIF format error log for analysis integration. |

**Example to customize warnings:**
```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);CA1810;CA1031</NoWarn>
</PropertyGroup>
```

**Example to suppress specific warnings in test projects:**
```xml
<ItemGroup>
  <CompilerVisibleProperty Include="IsTestableProject" />
</ItemGroup>
```

### Build Performance

| Property | Default | Description |
|----------|---------|-------------|
| `AccelerateBuildsInVisualStudio` | `true` | Optimizes Visual Studio build performance. |
| `Deterministic` | `true` (CI only) | Produces identical binaries from identical source across builds. Critical for reproducible builds. |

## Continuous Integration

NetEvolve.Defaults automatically detects CI/CD environments and applies optimization settings:

### Automatic CI Detection

The package detects execution in the following CI/CD platforms:

- **GitHub Actions** (`GITHUB_ACTIONS`)
- **Azure DevOps / TFS** (`TF_BUILD`)
- **GitLab CI** (`GITLAB_CI`)
- **Jenkins** (`JENKINS_URL`, `BUILD_ID`, `BUILD_URL`)
- **TeamCity** (`TEAMCITY_VERSION`)
- **AppVeyor** (`APPVEYOR`)
- **Travis CI** (`TRAVIS`)
- **CircleCI** (`CIRCLECI`)
- **AWS CodeBuild** (`CODEBUILD_BUILD_ID`, `AWS_REGION`)
- **Google Cloud Build** (`BUILD_ID`, `PROJECT_ID`)
- **JetBrains Space** (`JB_SPACE_API_URL`)
- **General CI** (`CI`)

### CI-Specific Settings

When CI is detected, the following settings are automatically applied:

| Property | Value | Purpose |
|----------|-------|---------|
| `IsContinuousIntegration` | `true` | Indicates CI environment execution. |
| `ContinuousIntegrationBuild` | `true` | Enables CI-specific optimizations in the .NET SDK. |
| `Deterministic` | `true` | Ensures reproducible builds across CI runs. |
| `RestorePackagesWithLockFile` | `false` | Allows dependency resolution flexibility in CI. |
| `TreatWarningsAsErrors` | `true` | All warnings must be resolved in CI builds. |

**Manual Override:**
```xml
<PropertyGroup>
  <IsContinuousIntegration>true</IsContinuousIntegration>
</PropertyGroup>
```

## NuGet Package Generation

### Package Information

For non-test projects built outside Visual Studio, the package is automatically generated during build:

| Property | Default | Description |
|----------|---------|-------------|
| `GeneratePackageOnBuild` | `true` | Automatically creates NuGet package during build (non-test, non-IDE builds). |

**Disable package generation:**
```xml
<PropertyGroup>
  <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
</PropertyGroup>
```

### Version Information

| Property | Default | Description |
|----------|---------|-------------|
| `Company` | (Not set by defaults) | Company name used in copyright and package metadata. |
| `CopyrightYearStart` | (Current year) | Starting year for copyright notice. Auto-calculated from current year. |
| `Copyright` | Auto-calculated | Copyright notice. Automatically generated as "Copyright @ {Company} {YearStart}" or "Copyright @ {Company} {YearStart} - {CurrentYear}" if years differ. |

**Example:**
```xml
<PropertyGroup>
  <Company>NetEvolve</Company>
  <CopyrightYearStart>2020</CopyrightYearStart>
</PropertyGroup>
```

## Security & Vulnerability Management

### NuGet Audit

NetEvolve.Defaults includes comprehensive NuGet vulnerability scanning:

| Property | Default | Description |
|----------|---------|-------------|
| `NuGetAudit` | `true` | Enables NuGet security audit checks during restore. |
| `NuGetAuditMode` | `all` | Audit scope: `all` (runtime + direct dependencies), `direct` (direct dependencies only), `transitively` (transitive dependencies). |
| `NuGetAuditLevel` | `low` | Minimum severity level: `low`, `moderate`, `high`, `critical`. |
| `WarningsAsErrors` | `NU1900-NU1904` (Release/CI) | Security warnings are treated as errors in Release and CI builds. |

**Custom configuration:**
```xml
<PropertyGroup>
  <NuGetAudit>true</NuGetAudit>
  <NuGetAuditMode>direct</NuGetAuditMode>
  <NuGetAuditLevel>high</NuGetAuditLevel>
</PropertyGroup>
```

## Platform & Framework Support

### Target Framework Configuration

| Property | Default | Description |
|----------|---------|-------------|
| `CheckEolTargetFramework` | `false` (Release) | Suppresses warnings about end-of-life target frameworks in Release builds. |
| `SuppressTfmSupportBuildWarnings` | `true` (Release) | Suppresses target framework support warnings in Release builds. |

## Advanced Settings

### Diagnostic Output

| Property | Default | Description |
|----------|---------|-------------|
| `ErrorLog` | `diagnostics-{ProjectName}-{TargetFramework}-{Configuration}.sarif` | Generates SARIF v2.1 format diagnostic reports for integration with analysis tools. |
| `ReportAnalyzer` | `true` | Reports analyzer performance metrics during build. |

**Example:**
```xml
<PropertyGroup>
  <ErrorLog>custom-diagnostics.sarif</ErrorLog>
</PropertyGroup>
```

### Preview Feature Support

NetEvolve.Defaults supports C# preview features:

```xml
<PropertyGroup>
  <LangVersion>preview</LangVersion>
</PropertyGroup>
```

## Suppressed Warnings

### Global Suppressions

The following warnings are globally suppressed as they conflict with modern C# practices:

- **CA1810**: Do not initialize static fields on type. (Initialization design pattern)
- **CA1031**: Do not catch general exception types. (Often necessary in middleware/handlers)

### Test Project Suppressions

Test projects additionally suppress:

- **CS8604**: Possible null reference argument
- **CA2007**: Consider calling ConfigureAwait on awaitable
- **CA1707**: Identifiers should not contain underscores (xUnit naming)
- **IDE1006**: Naming style (test method naming)
- **CA1822**: Mark members as static (test utilities)
- **And others** for testing-specific patterns

**Override suppressions:**
```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);CA1234</NoWarn>
</PropertyGroup>
```

### Non-Error Warnings

The following warnings are always treated as warnings, never errors:

- **CA5394**: Do not use insecure randomness
- **NU1701**: Package compatibility issues
- **CS0618**: Obsolete members (sometimes necessary for compatibility)
- **S1133**: Deprecated code patterns
- **NU1510**: Package not directly referenced

## Common Customization Scenarios

### Creating a Public Library

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <IncludeSymbols>true</IncludeSymbols>
  <SymbolPackageFormat>snupkg</SymbolPackageFormat>
  <Company>YourCompany</Company>
  <Description>Your package description</Description>
</PropertyGroup>
```

### Creating a Console Application

```xml
<PropertyGroup>
  <OutputType>Exe</OutputType>
  <IsTestableProject>false</IsTestableProject>
  <GenerateDocumentationFile>false</GenerateDocumentationFile>
</PropertyGroup>
```

### Strict Quality Enforcement

```xml
<PropertyGroup>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <NuGetAudit>true</NuGetAudit>
  <NuGetAuditLevel>low</NuGetAuditLevel>
</PropertyGroup>
```

### Relaxed Configuration (Development/Testing)

```xml
<PropertyGroup>
  <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  <CodeAnalysisTreatWarningsAsErrors>false</CodeAnalysisTreatWarningsAsErrors>
  <NuGetAudit>false</NuGetAudit>
</PropertyGroup>
```

## EditorConfig Integration

NetEvolve.Defaults includes a comprehensive `.editorconfig` file that enforces:

- Code formatting standards (indentation, spacing, line lengths)
- Naming conventions (PascalCase for types, camelCase for locals)
- Code style rules (var usage, expression preferences)
- Roslyn analyzer configuration

The `.editorconfig` is automatically deployed to your build output and is applied across all supported IDEs and editors.

## MSBuild Integration

All NetEvolve.Defaults settings are implemented as MSBuild properties and can be overridden at any level:

### Override Priority (highest to lowest)

1. Command-line properties: `dotnet build -p:PropertyName=Value`
2. Project file (`.csproj`): `<PropertyGroup>` elements
3. `Directory.Build.props`: Shared project settings
4. NetEvolve.Defaults package properties

**Example override via command line:**
```bash
dotnet build -p:TreatWarningsAsErrors=false
```

## Dependency Management

NetEvolve.Defaults requires:

- **.NET 5.0 or higher** for consumption as a package reference
- **C# 9.0 or higher** for projects using this package

No additional NuGet dependencies are introduced to your project.

## Support & Troubleshooting

### Disable Specific Features

Disable package information generation:
```xml
<PropertyGroup>
  <DisableSupportPackageInformation>true</DisableSupportPackageInformation>
</PropertyGroup>
```

### Diagnostic Reports

Build diagnostic reports are generated as SARIF files (one per project, target framework, and configuration) for integration with analysis tools and IDE extensions.

## Version History

Refer to the project's [CHANGELOG](https://github.com/dailydevops/defaults/blob/main/README.md) for version-specific changes and compatibility information.

## Contributing

We welcome contributions! Please ensure all code contributions:

- Follow the code style defined in `.editorconfig`
- Pass all code analysis checks
- Include comprehensive XML documentation
- Have appropriate test coverage