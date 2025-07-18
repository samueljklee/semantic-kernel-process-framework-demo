using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Process;
using Microsoft.SemanticKernel.ChatCompletion;  // for IChatCompletionService
// ... other SK namespaces as needed
#pragma warning disable SKEXP0080

namespace SKProcessDemo.Processes.DocumentationProcessSteps;

// Step 1: Gather product info (stateless example)
public class GatherProductInfoStep : KernelProcessStep
{
    public static class ProcessStepFunctions
    {
        public const string GatherInfo = nameof(GatherInfo);
    }

    [KernelFunction(ProcessStepFunctions.GatherInfo)]  // Mark function as invokable
    public string GatherInfo(string productName)
    {
        Console.WriteLine($"[GatherProductInfoStep] Gathering info for '{productName}'");
        // For demo, return a hardcoded description. In real use, this might call an API or database.
        return $"Product '{productName}' is a revolutionary gadget with cutting-edge features...";
    }
}

// Step 2: Generate documentation (stateful: keeps chat history)
public class GenerateDocumentationStep : KernelProcessStep<GenerateDocumentationStep.DocState>
{
    public static class ProcessStepFunctions
    {
        public const string GenerateDoc = nameof(GenerateDoc);
        public const string GenerateDocAfterHitl = nameof(GenerateDocAfterHitl);
    }

    public static class OutputEvents
    {
        public const string DocumentationGenerated = "DocumentationGenerated";
        public const string DocumentationGeneratedRequestFeedback = "DocumentationGeneratedRequestFeedback";
    }

    // Define state to persist across runs (chat history, last output, etc.)
    public class DocState { public ChatHistory? History { get; set; } }
    private DocState _state = new();

    // When the step is activated (started), ensure state is ready
    public override ValueTask ActivateAsync(KernelProcessStepState<DocState> state)
    {
        _state = state.State!;
        _state.History ??= new ChatHistory(SystemPrompt);
        return base.ActivateAsync(state);
    }

    private const string SystemPrompt = "You are an AI documentation writer..."; // system role prompt for LLM

    [KernelFunction(ProcessStepFunctions.GenerateDoc)]
    public async Task GenerateDocAsync(Kernel kernel, KernelProcessStepContext context, string productInfo)
    {
        Console.WriteLine("[GenerateDocumentationStep] Generating docs from product info...");
        // Add user message with the info to the chat history
        _state.History!.AddUserMessage($"Product Info: {productInfo}");
        // Use the kernel's AI service to get completion
        var chatService = kernel.GetRequiredService<IChatCompletionService>("gpt-4.1-mini");
        var result = await chatService.GetChatMessageContentAsync(_state.History);
        string generatedDoc = result.Content!;
        // (Optionally, store result in state)

        // Emit an event with the generated content to pass to next step
        await context.EmitEventAsync(GenerateDocumentationStep.OutputEvents.DocumentationGenerated, generatedDoc);
    }

    [KernelFunction(ProcessStepFunctions.GenerateDocAfterHitl)]
    public async Task GenerateDocAfterHitlAsync(Kernel kernel, KernelProcessStepContext context, string productInfo)
    {
        Console.WriteLine("[GenerateDocumentationWithHitlStep] Generating docs from product info...");
        // Add user message with the info to the chat history
        _state.History!.AddUserMessage($"Product Info: {productInfo}");
        // Use the kernel's AI service to get completion
        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var result = await chatService.GetChatMessageContentAsync(_state.History);
        string generatedDoc = result.Content!;
        // (Optionally, store result in state)

        // Emit an event with the generated content to pass to next step
        Console.WriteLine("[GenerateDocumentationStep] Documentation generated successfully! Please validate the output:\n" + generatedDoc);
        await context.EmitEventAsync(GenerateDocumentationStep.OutputEvents.DocumentationGeneratedRequestFeedback, generatedDoc);
    }
}

// Publish documentation (stateless)
public class PublishDocumentationStep : KernelProcessStep
{
    public static class ProcessStepFunctions
    {
        public const string PublishDoc = nameof(PublishDoc);
    }

    [KernelFunction(ProcessStepFunctions.PublishDoc)]
    public void Publish(string docs)
    {
        Console.WriteLine("[PublishDocumentationStep] Publishing document:\n" + docs);
        // In a real scenario, save to database or send to a service. Here, just output.
    }
}