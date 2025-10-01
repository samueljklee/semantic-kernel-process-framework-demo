# Example: Advanced Documentation Process (Human-in-the-Loop)

This walkthrough demonstrates the **Documentation Process with Human-in-the-Loop**, showcasing how to incorporate human review and feedback into the AI-powered documentation generation workflow.

## What You'll Learn

- How to interact with the human-in-the-loop workflow
- Providing feedback to improve AI-generated content
- Understanding the iterative refinement process
- When to approve or request changes

## Prerequisites

Before starting, ensure you have:
- ✅ .NET 9.0 SDK installed
- ✅ OpenAI API key configured
- ✅ Project built successfully (`dotnet build`)
- ✅ Familiarity with the basic process (recommended)

## Step-by-Step Walkthrough

### Step 1: Start the Application

Open your terminal and run:

```bash
cd /path/to/semantic-kernel-process-framework-demo
dotnet run
```

### Step 2: Select Human-in-the-Loop Process

Enter `3` to select the human-in-the-loop documentation process:

```
Enter your choice (1-5):
> 3
```

**Expected Output:**
```
You selected: Documentation Process with Human in the Loop

This process will:
- Gather product information
- Generate initial documentation using AI
- Request your review and feedback
- Incorporate your feedback and publish final documentation
```

### Step 3: Enter Product Information

For this example, we'll use a product that requires careful documentation:

```
Enter the input for Documentation Process with Human in the Loop:
> Medical Diagnostic Device
```

> **Note**: Medical devices, enterprise software, and safety-critical systems benefit most from human review.

### Step 4: Initial Information Gathering

**Output:**
```
[GatherProductInfoStep] Gathering product info for: Medical Diagnostic Device
[GatherProductInfoStep] Product info gathered: Product: Medical Diagnostic Device
```

### Step 5: AI Generates Initial Documentation

**Output:**
```
[GenerateDocumentationStep] Generating docs from product info...
[GenerateDocumentationStep] Documentation generated with 1247 characters
```

The AI creates a first draft of the documentation.

### Step 6: Review the Generated Documentation

You'll be prompted to review the AI-generated documentation:

**Output:**
```
[UserValidationStep] Please review the following documentation:

==================== DOCUMENTATION ====================
# Medical Diagnostic Device Documentation

## Overview
The Medical Diagnostic Device is an advanced medical instrument designed for...

## Technical Specifications
- Accuracy: ±0.5%
- Response Time: < 2 seconds
- Compliance: FDA approved

## Usage Instructions
1. Power on the device
2. Insert test sample
3. Wait for results
...

=======================================================

Please provide your feedback:
- Type 'approve' or 'yes' to accept and publish
- Type 'exit' or 'quit' to cancel
- Or describe what changes you'd like (e.g., "add safety warnings", "make it less technical")
Your feedback:
```

### Step 7: Provide Feedback (Option A - Request Changes)

Let's request improvements to make it more comprehensive:

```
Your feedback:
> Add detailed safety warnings and regulatory compliance information
```

**What Happens:**
1. The AI processes your natural language feedback
2. It modifies the documentation accordingly
3. You receive an updated version for review

**Output:**
```
[GenerateDocumentationStep] Processing feedback...
[GenerateDocumentationStep] Documentation updated based on feedback

[UserValidationStep] Please review the UPDATED documentation:

==================== DOCUMENTATION ====================
# Medical Diagnostic Device Documentation

## Overview
The Medical Diagnostic Device is an advanced medical instrument...

## ⚠️ SAFETY WARNINGS
- Only trained medical professionals should operate this device
- Always follow sterilization protocols
- Do not use on patients with pacemakers
- Keep away from strong magnetic fields

## Regulatory Compliance
- FDA 510(k) Clearance: K123456
- CE Mark: Class IIa Medical Device
- ISO 13485:2016 Certified
- HIPAA Compliant

## Technical Specifications
...

=======================================================

Your feedback:
```

### Step 8: Iterative Refinement (Optional)

You can continue refining:

```
Your feedback:
> Add a troubleshooting section for common error codes
```

The AI will further enhance the documentation based on your feedback.

### Step 9: Approve the Final Version

Once satisfied, approve the documentation:

```
Your feedback:
> approve
```

**Or use any of these alternatives:**
- `yes`
- `y`
- `looks good`
- `accept`

**Output:**
```
[UserValidationStep] Documentation approved. Proceeding to publication...
[PublishDocumentationStep] Publishing document:

# Medical Diagnostic Device Documentation

## Overview
...

[Complete final documentation displayed]

Process completed successfully!
```

## Understanding the Human-in-the-Loop Flow

```
User Input → GatherProductInfo → GenerateDocumentation → UserReview
                                                              ↓
                                                    [Feedback Loop]
                                                              ↓
                                    ProcessFeedback ← UserProvidesFeedback
                                          ↓
                                    GenerateUpdated → UserReview (repeat until approved)
                                                              ↓
                                                        UserApproves
                                                              ↓
                                                    PublishDocumentation
```

## Advanced Feedback Examples

### Example 1: Making Content More Technical

**Initial Documentation:**
```
The device measures blood samples quickly and accurately.
```

