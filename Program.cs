using Microsoft.SemanticKernel;
using SKProcessDemo.Processes.DocumentationProcessSteps;
using SKProcessDemo.Processes.UserValidationStep;
// ... (and ensure to include the namespace of your step classes)

#pragma warning disable SKEXP0080

class Program
{
    private readonly static string OPENAI_API_KEY = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");
    static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to the Semantic Kernel Process Demo!");
        Console.WriteLine("==============================================\n");

        // 1. Create an SK Kernel and configure AI service (OpenAI/AzureOpenAI):
        Kernel kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-4.1-mini", OPENAI_API_KEY, serviceId: "gpt-4.1-mini")
            .AddOpenAIChatCompletion("gpt-4o-mini", OPENAI_API_KEY, serviceId: "gpt-4o-mini")
            .Build();

        var processes = GetAvailableProcesses();

        // Main application loop
        while (true)
        {
            try
            {
                Console.WriteLine("\n" + new string('=', 50));
                Console.WriteLine("Available Processes:");
                for (int i = 0; i < processes.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {processes[i].Name} - {processes[i].Description}");
                }
                Console.WriteLine($"{processes.Count + 1}. Exit Application");

                Console.WriteLine($"\nSelect a process (enter number 1-{processes.Count + 1}):");
                Console.Write("> ");

                string? input = Console.ReadLine();

                // Check for exit
                if (int.TryParse(input, out int selectedIndex) && selectedIndex == processes.Count + 1)
                {
                    Console.WriteLine("\nThank you for using the Semantic Kernel Process Demo!");
                    Console.WriteLine("Goodbye! 👋");
                    break;
                }

                // Validate process selection
                if (!int.TryParse(input, out selectedIndex) || selectedIndex < 1 || selectedIndex > processes.Count)
                {
                    Console.WriteLine("Invalid selection. Please try again.");
                    continue;
                }

                var selectedProcess = processes[selectedIndex - 1];
                Console.WriteLine($"\nSelected: {selectedProcess.Name}");

                // Get input data for the selected process
                Console.WriteLine($"\nEnter the input for {selectedProcess.Name}:");
                Console.Write("> ");
                string? inputData = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(inputData))
                {
                    Console.WriteLine($"No input provided. Using default: '{selectedProcess.DefaultInput}'");
                    inputData = selectedProcess.DefaultInput;
                }

                Console.WriteLine($"\nStarting {selectedProcess.Name} for: {inputData}");
                Console.WriteLine("Processing...\n");

                // Build and run the selected process
                KernelProcess process = selectedProcess.BuildProcess();
                await process.StartAsync(kernel, new KernelProcessEvent { Id = selectedProcess.StartEventId, Data = inputData }).ConfigureAwait(false);

                Console.WriteLine($"\n✅ {selectedProcess.Name} completed successfully!");

                // Ask if user wants to continue
                Console.WriteLine("\nWhat would you like to do next?");
                Console.WriteLine("1. Run another process");
                Console.WriteLine("2. Exit application");
                Console.Write("> ");

                string? continueChoice = Console.ReadLine();
                if (continueChoice == "2" || continueChoice?.ToLower() == "exit")
                {
                    Console.WriteLine("\nThank you for using the Semantic Kernel Process Demo!");
                    Console.WriteLine("Goodbye! 👋");
                    break;
                }

                // Continue to next iteration (run another process)
                Console.WriteLine("\nLet's run another process...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ An error occurred: {ex.Message}");
                Console.WriteLine("\nWould you like to try again? (y/n):");
                Console.Write("> ");

                string? retry = Console.ReadLine();
                if (retry?.ToLower() != "y" && retry?.ToLower() != "yes")
                {
                    Console.WriteLine("Exiting application...");
                    break;
                }
            }
        }
    }

    private static List<ProcessDefinition> GetAvailableProcesses()
    {
        return new List<ProcessDefinition>
        {
            new ProcessDefinition
            {
                Name = "Quick Info Process", 
                Description = "Quickly gather basic product information",
                DefaultInput = "Quick Product",
                StartEventId = "StartQuickInfo",
                BuildProcess = () => BuildQuickInfoProcess()
            },
            new ProcessDefinition
            {
                Name = "Documentation Process automatically",
                Description = "Generate comprehensive documentation for a product without human intervention",
                DefaultInput = "Sample Product",
                StartEventId = "StartDocumentationProcess",
                BuildProcess = () => BuildDocumentationProcess()
            },
            new ProcessDefinition
            {
                Name = "Documentation Process with Human in the Loop",
                Description = "Includes human review and feedback in the documentation generation",
                DefaultInput = "Enterprise Product",
                StartEventId = "StartDocumentationWithHitlProcess", 
                BuildProcess = () => BuildDocumentationWithHitlProcess()
            }
        };
    }
    
    private static KernelProcess BuildQuickInfoProcess()
    {
        ProcessBuilder builder = new("QuickInfoProcess");
        var step1 = builder.AddStepFromType<GatherProductInfoStep>();

        builder.OnInputEvent("StartQuickInfo").SendEventTo(new ProcessFunctionTargetBuilder(step1));
        
        return builder.Build();
    }
    
    private static KernelProcess BuildDocumentationProcess()
    {
        ProcessBuilder builder = new("DocumentationProcess");
        var step1 = builder.AddStepFromType<GatherProductInfoStep>();
        var step2 = builder.AddStepFromType<GenerateDocumentationStep>();
        var step3 = builder.AddStepFromType<PublishDocumentationStep>();

        builder.OnInputEvent("StartDocumentationProcess").SendEventTo(new ProcessFunctionTargetBuilder(step1));
        step1.OnFunctionResult().SendEventTo(new ProcessFunctionTargetBuilder(step2, parameterName: "productInfo"));
        step2.OnEvent(GenerateDocumentationStep.OutputEvents.DocumentationGenerated).SendEventTo(new ProcessFunctionTargetBuilder(step3));

        return builder.Build();
    }
    
    private static KernelProcess BuildDocumentationWithHitlProcess()
    {
        ProcessBuilder builder = new("BuildDocumentationWithHitlProcess");
        var gatherProductInfoStep = builder.AddStepFromType<GatherProductInfoStep>();
        var userInputStep = builder.AddStepFromType<UserValidationStep>();
        var generateDocumentationStep = builder.AddStepFromType<GenerateDocumentationStep>();
        var publishDocumentationStep = builder.AddStepFromType<PublishDocumentationStep>();

        builder.OnInputEvent("StartDocumentationWithHitlProcess").SendEventTo(new ProcessFunctionTargetBuilder(gatherProductInfoStep));
        gatherProductInfoStep.OnFunctionResult().SendEventTo(new ProcessFunctionTargetBuilder(generateDocumentationStep, functionName: GenerateDocumentationStep.ProcessStepFunctions.GenerateDocAfterHitl, parameterName: "productInfo"));
        generateDocumentationStep.OnEvent(GenerateDocumentationStep.OutputEvents.DocumentationGeneratedRequestFeedback).SendEventTo(new ProcessFunctionTargetBuilder(userInputStep, UserValidationStep.ProcessStepFunctions.GetUserInput));

        userInputStep.OnEvent(UserValidationStep.OutputEvents.UserInputReceived)
            .SendEventTo(new ProcessFunctionTargetBuilder(userInputStep, functionName: UserValidationStep.ProcessStepFunctions.ShowUserInput, parameterName: "userInput"))
            .SendEventTo(new ProcessFunctionTargetBuilder(publishDocumentationStep));
        
        // publishDocumentationStep next step.

        userInputStep.OnEvent(UserValidationStep.OutputEvents.Exit).StopProcess();

        return builder.Build();
    }

    public class ProcessDefinition
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string DefaultInput { get; set; } = "";
        public string StartEventId { get; set; } = "";
        public Func<KernelProcess> BuildProcess { get; set; } = () => throw new NotImplementedException();
    }
}
