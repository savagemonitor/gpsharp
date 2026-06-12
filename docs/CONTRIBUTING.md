# Contributing to GpSharp

Thank you for your interest in contributing to the GpSharp SDK! We welcome improvements, bug reports, and new features.

## Development Workflow

1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd gpsharp
   ```

2. **Set Up Environment**:
   Ensure you have the .NET SDK installed. The project is multi-targeted for .NET Standard 2.1 and modern .NET versions.

3. **Implement Changes (TDD)**:
   We follow a strict TDD approach. Before writing any production code, write a failing test that defines the desired behavior.

4. **Run Tests**:
   Always run the full test suite before submitting changes:
   ```bash
   dotnet test
   ```

5. **Formatting**:
   We use `.editorconfig` to maintain a consistent coding style. Ensure your IDE is configured to respect these rules.

6. **Commit Messages**:
   Follow clear and descriptive commit messages (e.g., `feat: ...`, `fix: ...`, `test: ...`).

## Pull Requests

- All changes must be submitted via Pull Request.
- Ensure that all tests pass on the provided CI/CD pipeline.
- Provide a detailed description of your changes and any new features added.

## Code of Conduct

Please be respectful and professional in all interactions within the project.