**Your Feedback:**
```
> Add specific technical details about measurement methodology
```

**Improved Version:**
```
The device employs spectrophotometric analysis at 540nm wavelength 
to quantify hemoglobin concentration with ±0.5% accuracy using 
dual-beam optical path compensation.
```

### Example 2: Simplifying Language

**Initial Documentation:**
```
The apparatus utilizes photometric quantification methodologies...
```

**Your Feedback:**
```
> Make the language less technical and more user-friendly
```

**Improved Version:**
```
The device uses light-based measurement to analyze your sample...
```

### Example 3: Adding Specific Sections

**Your Feedback:**
```
> Include sections for installation requirements, maintenance schedule, 
and warranty information
```

The AI will add comprehensive sections for each requested topic.

### Example 4: Adjusting Tone

**Your Feedback:**
```
> Make the tone more professional and formal for enterprise audience
```

The AI adjusts language, structure, and presentation style accordingly.

## Real-World Use Cases

### Medical Devices
- **Why Human Review Matters**: Regulatory compliance, safety warnings, accurate medical terminology
- **Feedback Focus**: Clinical accuracy, compliance statements, contraindications

### Enterprise Software
- **Why Human Review Matters**: Complex integrations, security considerations, compliance requirements
- **Feedback Focus**: Architecture details, security features, integration guides

### Safety-Critical Systems
- **Why Human Review Matters**: Risk assessment, operational procedures, emergency protocols
- **Feedback Focus**: Safety procedures, failure modes, emergency responses

### Financial Products
- **Why Human Review Matters**: Regulatory disclosures, risk warnings, compliance
- **Feedback Focus**: Legal disclaimers, risk statements, regulatory requirements

## Tips for Effective Feedback

### ✅ DO

- **Be Specific**: "Add a section about data privacy" rather than "improve security section"
- **Focus on Content**: Request factual additions or structural changes
- **Use Natural Language**: Write as you would speak to a technical writer
- **Iterate Gradually**: Make one type of change at a time

### ❌ DON'T

- **Be Vague**: "Make it better" doesn't give actionable direction
- **Request Styling**: The system focuses on content, not visual formatting
- **Combine Too Many Requests**: Break complex changes into multiple iterations

## Comparison: Automatic vs Human-in-the-Loop

| Aspect | Automatic | Human-in-the-Loop |
|--------|-----------|-------------------|
| Speed | ⚡ Fast (10-30s) | 🐢 Slower (varies) |
| Control | 🤖 Full AI | 👤 Human oversight |
| Quality | ✓ Good baseline | ✓✓ Tailored & verified |
| Best For | General products | Critical applications |
| Iterations | None | Multiple rounds |
| Expertise | AI-only | AI + Human knowledge |

## When to Use Human-in-the-Loop

Choose this process when:

- ✅ Documentation requires domain expertise verification
- ✅ Compliance or regulatory content must be accurate
- ✅ Technical accuracy is critical
- ✅ Brand voice and tone are important
- ✅ Specific content requirements must be met
- ✅ Legal or medical implications exist

Use automatic process when:

- ✅ Quick draft is sufficient
- ✅ Content is straightforward
- ✅ Domain is general/common
- ✅ Iteration can happen offline

## Advanced Scenarios

### Scenario 1: Multiple Reviewers

For team workflows:
1. First reviewer focuses on technical accuracy
2. Second reviewer checks compliance and legal aspects
3. Third reviewer verifies user-friendliness

### Scenario 2: Template-Based Documentation

Provide specific requirements:
```
> Structure the documentation with these sections: Executive Summary, 
Technical Overview, Implementation Guide, API Reference, FAQ, and Glossary
```

### Scenario 3: Continuous Refinement

Use the feedback loop for:
- Adding missing information as discovered
- Updating based on testing results
- Incorporating stakeholder feedback
- Aligning with brand guidelines

## Troubleshooting

### Issue: Feedback not being applied correctly

**Solution**:
- Be more specific in your feedback
- Break complex requests into smaller changes
- Verify the feedback was understood by reviewing the update

### Issue: Process takes multiple iterations

**Solution**:
- This is normal for complex documentation
- Provide comprehensive feedback in each round
- Consider using the automatic process for drafts, then manual editing

### Issue: Want to start over

**Solution**:
- Type `exit` to cancel the current process
- Restart and provide different initial input
- Consider providing more detailed product information upfront

## Next Steps

- Explore the [GitHub Issue Creator Process](../../README_GitHub_Process.md) for another human-in-the-loop example
- Review [Process Architecture Documentation](../diagrams/process-architecture.md)
- Try the [Quick Info Process](../../README.md#1-quick-info-process) for comparison
- Read about [Customizing AI Behavior](../../README.md#customizing-ai-behavior)

## Key Takeaways

1. **Human-in-the-Loop enables precision**: Your expertise combined with AI efficiency
2. **Iterative refinement is powerful**: Each round improves quality
3. **Natural language feedback works**: No special syntax required
4. **Context is maintained**: AI remembers previous feedback in the session
5. **Quality over speed**: Extra time investment yields better documentation

---

**Need Help?** Check the [Troubleshooting Guide](../../README.md#troubleshooting) or review the [Getting Started Section](../../README.md#getting-started).
