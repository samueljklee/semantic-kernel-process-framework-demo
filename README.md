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

### Common Issues

1. **"OPENAI_API_KEY environment variable is not set"**
   - Ensure you've set the OpenAI API key environment variable
   - Restart your terminal/IDE after setting the environment variable

2. **Build errors**
   - Ensure you have .NET 9.0 SDK installed
   - Run `dotnet restore` to restore NuGet packages

3. **Process execution errors**
   - Check your internet connection (required for OpenAI API calls)
   - Verify your OpenAI API key is valid and has sufficient credits

### Getting Help

If you encounter issues:

1. Check that all prerequisites are installed
2. Verify your OpenAI API key is correctly set
3. Ensure you have an active internet connection
4. Try running `dotnet clean` followed by `dotnet build`

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
