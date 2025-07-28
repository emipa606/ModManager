# GitHub Copilot Instructions for RimWorld Modding Project

## Mod Overview and Purpose

This RimWorld modding project is a comprehensive mod manager tool for RimWorld, designed to improve and streamline the game’s modding experience. The primary purpose of this project is to provide players with robust tools for managing, installing, and customizing mods within RimWorld, enhancing both the ease of use and the overall gameplay experience.

## Key Features and Systems

- **Colour Picker**: A custom UI for selecting colors, allowing users to choose and preview color themes using a dialog interface.
- **Mod Management**: Provides interfaces for listing, importing, exporting, and configuring mods, including features to rename mod lists and handle version control.
- **Harmony Patching Support**: Incorporates Harmony patches to modify the game’s runtime behavior without altering the original game code.
- **Dependency Management**: Tools for checking and handling mod dependencies, ensuring compatibility and load order correctness.
- **User Interface Enhancements**: Additional UI components such as floating menus, selectable options, and customizable elements for a seamless integration experience.

## Coding Patterns and Conventions

- **Classes and Methods**: The project uses clear class naming conventions and public/private method organization to maintain a clean code structure. Each class file typically defines a single class and related methods.
  
- **Static Classes and Methods**: Where appropriate, static classes and methods are used, especially for utility and helper functions, ensuring efficient resource handling.

- **XML Integration**: XML files are employed for storing mod configurations, allowing for external modification and easy serialization/deserialization of data.

- **Use of Interfaces**: The implementation of interfaces like `IExposable` and `IRenameable` facilitates modular design and extends the mod's capabilities for saving and renaming functionalities.

## XML Integration

- This project effectively leverages XML for configuration management, using it to maintain mod data and dependency information.
  
- **Loading XML Data**: Methods like `LoadDataFromXmlCustom(XmlNode root)` are employed for custom XML data loading, ensuring compatibility and the ability to incorporate unique mod features from XML nodes.

## Harmony Patching

- Harmony is used to apply runtime modifications by creating patches to the existing game code. This ensures modular and conflict-free changes.

- **Patch Example**: Use simple C# classes to define pre/post-fix methods which Harmony recognizes for patching purposes. Typical patches might modify game behavior or enhance functionality without affecting the core system negatively.

## Suggestions for Copilot

- **Class Templates**: Use Copilot to generate class and method templates, especially for repetitive patterns like user interface components or Harmony patches.
  
- **XML Parsing Helpers**: Leverage Copilot's suggestions to create XML parsing and handling functions, reducing manual coding errors and increasing efficiency.

- **Utility Functions**: Encourage Copilot to suggest utility function enhancements and additional methods for commonly needed operations, such as color conversions or UI rendering functions.

- **Test Case Generation**: Utilize Copilot’s ability to assist in creating test cases to cover various mod scenarios and ensure robust functionality.

By following these instructions and practices, developers can effectively use GitHub Copilot to enhance their workflow in modding projects for RimWorld, leading to improved mod quality and user experience.
