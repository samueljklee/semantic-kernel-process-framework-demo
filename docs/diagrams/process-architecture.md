# Process Architecture Documentation

This document explains the architecture and design patterns used in the Semantic Kernel Process Framework Demo.

## Table of Contents

- [Overview](#overview)
- [Process Framework Fundamentals](#process-framework-fundamentals)
- [Process Workflows](#process-workflows)
- [State Management](#state-management)
- [Event-Driven Architecture](#event-driven-architecture)

## Overview

The application demonstrates Microsoft's Semantic Kernel Process Framework through multiple workflow patterns. Each process is built using modular, reusable steps connected via an event-driven architecture.

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Application Layer                        │
│  (Program.cs - Menu Interface & Process Selection)           │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                   Process Framework                          │
│  (KernelProcess, ProcessBuilder, Event Routing)              │
└────────────┬────────────────────────┬────────────────────────┘
             │                        │
             ▼                        ▼
┌─────────────────────┐  ┌───────────────────────────┐
│   Process Steps      │  │   External Services       │
│  - Stateless         │  │  - OpenAI API (GPT)       │
│  - Stateful          │  │  - GitHub API             │
│  - Human-in-Loop     │  │                           │
└─────────────────────┘  └───────────────────────────┘
```

## Process Framework Fundamentals

### Core Components

#### 1. KernelProcessStep

Base class for all process steps. Steps can be:

- **Stateless**: No data persists between invocations
  - Example: `GatherProductInfoStep`, `PublishDocumentationStep`
  
- **Stateful**: Maintains state across invocations
  - Example: `GenerateDocumentationStep` (maintains chat history)

```
┌──────────────────────────────────────┐
│        KernelProcessStep             │
├──────────────────────────────────────┤
│  + [KernelFunction] Methods          │
│  + OnEvent() Handlers                │
│  + State Management (optional)       │
└──────────────────────────────────────┘
           │
           ├─── Stateless Steps
           │      └─ Direct input/output
           │
           └─── Stateful Steps<T>
                  └─ Maintains state object
```

#### 2. ProcessBuilder

Constructs and wires processes:

```csharp
ProcessBuilder builder = new("ProcessName");
var step1 = builder.AddStepFromType<Step1>();
var step2 = builder.AddStepFromType<Step2>();

// Wire events
builder.OnInputEvent("Start")
    .SendEventTo(new ProcessFunctionTargetBuilder(step1));
    
step1.OnFunctionResult()
    .SendEventTo(new ProcessFunctionTargetBuilder(step2));

return builder.Build();
```

#### 3. Event System

Communication between steps via named events:

```
Step A emits: "DataProcessed"
       │
       ├──► Step B listens: OnEvent("DataProcessed")
       │
       └──► Step C listens: OnEvent("DataProcessed")
```

## Process Workflows

### 1. Quick Info Process (Simplest)

**Purpose**: Gather and display basic product information

```
┌─────────────┐
│ User Input  │
└──────┬──────┘
       │
       ▼
┌─────────────────────────┐
│ GatherProductInfoStep   │
│  - Input: Product name  │
│  - Output: Product info │
└──────┬──────────────────┘
       │
       ▼
   [Display]
```

**Flow Diagram**:

```
Start Event
    │
    └──► GatherProductInfoStep.GatherInfo(productName)
              │
              └──► OnFunctionResult
                       │
                       └──► [Console Output]
```

**Characteristics**:
- Single step
- No AI involvement
- Stateless
- Immediate completion

### 2. Documentation Process (Automatic)

**Purpose**: End-to-end automated documentation generation

```
┌─────────────┐
│ User Input  │
└──────┬──────┘
       │
       ▼
┌──────────────────────────┐
│ GatherProductInfoStep    │
│  Type: Stateless         │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ GenerateDocumentationStep    │
│  Type: Stateful              │
│  State: ChatHistory          │
│  AI: GPT-4.1-mini            │
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ PublishDocumentationStep     │
│  Type: Stateless             │
└──────────────────────────────┘
```

**Detailed Event Flow**:

```
User Input
    │
    └──► StartDocumentationProcess Event
              │
              ▼
         GatherProductInfoStep
              │
              └──► Emits: FunctionResult(productInfo)
                        │
                        ▼
                   GenerateDocumentationStep.GenerateDoc(productInfo)
                        │
                        ├──► Adds to ChatHistory
                        ├──► Calls AI Service
                        └──► Emits: DocumentationGenerated(doc)
                                  │
                                  ▼
                             PublishDocumentationStep.Publish(doc)
                                  │
                                  └──► [Console Output]
```

**Key Features**:
- Three-step pipeline
- Stateful AI generation
- No human interaction
- ~10-30 second execution

### 3. Documentation Process (Human-in-the-Loop)

**Purpose**: Documentation generation with human review and feedback

```
┌─────────────┐
│ User Input  │
└──────┬──────┘
       │
       ▼
┌──────────────────────────┐
│ GatherProductInfoStep    │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ GenerateDocumentationStep    │
│  Function: GenerateDocAfterHitl│
└──────┬───────────────────────┘
       │
       ▼
┌──────────────────────────────┐
│ UserValidationStep           │
│  - Display documentation     │
│  - Request feedback          │
└──────┬───────────────────────┘
       │
       ├──► [Feedback Loop] ─────┐
       │                         │
       │    ┌────────────────────┘
       │    │
       │    ▼
       │ ┌─────────────────────────────┐
       │ │ GenerateDocumentationStep   │
       │ │  Function: GenerateDoc      │
       │ │  (Process feedback)         │
       │ └──────┬──────────────────────┘
       │        │
       │        └──► Back to UserValidationStep
       │
       └──► [Approved]
              │
              ▼
       ┌──────────────────────────────┐
       │ PublishDocumentationStep     │
       └──────────────────────────────┘
```

**Event Flow with Feedback Loop**:

```
StartDocumentationWithHitlProcess
    │
    └──► GatherProductInfoStep
              │
              └──► FunctionResult
                        │
                        ▼
                   GenerateDocumentationStep.GenerateDocAfterHitl
                        │
                        └──► DocumentationGeneratedRequestFeedback
                                  │
                                  ▼
                             UserValidationStep.GetUserInput
                                  │
                                  ├──► UserInputReceived
                                  │        │
                                  │        ├──► ShowUserInput
                                  │        │        │
                                  │        │        └──► Publish (if approved)
                                  │        │
                                  │        └──► GenerateDocAfterHitl (feedback)
                                  │                  │
                                  │                  └──► [Loop back]
                                  │
                                  └──► Exit
                                           │
                                           └──► StopProcess()
```

**Characteristics**:
- Interactive workflow
- Feedback loop support
- Natural language processing
- Variable duration (depends on iterations)

### 4. GitHub Issue Creator Process

**Purpose**: AI-enhanced GitHub issue creation with human approval

```
┌─────────────────┐
│ User Input      │
│ (owner/repo|    │
│  title|body)    │
└────────┬────────┘
         │
         ▼
┌─────────────────────────┐
│ ValidateIssueInputStep  │
│  - Parse format         │
│  - Validate repo        │
└────────┬────────────────┘
         │
         ▼
┌─────────────────────────┐
│ EnhanceIssueStep        │
│  - AI improves title    │
│  - Formats body         │
│  - Suggests labels      │
└────────┬────────────────┘
         │
         ▼
┌──────────────────────────┐
│ UserReviewStep           │
│  - Display original vs   │
│    enhanced              │
│  - Request approval      │
└────────┬─────────────────┘
         │
         ├──► [Modification Loop] ──┐
         │                          │
         │    ┌─────────────────────┘
         │    │
         │    ▼
         │ ┌──────────────────────────┐
         │ │ ProcessUserFeedbackStep  │
         │ │  - AI processes feedback │
         │ │  - Updates issue         │
         │ └──────┬───────────────────┘
         │        │
         │        └──► Back to UserReviewStep
         │
         └──► [Approved]
                │
                ▼
         ┌──────────────────────────┐
         │ CreateGitHubIssueStep    │
         │  - REST API call         │
         │  - Create issue          │
         └────────┬─────────────────┘
                  │
                  ▼
         ┌──────────────────────────┐
         │ IssueConfirmationStep    │
         │  - Display results       │
         └──────────────────────────┘
```

## State Management

### Stateless Steps

No persistent state between invocations:

```csharp
public class GatherProductInfoStep : KernelProcessStep
{
    [KernelFunction]
    public string GatherInfo(string productName)
    {
        // No state stored
        return $"Product: {productName}";
    }
}
```

**Use Cases**:
- Input validation
- Data transformation
- Output formatting
- API calls (without context)

### Stateful Steps

Maintains state across the process lifecycle:

```csharp
public class GenerateDocumentationStep 
    : KernelProcessStep<GenerateDocumentationStep.DocState>
{
    public class DocState 
    { 
        public ChatHistory? History { get; set; } 
    }
    
    private DocState _state;
    
    public override ValueTask ActivateAsync(
        KernelProcessStepState<DocState> state)
    {
        _state = state.State!;
        _state.History ??= new ChatHistory(SystemPrompt);
        return base.ActivateAsync(state);
    }
    
    [KernelFunction]
    public async Task GenerateDoc(Kernel kernel, string productInfo)
    {
        // State persists across calls
        _state.History.AddUserMessage(productInfo);
        // ... AI processing
    }
}
```

**State Lifecycle**:

```
Process Start
    │
    ▼
ActivateAsync() ─────► Initialize State
    │
    ▼
[Step Functions] ─────► Read/Modify State
    │                         │
    │                         ▼
    │                   State Persists
    │                         │
    └────────► [Next Call] ───┘
                  │
                  ▼
              Process End ──► State Disposed
```

**Use Cases**:
- Chat history maintenance
- Context accumulation
- Multi-step calculations
- Feedback processing

## Event-Driven Architecture

### Event Types

#### 1. Input Events

Trigger process start:

```csharp
builder.OnInputEvent("StartDocumentationProcess")
    .SendEventTo(firstStep);
```

#### 2. Function Result Events

Automatically emitted when functions complete:

```csharp
step1.OnFunctionResult()
    .SendEventTo(step2);
```

#### 3. Custom Events

Explicitly emitted from step code:

```csharp
await context.EmitEventAsync(
    "DocumentationGenerated", 
    documentationContent
);
```

Then handled:

```csharp
generateStep.OnEvent("DocumentationGenerated")
    .SendEventTo(publishStep);
```

### Event Routing Patterns

#### 1. Linear Pipeline

```
Step A ──► Step B ──► Step C
```

```csharp
stepA.OnFunctionResult().SendEventTo(stepB);
stepB.OnFunctionResult().SendEventTo(stepC);
```

#### 2. Conditional Branching

```
       ┌──► Step B (on success)
Step A ┤
       └──► Step C (on failure)
```

```csharp
stepA.OnEvent("Success").SendEventTo(stepB);
stepA.OnEvent("Failure").SendEventTo(stepC);
```

#### 3. Feedback Loop

```
Step A ──► Step B ──┐
   ▲               │
   └───────────────┘
```

```csharp
stepA.OnFunctionResult().SendEventTo(stepB);
stepB.OnEvent("RequiresFeedback").SendEventTo(stepA);
```

#### 4. Fan-Out

```
       ┌──► Step B
Step A ┼──► Step C
       └──► Step D
```

```csharp
stepA.OnFunctionResult()
    .SendEventTo(stepB)
    .SendEventTo(stepC)
    .SendEventTo(stepD);
```

### Event Data Flow

Events can carry data:

```csharp
// Emit with data
await context.EmitEventAsync("DataReady", myData);

// Receive as parameter
[KernelFunction]
public void ProcessData(string myData) 
{
    // myData contains the emitted value
}
```

**Parameter Mapping**:

```csharp
stepA.OnEvent("DataReady")
    .SendEventTo(new ProcessFunctionTargetBuilder(
        stepB,
        functionName: "ProcessData",
        parameterName: "myData"  // Maps event data to parameter
    ));
```

## Design Patterns Demonstrated

### 1. Chain of Responsibility

Steps process data sequentially, each handling its concern:

```
Input → Validate → Enhance → Review → Publish
```

### 2. State Pattern

Stateful steps change behavior based on accumulated context:

```
First call: Initial generation
Second call (with feedback): Refinement
Third call: Further refinement
```

### 3. Observer Pattern

Steps react to events from other steps:

```
GenerateStep emits "DocumentGenerated"
    │
    ├──► ReviewStep observes and displays
    └──► PublishStep observes and outputs
```

### 4. Strategy Pattern

Different process configurations for different needs:

- Quick Info: Single-step strategy
- Automatic: Three-step pipeline strategy  
- Human-in-Loop: Interactive loop strategy

## Best Practices

### 1. Step Design

✅ **DO**:
- Keep steps focused on single responsibility
- Use stateless steps when possible
- Make state serializable
- Handle errors gracefully

❌ **DON'T**:
- Mix multiple concerns in one step
- Store non-serializable objects in state
- Assume step execution order
- Create circular dependencies

### 2. Event Design

✅ **DO**:
- Use descriptive event names
- Include necessary data with events
- Document event contracts
- Handle all emitted events

❌ **DON'T**:
- Use generic event names ("Done", "Error")
- Send complex objects that might not serialize
- Create ambiguous event flows
- Leave events unhandled

### 3. Process Design

✅ **DO**:
- Start simple, add complexity as needed
- Test each step independently
- Document the event flow
- Provide exit points

❌ **DON'T**:
- Create overly complex flows initially
- Tightly couple steps
- Create infinite loops without exit
- Mix concerns across layers

## Extension Points

### Adding New Processes

1. Create process steps (inherit from `KernelProcessStep`)
2. Define events and functions
3. Build process in `Program.cs`
4. Add to menu

### Adding New Steps

1. Create class inheriting from `KernelProcessStep` or `KernelProcessStep<TState>`
2. Add `[KernelFunction]` methods
3. Emit events as needed
4. Wire into existing or new processes

### Integrating External Services

1. Create step for service integration
2. Handle authentication/configuration
3. Map service responses to events
4. Add error handling

## Performance Considerations

### AI Service Calls

- Cached at Semantic Kernel level
- Async by default
- Rate limiting handled by service

### State Management

- State persists in memory during process
- Lightweight objects preferred
- Consider serialization for long-running processes

### Event Processing

- Events processed asynchronously
- Non-blocking by default
- Error handling per step

## Further Reading

- [Microsoft Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Process Framework Overview](https://learn.microsoft.com/en-us/semantic-kernel/concepts/process-framework)
- [Basic Example Walkthrough](../examples/basic-documentation-process.md)
- [Advanced Example Walkthrough](../examples/advanced-human-in-loop.md)
- [GitHub Process Documentation](../../README_GitHub_Process.md)

---

**Questions or suggestions?** See [CONTRIBUTING.md](../../CONTRIBUTING.md) for how to contribute to documentation.
