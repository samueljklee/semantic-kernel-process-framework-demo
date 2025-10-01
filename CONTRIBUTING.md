# Contributing to Semantic Kernel Process Framework Demo

Thank you for your interest in contributing! This document provides guidelines for contributing to the project, including documentation improvements, code contributions, and examples.

## Table of Contents

- [Getting Started](#getting-started)
- [Contributing Documentation](#contributing-documentation)
- [Contributing Code](#contributing-code)
- [Contributing Examples](#contributing-examples)
- [Style Guidelines](#style-guidelines)
- [Submitting Changes](#submitting-changes)

## Getting Started

### Prerequisites for Contributing

- .NET 9.0 SDK installed
- Git for version control
- A GitHub account
- OpenAI API key (for testing AI-related changes)
- Basic understanding of Markdown (for documentation)

### Setting Up Development Environment

1. **Fork and Clone**
   ```bash
   # Fork the repository on GitHub first, then:
   git clone https://github.com/YOUR_USERNAME/semantic-kernel-process-framework-demo.git
   cd semantic-kernel-process-framework-demo
   ```

2. **Set Up Environment Variables**
   ```bash
   export OPENAI_API_KEY="your-api-key"
   export GITHUB_TOKEN="your-github-token"  # If working on GitHub integration
   ```

3. **Build and Test**
   ```bash
   dotnet restore
   dotnet build
   dotnet run  # Verify everything works
   ```

## Contributing Documentation

Documentation improvements are always welcome! Here's how to contribute:

### Types of Documentation Contributions

1. **README Improvements**
   - Clarify existing sections
   - Add missing information
   - Fix typos or errors
   - Improve examples

2. **Example Walkthroughs**
   - Add new use case examples
   - Improve existing walkthroughs
   - Add screenshots or diagrams
   - Document edge cases

3. **Architecture Documentation**
   - Explain design decisions
   - Document new patterns
   - Add flow diagrams
   - Clarify complex concepts

4. **Troubleshooting Guides**
   - Document new issues and solutions
   - Improve existing solutions
   - Add platform-specific guidance

### Documentation Style Guide

#### Markdown Formatting

- Use clear, descriptive headers
- Use code blocks with language specification:
  ````markdown
  ```csharp
  // Your code here
  ```
  ````

- Use tables for comparisons
- Use lists for sequential steps
- Use blockquotes for important notes:
  ```markdown
  > **Note**: Important information here
  ```

#### Writing Style

- **Be Clear**: Use simple, direct language
- **Be Concise**: Get to the point quickly
- **Be Consistent**: Follow existing documentation patterns
- **Be Helpful**: Anticipate reader questions

#### Code Examples

- Include complete, runnable examples
- Add comments to explain non-obvious code
- Show expected output
- Provide context for when to use

#### Screenshots and Diagrams

- Use descriptive alt text
- Keep file sizes reasonable (< 1MB)
- Use consistent styling
- Store in `docs/images/` directory

### Documentation Structure

```
semantic-kernel-process-framework-demo/
├── README.md                          # Main documentation
├── README_GitHub_Process.md           # GitHub-specific process docs
├── CONTRIBUTING.md                    # This file
├── docs/
│   ├── examples/                      # Example walkthroughs
│   │   ├── basic-documentation-process.md
│   │   ├── advanced-human-in-loop.md
│   │   └── [your-new-example].md     # Add new examples here
│   ├── diagrams/                      # Architecture and flow docs
│   │   ├── process-architecture.md
│   │   └── [your-diagrams].md        # Add diagrams here
│   └── images/                        # Screenshots and images
│       └── [your-images].png          # Store images here
```

### Adding New Documentation

#### Creating a New Example Walkthrough

1. **Choose a Clear Use Case**
   - Focus on a specific scenario
   - Ensure it's not already covered
   - Consider your target audience

2. **Use the Template**
   
   Create `docs/examples/your-example.md`:
   
   ```markdown
   # Example: [Your Example Title]
   
   Brief description of what this example demonstrates.
   
   ## What You'll Learn
   
   - Bullet points of learning objectives
   
   ## Prerequisites
   
   - Required setup
   
   ## Step-by-Step Walkthrough
   
   ### Step 1: [First Step]
   
   Instructions...
   
   **Expected Output:**
   ```
   Show what user should see
   ```
   
   ### Step 2: [Next Step]
   
   Continue...
   
   ## Key Takeaways
   
   - Summary points
   
   ## Troubleshooting
   
   Common issues specific to this example
   ```

3. **Test Your Example**
   - Follow your own instructions
   - Verify all commands work
   - Check output matches your documentation

4. **Link From Main README**
   - Add reference in appropriate section
   - Update table of contents if needed

#### Adding Architecture Documentation

1. **Create in `docs/diagrams/`**
2. **Include visual diagrams** (ASCII art, Mermaid, or images)
3. **Explain the "why"**, not just the "what"
4. **Use consistent terminology** with existing docs

#### Improving Troubleshooting Section

When adding troubleshooting entries:

1. **Use Clear Structure**:
   ```markdown
   #### Issue Number. Issue Title
   
   **Symptoms:**
   - What the user sees
   - Error messages
   
   **Solutions:**
   - Step-by-step fixes
   - Alternative approaches
   - Links to related docs
   ```

2. **Be Specific**:
   - Include exact error messages
   - Provide platform-specific solutions
   - Show commands to diagnose

3. **Verify Solutions**:
   - Test each solution yourself
   - Note which platforms were tested
   - Include version information if relevant

### Documentation Review Process

1. **Self-Review Checklist**:
   - [ ] Grammar and spelling checked
   - [ ] Code examples tested
   - [ ] Links verified
   - [ ] Formatting consistent
   - [ ] Screenshots current

2. **Request Review**:
   - Submit PR with clear description
   - Tag as documentation
   - Explain what problem you're solving

## Contributing Code

### Code Style Guidelines

- Follow C# coding conventions
- Use meaningful variable names
- Add XML comments for public APIs
- Keep methods focused and small

### Creating New Process Steps

1. **Inherit from Base Class**:
   ```csharp
   // Stateless step
   public class MyStep : KernelProcessStep
   {
       [KernelFunction]
       public void MyFunction(string input) { }
   }
   
   // Stateful step
   public class MyStatefulStep : KernelProcessStep<MyState>
   {
       public class MyState { /* state properties */ }
       
       [KernelFunction]
       public async Task MyFunction(Kernel kernel, string input) { }
   }
   ```

2. **Define Events Clearly**:
   ```csharp
   public static class OutputEvents
   {
       public const string MyEventName = nameof(MyEventName);
   }
   ```

3. **Document Your Step**:
   - XML comments on class
   - Explain purpose and behavior
   - Document events emitted

### Testing Your Changes

1. **Manual Testing**:
   ```bash
   dotnet build
   dotnet run
   # Test all affected workflows
   ```

2. **Test Edge Cases**:
   - Empty inputs
   - Very long inputs
   - Special characters
   - Error conditions

3. **Test Across Processes**:
   - Verify existing processes still work
   - Test new processes thoroughly

## Contributing Examples

### Adding Process Workflow Examples

1. **Design the Workflow**:
   - Clear entry and exit points
   - Well-defined steps
   - Appropriate error handling

2. **Implement the Steps**:
   - Create step classes
   - Wire events properly
   - Add to Program.cs menu

3. **Document the Process**:
   - Create example walkthrough
   - Update architecture docs
   - Add to main README

### Sharing Configuration Examples

- Include complete working examples
- Explain configuration options
- Document environment requirements

## Style Guidelines

### Code Comments

```csharp
/// <summary>
/// Brief description of what this does
/// </summary>
/// <param name="input">Description of parameter</param>
/// <returns>Description of return value</returns>
[KernelFunction]
public string MyFunction(string input)
{
    // Implementation comments only for complex logic
    return ProcessInput(input);
}
```

### Commit Messages

Use clear, descriptive commit messages:

```
Good:
- "Add troubleshooting section for GitHub token errors"
- "Fix typo in basic documentation process example"
- "Improve error handling in ValidateIssueInputStep"

Avoid:
- "Update README"
- "Fix stuff"
- "Changes"
```

### Branch Naming

Use descriptive branch names:

```
docs/add-troubleshooting-guide
feature/add-email-notification-step
fix/github-api-rate-limiting
```

## Submitting Changes

### Pull Request Process

1. **Create Feature Branch**:
   ```bash
   git checkout -b docs/your-improvement
   ```

2. **Make Your Changes**:
   - Follow style guidelines
   - Test thoroughly
   - Update relevant documentation

3. **Commit Changes**:
   ```bash
   git add .
   git commit -m "Your descriptive message"
   ```

4. **Push to Your Fork**:
   ```bash
   git push origin docs/your-improvement
   ```

5. **Create Pull Request**:
   - Go to GitHub and create PR
   - Use the PR template
   - Provide clear description
   - Link any related issues

### Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Documentation update
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change

## Checklist
- [ ] I have tested my changes
- [ ] I have updated relevant documentation
- [ ] My changes follow the project style
- [ ] I have added examples if needed

## Related Issues
Closes #123
```

### Review Process

- PRs require maintainer review
- Address feedback promptly
- Be open to suggestions
- Keep PR focused and small

## Questions or Help?

- **Found a bug?** Open an issue
- **Have a question?** Start a discussion
- **Need clarification?** Ask in your PR

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Help others learn
- Celebrate contributions

## License

By contributing, you agree that your contributions will be licensed under the same license as the project.

---

**Thank you for contributing!** Every improvement, no matter how small, helps make this project better for everyone.
