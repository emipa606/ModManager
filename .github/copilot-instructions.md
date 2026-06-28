# GitHub Copilot Instructions for Mod Manager (Continued) Mod Project

Welcome to the Mod Manager (Continued) mod project! This file provides guidance on the mod's structure, development, and how to effectively use GitHub Copilot to enhance your contributions.

## Mod Overview and Purpose

**Mod Name:** Mod Manager (Continued)  
**Author:** Update of Fluffy's mod  
**PackageId:** Mlie.ModManager

The Mod Manager (Continued) is a comprehensive tool designed to improve the way you handle mods in RimWorld, offering a more intuitive and feature-rich mod management experience. It aims to streamline the process of organizing, backing up, and manipulating mods, both from Steam and local copies.

## Key Features and Systems

- **Enhanced Mod Management Screen:** Provides distinct lists for available and active mods with drag-and-drop functionality and search filtering.
- **Mod Copy Creation:** Easily create local backups of Steam mods to ensure game stability during updates.
- **Mod List Backups:** Save and load mod lists, aiding in quick restoration of your preferred setups.
- **Steam Subscription Management:** Subscribe to missing workshop mods directly from the mod manager interface.
- **Mod and Mod List Coloring:** Customize mod visibility with individual and list-wide color settings.
- **Keyboard Navigation:** Enhanced keyboard controls for efficient mod browsing and management.
- **Mod Promotions:** Discover other mods by favorite authors effortlessly.

## Coding Patterns and Conventions

- **C# File Organization:** The mod organizes its features into logical components with utility classes like `Direction`, `Extensions`, and specialized controls like `FloatMenuOption_Aligned`. Familiarize yourself with these utilities for consistent development.
- **Naming Conventions:** Stick to PascalCase for type and method names while using camelCase for local variables.
- **Documentation:** Annotate methods and classes inline with XML documentation comments for clarity and maintainability.

## XML Integration

- **Manifest.xml:** Utilization of a Manifest file enables version, dependency, and incompatibility checks, enhancing the mod's functionality beyond what is available in vanilla.
- **About.xml and other XML files:** Ensure correct references and dependencies are maintained. The `About.xml` file currently depends on `brrainz.harmony`.

## Harmony Patching

- **Harmony Usage:** This mod utilizes Harmony (brrainz.harmony) for patching RimWorld's code. Make sure to follow Harmony's guidelines for safe and efficient patching. Study existing patches and structure new patches accordingly to avoid conflicts.

## Suggestions for Copilot

To make the most of GitHub Copilot in this mod project:

1. **Utilize Boilerplate Generation:** Use Copilot to quickly generate boilerplate code for new C# classes and methods.
2. **Leverage Documentation Assistance:** Let Copilot assist in writing XML documentation for your new methods and classes.
3. **Predictive Code Completion:** Use Copilot’s predictive capabilities to speed up writing repetitive tasks like property definitions or default constructors.
4. **Code Cleanup and Refactoring:** Rely on Copilot suggestions to refactor and enhance readability of complex logic.
5. **XML File Completion:** When adding new entries or dependencies in XML files, use Copilot to ensure syntax consistency and completeness.

Feel free to enhance this guide as the mod evolves to keep it up-to-date with the latest development practices and project changes. Happy modding!


This `.github/copilot-instructions.md` file provides a comprehensive overview of the mod project, while also outlining coding practices, XML integration, and specific guidance on using GitHub Copilot effectively within the modding workflow.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

