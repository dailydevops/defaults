---
applyTo: '.github/copilot-instructions.md, .github/instructions/*.instructions.md'
description: 'This file contains instructions for wording and writing style. It includes guidelines for creating and modifying instruction files and Copilot instructions. Ensure to follow the practices outlined in this file to maintain consistency and clarity in documentation.'
---

# Wording and Writing Style Instructions

## General Style

* MUST use clear, concise, and direct language.
* MUST maintain a consistent formal, professional tone throughout all documentation.
* MUST use active voice rather than passive voice (e.g., "Use this pattern" instead of "This pattern should be used").
* MUST use present tense for instructions and guidelines.
* MUST format headings using Title Case.
* MUST format all documentation in proper Markdown.

## Directive Language

* MUST use modal verbs to indicate requirements clearly:
  - **MUST**: Required, mandatory action or rule
  - **SHOULD**: Recommended action but not mandatory
  - **MAY**: Optional action
  - **MUST NOT** or **MUST NEVER**: Prohibited action
* MUST place the directive at the beginning of the bullet point or sentence.
* MUST maintain consistent formatting of directives (all caps).

## Instruction Files

* MUST include a YAML frontmatter section with `applyTo` and `description` fields.
* MUST organize content under clear, hierarchical headings.
* MUST use bullet points for individual instructions within each section.
* MUST provide concrete examples where appropriate.
* MUST use code formatting (backticks) for code, file names, and technical terms.
* MUST include both general principles and specific implementation details.

## Copilot Instructions

* MUST structure guidance as imperative statements.
* MUST group related instructions under appropriate section headings.
* MUST explicitly state constraints and limitations.
* MUST reference specific file paths, folders, or patterns when applicable.
* MUST include cross-references to related instruction files when appropriate.

## Examples and References

* MUST use code blocks with appropriate language specifiers for code examples.
* MUST use relative links when referencing other documents within the repository.
* SHOULD include brief explanatory comments for complex code examples.
* SHOULD provide references to external resources when applicable.

## Formatting Conventions

* MUST use a single blank line to separate paragraphs.
* MUST use a single blank line before and after lists, code blocks, and headings.
* MUST use consistent indentation (2 spaces) for nested list items.
* MUST use consistent bullet point symbols (* for primary bullets, - for secondary).
* MUST use proper nesting of headings (H1 for title, H2 for major sections, H3 for subsections).
