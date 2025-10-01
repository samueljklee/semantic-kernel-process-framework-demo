# Semantic Kernel Process Demo

A demonstration application showcasing Microsoft Semantic Kernel's Process framework, featuring automated documentation generation workflows with optional human-in-the-loop validation.

## Overview

This project demonstrates three different process workflows:

1. **Quick Info Process** - Simple product information gathering
2. **Documentation Process (Automatic)** - End-to-end automated documentation generation
3. **Documentation Process (Human-in-the-Loop)** - Documentation generation with human review and feedback

## Prerequisites

- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **OpenAI API Key** - Required for AI-powered documentation generation

## Getting Started

New to this project? Follow these steps to get up and running quickly:

### Quick Start (5 minutes)

1. **Verify Prerequisites**
   ```bash
   # Check .NET version
   dotnet --version
   # Should show 9.0.x or higher
   ```

2. **Clone and Navigate**
   ```bash
   git clone https://github.com/samueljklee/semantic-kernel-process-framework-demo.git
   cd semantic-kernel-process-framework-demo
   ```

3. **Set Your API Key**
   ```bash
   # On macOS/Linux
   export OPENAI_API_KEY="your-actual-api-key-here"
   
   # On Windows PowerShell
   $env:OPENAI_API_KEY="your-actual-api-key-here"
   ```

4. **Build and Run**
   ```bash
   dotnet build
   dotnet run
   ```

5. **Try Your First Process**
   - Select option `2` (Documentation Process automatically)
   - Enter a product name like "Smart Thermostat"
   - Watch the AI generate documentation!

### What to Try Next

- 📖 [Basic Example Walkthrough](docs/examples/basic-documentation-process.md) - Detailed guide for beginners
- 🚀 [Advanced Example with Human-in-the-Loop](docs/examples/advanced-human-in-loop.md) - Interactive documentation generation
- 🏗️ [Process Architecture Guide](docs/diagrams/process-architecture.md) - Understand how it works
- 🐙 [GitHub Issue Creator Process](README_GitHub_Process.md) - Advanced workflow example

## Setup Instructions

### 1. Clone or Download the Project

```bash
cd /path/to/your/projects
# If you haven't already, navigate to the project directory
cd SKProcessDemo
```

### 2. Set Up OpenAI API Key

You need to set your OpenAI API key as an environment variable. Choose one of the methods below:

#### Option A: Set Environment Variable (Recommended)

**On macOS/Linux:**
```bash
export OPENAI_API_KEY="your-actual-api-key-here"
```

**On Windows (PowerShell):**
```powershell
$env:OPENAI_API_KEY="your-actual-api-key-here"
```

**On Windows (Command Prompt):**
```cmd
set OPENAI_API_KEY=your-actual-api-key-here
```

