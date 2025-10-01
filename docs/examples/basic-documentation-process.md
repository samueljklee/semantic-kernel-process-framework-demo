# Example: Basic Documentation Process (Automatic)

This walkthrough demonstrates how to use the **Documentation Process (Automatic)** to generate comprehensive documentation for a product without human intervention.

## What You'll Learn

- How to run the automatic documentation generation workflow
- Understanding the step-by-step process flow
- Interpreting the generated output

## Prerequisites

Before starting, ensure you have:
- ✅ .NET 9.0 SDK installed
- ✅ OpenAI API key configured
- ✅ Project built successfully (`dotnet build`)

## Step-by-Step Walkthrough

### Step 1: Start the Application

Open your terminal and navigate to the project directory:

```bash
cd /path/to/semantic-kernel-process-framework-demo
dotnet run
```

**Expected Output:**
```
Available Processes:
1. Quick Info Process - Quickly gather basic product information
2. Documentation Process automatically - Generate comprehensive documentation for a product without human intervention
3. Documentation Process with Human in the Loop - Includes human review and feedback in the documentation generation
4. GitHub Issue Creator - Create GitHub issues with AI enhancement and human approval
5. Exit Application

Enter your choice (1-5):
```

### Step 2: Select the Automatic Documentation Process

Enter `2` to select the automatic documentation process:

```
> 2
```

**Expected Output:**
```
You selected: Documentation Process automatically

This process will:
- Gather product information
- Generate documentation using AI
- Publish documentation automatically
```

### Step 3: Enter Product Information

When prompted, enter a product name. For this example, we'll use "Smart Home Security System":

```
Enter the input for Documentation Process automatically:
> Smart Home Security System
```

### Step 4: Watch the Process Execute

The process will now run through three automated steps:

#### 4.1 Gathering Product Information

**Output:**
```
[GatherProductInfoStep] Gathering product info for: Smart Home Security System
[GatherProductInfoStep] Product info gathered: Product: Smart Home Security System
```

The system collects basic information about the product you specified.

#### 4.2 Generating Documentation

**Output:**
```
[GenerateDocumentationStep] Generating docs from product info...
```

The AI (GPT-4.1-mini) processes the product information and creates comprehensive documentation.

#### 4.3 Publishing Documentation

**Output:**
```
[PublishDocumentationStep] Publishing document:

# Smart Home Security System Documentation

## Overview
Smart Home Security System is a comprehensive solution designed to protect your home...

## Key Features
- 24/7 monitoring and alerts
- Mobile app integration
- AI-powered threat detection
...

## Installation Guide
...

## User Manual
...
```

### Step 5: Review the Generated Documentation

The final output will be displayed in your console. The documentation typically includes:

- **Overview**: Product description and purpose
- **Key Features**: Main capabilities and benefits
- **Installation Guide**: Setup instructions
- **User Manual**: How to use the product
- **Troubleshooting**: Common issues and solutions

### Step 6: Return to Main Menu or Exit

After the process completes, you'll return to the main menu where you can:
- Run another process
- Try a different product
- Exit the application

## Understanding the Process Flow

```
User Input → GatherProductInfoStep → GenerateDocumentationStep → PublishDocumentationStep → Output
```

1. **GatherProductInfoStep**: Collects and formats product information
2. **GenerateDocumentationStep**: Uses AI to generate comprehensive documentation
3. **PublishDocumentationStep**: Outputs the final documentation

## Key Characteristics

- **Fully Automated**: No human intervention required
- **Fast**: Typically completes in 10-30 seconds
- **AI-Powered**: Uses GPT-4.1-mini for intelligent content generation
- **Stateful**: Maintains chat history for context-aware generation

## Try These Examples

Experiment with different product types to see how the AI adapts:

1. **Software Products**:
   - "Enterprise CRM Software"
   - "Mobile Banking App"
   - "Cloud Storage Platform"

2. **Hardware Products**:
   - "Wireless Earbuds"
   - "Smart Thermostat"
   - "Fitness Tracker"

3. **Services**:
   - "Managed IT Services"
   - "Digital Marketing Platform"
   - "Customer Support Portal"

## Common Observations

- The AI generates contextually appropriate documentation based on product type
- Technical products receive more detailed specifications
- Consumer products focus more on user-friendly explanations
- The documentation structure adapts to the product domain

## Next Steps

- Try the [Advanced Human-in-the-Loop Process](./advanced-human-in-loop.md)
- Learn about the [GitHub Issue Creator Process](../../README_GitHub_Process.md)
- Explore the [project architecture](../diagrams/process-architecture.md)

## Troubleshooting

### Issue: Process takes too long

**Solution**: 
- Check your internet connection
- Verify OpenAI API rate limits
- Try with a shorter product name

### Issue: Generated documentation is too brief

**Solution**:
- Provide a more descriptive product name
- Use product category in the name (e.g., "Enterprise Software - ProjectName")

### Issue: API key errors

**Solution**:
- Verify your OPENAI_API_KEY environment variable is set
- Restart your terminal after setting the variable
- Check that your API key has sufficient credits
