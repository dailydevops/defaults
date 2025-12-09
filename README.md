# NetEvolve.Defaults

NetEvolve.Defaults is a comprehensive enterprise-grade build automation and configuration platform that establishes consistent development standards across all NetEvolve projects. It provides unified build properties, standardized configuration files, integrated code analysis, and development guidelines that enforce quality, consistency, and best practices throughout your entire project ecosystem.

## Overview

This repository contains a complete infrastructure for maintaining code quality and consistency across distributed teams and projects. It encompasses:

### Core Components

- **[NetEvolve.Defaults](https://www.nuget.org/packages/NetEvolve.Defaults)**: The foundational build configuration package providing MSBuild properties, targets, and shared settings
- **[NetEvolve.Defaults.Analyzer](https://www.nuget.org/packages/NetEvolve.Defaults.Analyzer)**: A comprehensive Roslyn-based diagnostic analyzer enforcing NuGet package quality standards
- **Build & Configuration Files**: Centralized editor configuration, version management, and continuous integration settings

### Key Features

- **Centralized Build Configuration**: Unified project settings and compilation options for single and multi-targeted projects
- **Code Analysis & Quality Enforcement**: Automated static analysis with Roslyn analyzers and code style enforcement via `.editorconfig`
- **Modern Language Features**: C# 13 language features with nullable reference types, implicit usings, and file-scoped namespaces
- **Continuous Integration Support**: Automatic detection and optimization for CI/CD environments with GitVersion integration
- **NuGet Security Auditing**: Integrated vulnerability scanning for package dependencies via NuGet Audit
- **Semantic Versioning**: Automated semantic versioning powered by GitVersion for consistent version management
- **Package Quality Standards**: Enforced NuGet metadata validation ensuring professional package documentation and discoverability
- **Multi-Framework Support**: Seamless configuration for projects targeting multiple .NET frameworks

## Quick Start

### Installation

Install the core package in your project:

```bash
dotnet add package NetEvolve.Defaults
```

For enhanced package quality validation, also add the analyzer:

```bash
dotnet add package NetEvolve.Defaults.Analyzer
```

### Manual Configuration

Add to your project file (`.csproj`, `.fsproj`, or `.vbproj`):

```xml
<ItemGroup>
  <PackageReference Include="NetEvolve.Defaults" Version="x.x.x" PrivateAssets="all" />
  <PackageReference Include="NetEvolve.Defaults.Analyzer" Version="x.x.x" PrivateAssets="all" />
</ItemGroup>
```

The `PrivateAssets="all"` attribute ensures that these build utilities are only applied to your project and not propagated to consumers of your packages.

## Repository Structure

```
defaults/
├── src/
│   ├── NetEvolve.Defaults/           # Core build configuration package
│   └── NetEvolve.Defaults.Analyzer/  # NuGet package quality analyzer
├── tests/
│   └── NetEvolve.Defaults.Tests.Integration/  # Integration tests
├── docs/
│   └── usage/                        # Diagnostic rule documentation
├── decisions/                        # Architectural decision records
├── .github/                          # GitHub workflows and instructions
├── Directory.Packages.props          # Centralized NuGet version management
├── Directory.Build.props             # Global MSBuild properties
├── Directory.Build.targets           # Global MSBuild targets
└── GitVersion.yml                    # Semantic versioning configuration
```

## Included Packages

### NetEvolve.Defaults

The foundational package providing standardized build settings:

- **MSBuild Properties & Targets**: Consistent compilation configuration across all projects
- **Editor Configuration**: Standardized code style rules via `.editorconfig`
- **Analyzer Settings**: Pre-configured Roslyn analyzer rules matching enterprise standards
- **NuGet Audit Integration**: Automated security vulnerability scanning
- **Language Configuration**: Modern C# language features with sensible defaults

[Learn more about NetEvolve.Defaults →](https://www.nuget.org/packages/NetEvolve.Defaults)

### NetEvolve.Defaults.Analyzer

Comprehensive diagnostic analyzer for NuGet package quality:

- **Package Metadata Validation**: Ensures complete and professional package information
- **Repository Configuration**: Validates repository URLs, issue trackers, and licensing
- **Build Standards Enforcement**: Verifies compliance with modern build patterns
- **Real-Time Diagnostics**: Immediate feedback during development with actionable fixes
- **9+ Diagnostic Rules**: Covering critical package configuration aspects

[Learn more about NetEvolve.Defaults.Analyzer →](https://www.nuget.org/packages/NetEvolve.Defaults.Analyzer)

## Diagnostic Rules

The analyzer package enforces standardized quality checks via diagnostic rules. For detailed documentation on all rules and remediation steps:

- [NED0001-NED0009 Rules Documentation](https://github.com/dailydevops/defaults/tree/main/docs/usage)
- [Diagnostic Configuration Guide](https://github.com/dailydevops/defaults/tree/main/src/NetEvolve.Defaults.Analyzer#diagnostic-rules)

## Configuration & Customization

All settings can be customized through:

1. **Project File Properties**: Override specific settings in individual `.csproj` files
2. **Directory.Build.props**: Apply settings across all projects in a solution
3. **.editorconfig**: Customize code style and formatting rules
4. **.github/instructions**: Apply project-specific guidelines and conventions

### Common Customizations

**Enable Preview Language Features:**
```xml
<PropertyGroup>
  <LangVersion>preview</LangVersion>
</PropertyGroup>
```

**Disable Nullable Reference Types (Not Recommended):**
```xml
<PropertyGroup>
  <Nullable>disable</Nullable>
</PropertyGroup>
```

**Customize Target Frameworks:**
```xml
<PropertyGroup>
  <TargetFrameworks>net8.0;net9.0</TargetFrameworks>
</PropertyGroup>
```

## Best Practices

1. **Use PrivateAssets="all"**: Ensures build tools don't leak into package consumers
2. **Enable Nullable Reference Types**: Improves type safety and reduces null reference exceptions
3. **Use Implicit Usings**: Reduces boilerplate and improves code readability
4. **Configure Complete Metadata**: Ensure all NuGet package properties are set for discoverability
5. **Run Analyzers Regularly**: Address diagnostics during development, not at release time
6. **Test Multi-Framework Projects**: Verify code works across all target frameworks

## Architectural Decisions

This project follows architectural principles documented in the [decisions/](https://github.com/dailydevops/defaults/tree/main/decisions) folder. Key decision areas include:

- **Folder Structure & Naming Conventions**: Consistent project organization
- **Centralized Package Management**: Single source of truth for NuGet versions
- **English as Project Language**: All code, documentation, and comments in English
- **.NET 10 & C# 13 Adoption**: Modern language features and runtime support
- **Conventional Commits**: Structured commit messages for automated versioning
- **GitVersion Integration**: Automated semantic versioning from git history

For more details, see [Architectural Decisions](https://github.com/dailydevops/defaults/tree/main/decisions).

## Contributing

Contributions are welcome! When contributing to this project:

1. Follow the existing code style and conventions
2. Ensure all diagnostic analyzers pass without warnings
3. Update relevant documentation
4. Follow [Conventional Commits](https://www.conventionalcommits.org/) format for commit messages
5. Add tests for new functionality

## Support

For issues, feature requests, or questions:

- [GitHub Issues](https://github.com/dailydevops/defaults/issues)
- [GitHub Discussions](https://github.com/dailydevops/defaults/discussions)

## Documentation

- [NetEvolve.Defaults Documentation](https://www.nuget.org/packages/NetEvolve.Defaults)
- [NetEvolve.Defaults.Analyzer Documentation](https://www.nuget.org/packages/NetEvolve.Defaults.Analyzer)
- [Diagnostic Rules Reference](https://github.com/dailydevops/defaults/tree/main/docs/usage)
- [Architectural Decisions](https://github.com/dailydevops/defaults/tree/main/decisions)

## License

This project is licensed under the [MIT License](https://github.com/dailydevops/defaults/blob/main/LICENSE).