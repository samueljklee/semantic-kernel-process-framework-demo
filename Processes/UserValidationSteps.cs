

using Microsoft.SemanticKernel;
#pragma warning disable SKEXP0080

namespace SKProcessDemo.Processes.UserValidationStep;

/// <summary>
/// A step that elicits user input.
/// </summary>
public class UserValidationStep : KernelProcessStep
{
    public static class ProcessStepFunctions
    {
        public const string GetUserInput = nameof(GetUserInput);
        public const string ShowUserInput = nameof(ShowUserInput);
    }

    public static class OutputEvents
    {
        public const string UserInputReceived = "UserInputReceived";
        public const string Exit = "Exit";
    }

    /// <summary>
    /// Activates the user input step by initializing the state object. This method is called when the process is started
    /// and before any of the KernelFunctions are invoked.
    /// </summary>
    /// <param name="state">The state object for the step.</param>
    /// <returns>A <see cref="ValueTask"/></returns>
    public override ValueTask ActivateAsync(KernelProcessStepState state)
    {

        return ValueTask.CompletedTask;
    }

    [KernelFunction(ProcessStepFunctions.GetUserInput)]
    public async ValueTask GetUserInputAsync(KernelProcessStepContext context)
    {
        // Prompt user for input
        Console.WriteLine("[UserValidationSteps::GetUserInput] Please enter your input (type 'exit' to quit):");
        Console.Write("> ");
        string? userInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(userInput))
        {
            Console.WriteLine("Input cannot be empty. Please try again.");
            return;
        }

        if (string.Equals(userInput, "exit", StringComparison.OrdinalIgnoreCase) || string.Equals(userInput, "quit", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Exiting the process as per user request.");
            // exit condition met, emitting exit event
            await context.EmitEventAsync(new() { Id = OutputEvents.Exit, Data = userInput });
            return;
        }
        // if user's intent is not to continue, we can emit an exit event
        // TODO: add validation or let another step handle this.

        // emitting userInputReceived event
        await context.EmitEventAsync(new() { Id = OutputEvents.UserInputReceived, Data = userInput });
    }
    [KernelFunction(ProcessStepFunctions.ShowUserInput)]
    public ValueTask ShowUserInputAsync(KernelProcessStepContext context, string userInput)
    {
        // Display the user input
        Console.WriteLine($"[UserValidationSteps::ShowUserInput] You entered: {userInput}");
        return ValueTask.CompletedTask;
    }
}
