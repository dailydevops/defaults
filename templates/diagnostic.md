# {{RULE_ID}}: {{SHORT_TITLE}}

## Overview

**Severity**: {{SEVERITY}}  
**Category**: {{CATEGORY}}  
**Applies To**: {{APPLIES_TO}}

## Description

{{DETAILED_DESCRIPTION}}

The purpose of this diagnostic is to ensure that {{SPECIFIC_PURPOSE}}.

## Impact

- {{IMPACT_POINT_1}}
- {{IMPACT_POINT_2}}
- {{IMPACT_POINT_3}}
- {{IMPACT_POINT_4}}

## Resolution

### Step 1: Open Your Project File

Locate and open your project file (`.csproj`, `.fsproj`, or `.vbproj`).

### Step 2: Add or Update {{PROPERTY_NAME}} Property

Ensure the `<{{PROPERTY_NAME}} />` property is present in your project's `<PropertyGroup>` section:

```xml
<PropertyGroup>
  <{{PROPERTY_NAME}}>{{EXPECTED_VALUE}}</{{PROPERTY_NAME}}>
  <!-- Additional properties -->
</PropertyGroup>
```

### Step 3: Validate and Rebuild

Validate your project file for syntax errors and rebuild your project:

```bash
dotnet build
```

## Best Practices

- {{BEST_PRACTICE_1}}
- {{BEST_PRACTICE_2}}
- {{BEST_PRACTICE_3}}
- {{BEST_PRACTICE_4}}

## Example

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <PackageId>Your.Package.Id</PackageId>
    <Title>Your Package Title</Title>
    <{{PROPERTY_NAME}}>{{EXAMPLE_VALUE}}</{{PROPERTY_NAME}}>
    <!-- Additional properties -->
  </PropertyGroup>
</Project>
```

## See Also

- [NuGet Package Metadata Reference](https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets)
- {{ADDITIONAL_REFERENCE_1}}
- {{ADDITIONAL_REFERENCE_2}}

---

## Template Instructions

Use the following placeholders when creating new diagnostic documentation:

| Placeholder | Description | Example |
|---|---|---|
| `{{RULE_ID}}` | Diagnostic rule identifier | NED0001, OLD0001 |
| `{{SHORT_TITLE}}` | Concise title describing the diagnostic | Missing Package ID Configuration |
| `{{SEVERITY}}` | Error, Warning, or Information | Error |
| `{{CATEGORY}}` | Diagnostic category | Package Metadata, Project Configuration |
| `{{APPLIES_TO}}` | Target project types | Packable Projects, All Projects |
| `{{DETAILED_DESCRIPTION}}` | Detailed explanation of what the diagnostic checks | The project file is missing... |
| `{{SPECIFIC_PURPOSE}}` | Purpose of the check | ...packages are properly identified... |
| `{{IMPACT_POINT_1..4}}` | Consequences of non-compliance | Users cannot find your package |
| `{{PROPERTY_NAME}}` | MSBuild property being validated | PackageId, Description, Authors |
| `{{EXPECTED_VALUE}}` | Example of correct value | Your.Company.PackageName |
| `{{BEST_PRACTICE_1..4}}` | Professional guidance | Use reverse domain notation... |
| `{{EXAMPLE_VALUE}}` | Real-world example value | NetEvolve.Defaults |
| `{{ADDITIONAL_REFERENCE_1..2}}` | Related documentation links | [Creating Discoverable Packages](...) |