#### Option B: Create a .env file (Alternative)
Create a `.env` file in the project root:
```
OPENAI_API_KEY=your-actual-api-key-here
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Project

```bash
dotnet build
```

## Running the Application

### Start the Application

```bash
dotnet run
```

### Application Menu

Upon starting, you'll see a menu with three process options:

```
Available Processes:
1. Quick Info Process - Quickly gather basic product information
2. Documentation Process automatically - Generate comprehensive documentation for a product without human intervention
3. Documentation Process with Human in the Loop - Includes human review and feedback in the documentation generation
4. Exit Application
```

### Process Workflows

#### 1. Quick Info Process
- **Input**: Product name (e.g., "Smart Thermostat")
- **Output**: Basic product information gathering
- **Duration**: Quick execution

#### 2. Documentation Process (Automatic)
- **Input**: Product name (e.g., "Enterprise Software")
- **Workflow**: 
  1. Gathers product information
  2. Generates documentation using AI
  3. Publishes documentation automatically
- **Output**: Complete documentation without human intervention

#### 3. Documentation Process (Human-in-the-Loop)
- **Input**: Product name (e.g., "Medical Device")
- **Workflow**:
  1. Gathers product information
  2. Generates initial documentation using AI
  3. **Prompts for human review and feedback**
  4. Incorporates feedback and publishes final documentation
- **Interaction**: You'll be prompted to review and provide feedback during the process

## Example Usage

1. Start the application:
   ```bash
   dotnet run
   ```

2. Select a process (enter number 1-3):
   ```
   > 2
   ```

3. Enter product information when prompted:
   ```
   Enter the input for Documentation Process automatically:
   > Smart Home Security System
   ```

4. Watch the process execute and generate documentation automatically!

## Project Structure

```
SKProcessDemo/
├── Program.cs                              # Main application entry point
├── SKProcessDemo.csproj                    # Project configuration
├── Processes/
│   ├── DocumentationProcessSteps.cs       # Documentation generation steps
│   └── UserValidationSteps.cs            # Human-in-the-loop validation steps
└── README.md                              # This file
```

## Key Components

### Process Steps
- **GatherProductInfoStep**: Collects product information
- **GenerateDocumentationStep**: Uses AI to create documentation
- **PublishDocumentationStep**: Outputs final documentation
- **UserValidationStep**: Handles human feedback and validation

### AI Models Used
- **GPT-4.1-mini**: Primary model for documentation generation
- **GPT-4o-mini**: Alternative model option

## Troubleshooting

### Common Issues and Solutions

#### 1. "OPENAI_API_KEY environment variable is not set"

**Symptoms:**
- Application crashes on startup
- Error message: "OPENAI_API_KEY environment variable is not set"

**Solutions:**
- Ensure you've set the OpenAI API key environment variable
- Restart your terminal/IDE after setting the environment variable
- Verify the variable is set: `echo $OPENAI_API_KEY` (Linux/macOS) or `echo %OPENAI_API_KEY%` (Windows)
- Try setting it in your shell profile for persistence:
  ```bash
  # Add to ~/.bashrc, ~/.zshrc, or equivalent
  export OPENAI_API_KEY="your-key-here"
  ```

#### 2. Build Errors

**Symptoms:**
- `dotnet build` fails
- Missing package references
- Compilation errors

**Solutions:**
- Ensure you have .NET 9.0 SDK installed: `dotnet --version`
- Run `dotnet restore` to restore NuGet packages
- Clear NuGet cache if packages are corrupted:
  ```bash
  dotnet nuget locals all --clear
  dotnet restore
  dotnet build
  ```
- Check for SDK installation issues:
  ```bash
  dotnet --info
  ```

#### 3. Process Execution Errors

**Symptoms:**
- Process starts but fails during execution
- Timeout errors
- API call failures

**Solutions:**
- Check your internet connection (required for OpenAI API calls)
- Verify your OpenAI API key is valid: https://platform.openai.com/api-keys
- Check OpenAI API status: https://status.openai.com/
- Ensure your API key has sufficient credits
- Check for rate limiting (wait a moment and retry)

#### 4. Slow Documentation Generation

**Symptoms:**
- Process takes longer than 30 seconds
- Appears to hang during AI generation

**Solutions:**
- This is normal for complex or lengthy product descriptions
- OpenAI API response time varies based on server load
- Try with a simpler/shorter product name
- Check your internet connection speed
- Verify OpenAI service status

#### 5. GitHub Issue Creator Not Working

**Symptoms:**
- Cannot create GitHub issues
- Authentication errors
- "GITHUB_TOKEN not set" error

**Solutions:**
- Set the GITHUB_TOKEN environment variable:
  ```bash
  export GITHUB_TOKEN="your-github-token"
  ```
- Ensure your token has correct permissions:
  - `public_repo` for public repositories
  - `repo` for private repositories
- Generate a new token at: https://github.com/settings/tokens
- Verify repository access permissions

#### 6. Documentation Quality Issues

**Symptoms:**
- Generated documentation is too brief
- Content doesn't match expectations
- Missing important sections

**Solutions:**
- Use the Human-in-the-Loop process (option 3) for better control
- Provide more detailed product names: "Enterprise CRM Software" vs "Software"
- Give feedback during review to refine output
- Try multiple iterations with different product descriptions
- Modify the SystemPrompt in `GenerateDocumentationStep` for custom behavior

#### 7. Environment Variable Not Persisting

**Symptoms:**
- Need to set OPENAI_API_KEY every time terminal is opened
- Environment variable "disappears" after session ends

**Solutions:**
- Add to your shell profile for persistence:
  
  **macOS/Linux (bash):**
  ```bash
  echo 'export OPENAI_API_KEY="your-key"' >> ~/.bashrc
  source ~/.bashrc
  ```
  
  **macOS/Linux (zsh):**
  ```bash
  echo 'export OPENAI_API_KEY="your-key"' >> ~/.zshrc
  source ~/.zshrc
  ```
  
  **Windows PowerShell (persistent):**
  ```powershell
  [System.Environment]::SetEnvironmentVariable('OPENAI_API_KEY', 'your-key', 'User')
  ```

#### 8. Application Menu Not Displaying Correctly

**Symptoms:**
- Garbled text
- Missing menu options
- Encoding issues

**Solutions:**
- Ensure your terminal supports UTF-8 encoding
- Try a different terminal emulator
- On Windows, use Windows Terminal for best compatibility
- Check console output settings in your IDE

### Getting Help

If you encounter issues not listed above:

1. **Check the documentation:**
   - [Basic Example Walkthrough](docs/examples/basic-documentation-process.md)
   - [Advanced Example Guide](docs/examples/advanced-human-in-loop.md)
   - [Architecture Documentation](docs/diagrams/process-architecture.md)

2. **Verify your setup:**
   - All prerequisites are installed
   - OpenAI API key is correctly set
   - Internet connection is active
   - .NET SDK is version 9.0 or higher

3. **Try clean rebuild:**
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

4. **Check logs and error messages:**
   - Read the full error message carefully
   - Note which process step is failing
   - Check console output for additional details

5. **Test with minimal example:**
   - Try option 1 (Quick Info Process) first
   - Use simple product names
   - Verify basic functionality before advanced features

## Development

### Adding New Process Steps

1. Create a new class inheriting from `KernelProcessStep`
2. Define process functions with `[KernelFunction]` attributes
3. Add the step to your process workflow in `Program.cs`

### Customizing AI Behavior

- Modify the `SystemPrompt` in `GenerateDocumentationStep` to change AI behavior
- Adjust model selection in the kernel builder configuration

## License

This is a demonstration project for learning Microsoft Semantic Kernel Process framework.

---

**Happy coding with Semantic Kernel! 🚀**
