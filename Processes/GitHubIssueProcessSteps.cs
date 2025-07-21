using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Process;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

#pragma warning disable SKEXP0080

namespace SKProcessDemo.Processes.GitHubIssueProcessSteps;

// Data models for GitHub issue workflow
public class IssueInput
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Repository { get; set; } = "";
    public string Owner { get; set; } = "";
    public List<string> Labels { get; set; } = new();
}

public class ProcessSummary
{
    public string OriginalInput { get; set; } = "";
    public IssueInput ParsedInput { get; set; } = new();
    public EnhancedIssue Enhancement { get; set; } = new();
    public CreatedIssue Result { get; set; } = new();
    public DateTime StartTime { get; set; } = DateTime.Now;
    public DateTime EndTime { get; set; } = DateTime.Now;
    public List<string> StepsCompleted { get; set; } = [];
}

// Static progress tracker for the entire process
public static class ProcessProgressTracker
{
    private static readonly List<string> AllSteps =
    [
        "🔍 Input Validation - Parse and validate repository and issue details",
        "🤖 AI Enhancement - Improve title, body formatting, and suggest labels",
        "👤 Human Review - Show enhanced version for user approval",
        "🚀 GitHub API Call - Create the issue via REST API",
        "📋 Confirmation - Display success and process summary"
    ];

    private static readonly HashSet<int> CompletedSteps = [];
    private static bool _progressShown = false;

    public static void ShowInitialProgress()
    {
        if (_progressShown) return;
        
        Console.WriteLine("\n" + new string('═', 65));
        Console.WriteLine("🔄 GITHUB ISSUE CREATION WORKFLOW");
        Console.WriteLine(new string('═', 65));
        Console.WriteLine("\n📋 Process Steps:");
        
        for (int i = 0; i < AllSteps.Count; i++)
        {
            Console.WriteLine($"   {i + 1}. ⏳ {AllSteps[i]}");
        }
        
        Console.WriteLine("\n" + new string('═', 65));
        Console.WriteLine("🚀 Starting process execution...\n");
        _progressShown = true;
    }

    public static void ShowStepProgress(int currentStep, string stepAction = "")
    {
        Console.WriteLine("\n" + new string('─', 65));
        Console.WriteLine("📊 WORKFLOW PROGRESS");
        Console.WriteLine(new string('─', 65));
        
        for (int i = 0; i < AllSteps.Count; i++)
        {
            int stepNum = i + 1;
            string status;
            string stepText = AllSteps[i];
            
            if (CompletedSteps.Contains(stepNum))
            {
                status = "✅";
            }
            else if (stepNum == currentStep)
            {
                status = "🔄";
                if (!string.IsNullOrEmpty(stepAction))
                {
                    stepText += $" - {stepAction}";
                }
            }
            else
            {
                status = "⏳";
            }
            
            Console.WriteLine($"   {stepNum}. {status} {stepText}");
        }
        Console.WriteLine(new string('─', 65) + "\n");
    }

    public static void MarkStepCompleted(int stepNumber, string additionalInfo = "")
    {
        if (stepNumber < 1 || stepNumber > AllSteps.Count) return;
        
        CompletedSteps.Add(stepNumber);
        
        if (!string.IsNullOrEmpty(additionalInfo))
        {
            Console.WriteLine($"✅ {additionalInfo}");
        }
    }
    
    public static void MarkStepCancelled(int stepNumber, string additionalInfo = "")
    {
        if (stepNumber < 1 || stepNumber > AllSteps.Count) return;
        
        CompletedSteps.Add(stepNumber);
        
        if (!string.IsNullOrEmpty(additionalInfo))
        {
            Console.WriteLine($"❌ {additionalInfo}");
        }
    }

    public static void ShowCurrentProgress()
    {
        Console.WriteLine("\n" + new string('─', 50));
        Console.WriteLine("📊 CURRENT PROGRESS");
        Console.WriteLine(new string('─', 50));

        for (int i = 0; i < AllSteps.Count; i++)
        {
            string status = CompletedSteps.Contains(i + 1) ? "✅" : "⏳";
            Console.WriteLine($"   {i + 1}. {status} {AllSteps[i]}");
        }
        Console.WriteLine(new string('─', 50));
    }

    public static void Reset()
    {
        CompletedSteps.Clear();
        _progressShown = false;
    }
}

public class EnhancedIssue
{
    public string OriginalTitle { get; set; } = "";
    public string EnhancedTitle { get; set; } = "";
    public string OriginalBody { get; set; } = "";
    public string EnhancedBody { get; set; } = "";
    public List<string> SuggestedLabels { get; set; } = new();
    public string Repository { get; set; } = "";
    public string Owner { get; set; } = "";
}

public class CreatedIssue
{
    public long Id { get; set; }
    public int Number { get; set; }
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Repository { get; set; } = "";
    public string Owner { get; set; } = "";
    public List<string> Labels { get; set; } = [];
    public string State { get; set; } = "";
    public string CreatedAt { get; set; } = "";
    public string Author { get; set; } = "";
}

// Step 1: Validate issue input
public class ValidateIssueInputStep : KernelProcessStep
{
    public static class ProcessStepFunctions
    {
        public const string ValidateInput = nameof(ValidateInput);
    }

    public static class OutputEvents
    {
        public const string InputValidated = "InputValidated";
        public const string ValidationFailed = "ValidationFailed";
    }

    [KernelFunction(ProcessStepFunctions.ValidateInput)]
    public async Task ValidateInputAsync(KernelProcessStepContext context, string rawInput)
    {
        ProcessProgressTracker.ShowInitialProgress();
        ProcessProgressTracker.ShowStepProgress(1, "Parsing repository and issue details");
        
        try
        {
            // Parse the raw input (expecting format: "owner/repo|title|body")
            var parts = rawInput.Split('|', 3);
            
            if (parts.Length < 2)
            {
                Console.WriteLine("❌ Invalid input format. Expected: 'owner/repo|title|body'");
                await context.EmitEventAsync(new() { Id = OutputEvents.ValidationFailed, Data = "Invalid input format" });
                return;
            }

            var repoParts = parts[0].Split('/');
            if (repoParts.Length != 2)
            {
                Console.WriteLine("❌ Invalid repository format. Expected: 'owner/repo'");
                await context.EmitEventAsync(new() { Id = OutputEvents.ValidationFailed, Data = "Invalid repository format" });
                return;
            }

            var issueInput = new IssueInput
            {
                Owner = repoParts[0],
                Repository = repoParts[1],
                Title = parts[1].Trim(),
                Body = parts.Length > 2 ? parts[2].Trim() : ""
            };

            if (string.IsNullOrWhiteSpace(issueInput.Title))
            {
                Console.WriteLine("❌ Title cannot be empty");
                await context.EmitEventAsync(new() { Id = OutputEvents.ValidationFailed, Data = "Title cannot be empty" });
                return;
            }

            Console.WriteLine($"   Repository: {issueInput.Owner}/{issueInput.Repository}");
            Console.WriteLine($"   Title: {issueInput.Title}");
            Console.WriteLine($"   Body: {(string.IsNullOrEmpty(issueInput.Body) ? "(empty)" : $"{issueInput.Body[..Math.Min(50, issueInput.Body.Length)]}...")}");

            ProcessProgressTracker.MarkStepCompleted(1, $"Input validated: {issueInput.Owner}/{issueInput.Repository}");
            await context.EmitEventAsync(new() { Id = OutputEvents.InputValidated, Data = JsonSerializer.Serialize(issueInput) });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Validation error: {ex.Message}");
            await context.EmitEventAsync(new() { Id = OutputEvents.ValidationFailed, Data = ex.Message });
        }
    }
}

// Step 2: AI Enhancement
public class EnhanceIssueStep : KernelProcessStep<EnhanceIssueStep.EnhancementState>
{
    public static class ProcessStepFunctions
    {
        public const string EnhanceIssue = nameof(EnhanceIssue);
    }

    public static class OutputEvents
    {
        public const string IssueEnhanced = "IssueEnhanced";
    }

    public class EnhancementState 
    { 
        public ChatHistory? History { get; set; } 
    }

    private EnhancementState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<EnhancementState> state)
    {
        _state = state.State!;
        _state.History ??= new ChatHistory(SystemPrompt);
        return base.ActivateAsync(state);
    }

    private const string SystemPrompt = @"You are an expert GitHub issue assistant. Your job is to improve issue titles and descriptions to make them clear, actionable, and well-formatted.

For the title:
- Make it concise but descriptive
- Use imperative mood when appropriate
- Ensure it clearly describes the problem or request

For the body:
- Structure it with clear sections
- Add markdown formatting
- Include relevant details like steps to reproduce, expected behavior, etc.
- Suggest appropriate labels based on content

Respond with a JSON object containing:
{
  ""enhancedTitle"": ""improved title"",
  ""enhancedBody"": ""improved body with markdown"",
  ""suggestedLabels"": [""label1"", ""label2""]
}";

    [KernelFunction(ProcessStepFunctions.EnhanceIssue)]
    public async Task EnhanceIssueAsync(Kernel kernel, KernelProcessStepContext context, string issueInputJson)
    {
        ProcessProgressTracker.ShowStepProgress(2, "Using AI to improve content");
        
        var issueInput = JsonSerializer.Deserialize<IssueInput>(issueInputJson)!;
        
        // Add user message with the issue content
        var userMessage = $@"Please enhance this GitHub issue:

Title: {issueInput.Title}
Body: {issueInput.Body}
Repository: {issueInput.Owner}/{issueInput.Repository}

Provide suggestions to make it clearer and more actionable.";

        _state.History!.AddUserMessage(userMessage);
        
        // Use AI to enhance the issue
        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var result = await chatService.GetChatMessageContentAsync(_state.History);
        
        // Create enhanced issue object
        var enhancedIssue = new EnhancedIssue
        {
            OriginalTitle = issueInput.Title,
            OriginalBody = issueInput.Body,
            Repository = issueInput.Repository,
            Owner = issueInput.Owner,
            EnhancedTitle = issueInput.Title, // Fallback
            EnhancedBody = result.Content ?? issueInput.Body,
            SuggestedLabels = new List<string> { "enhancement" }
        };

        try
        {
            // Try to parse AI response as JSON
            var aiResponse = JsonSerializer.Deserialize<JsonElement>(result.Content!);
            if (aiResponse.TryGetProperty("enhancedTitle", out var titleElement))
                enhancedIssue.EnhancedTitle = titleElement.GetString() ?? issueInput.Title;
            if (aiResponse.TryGetProperty("enhancedBody", out var bodyElement))
                enhancedIssue.EnhancedBody = bodyElement.GetString() ?? result.Content!;
            if (aiResponse.TryGetProperty("suggestedLabels", out var labelsElement))
                enhancedIssue.SuggestedLabels = labelsElement.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToList();
        }
        catch
        {
            // If JSON parsing fails, use the raw content as enhanced body
            enhancedIssue.EnhancedBody = result.Content!;
        }

        ProcessProgressTracker.MarkStepCompleted(2, $"AI enhancement complete: {enhancedIssue.EnhancedTitle}");
        await context.EmitEventAsync(new() { Id = OutputEvents.IssueEnhanced, Data = JsonSerializer.Serialize(enhancedIssue) });
    }
}

// Step 3: User Review
public class UserReviewStep : KernelProcessStep
{
    public static class ProcessStepFunctions
    {
        public const string ReviewEnhancement = nameof(ReviewEnhancement);
    }

    public static class OutputEvents
    {
        public const string ApprovalReceived = "ApprovalReceived";
        public const string RejectionReceived = "RejectionReceived";
        public const string ModificationRequested = "ModificationRequested";
    }

    [KernelFunction(ProcessStepFunctions.ReviewEnhancement)]
    public async Task ReviewEnhancementAsync(KernelProcessStepContext context, string enhancedIssueJson)
    {
        ProcessProgressTracker.ShowStepProgress(3, "Awaiting user approval");
        var enhancedIssue = JsonSerializer.Deserialize<EnhancedIssue>(enhancedIssueJson)!;
        
        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("📋 ISSUE ENHANCEMENT REVIEW");
        Console.WriteLine(new string('=', 60));
        
        Console.WriteLine($"\n🏢 Repository: {enhancedIssue.Owner}/{enhancedIssue.Repository}");
        
        Console.WriteLine($"\n📝 ORIGINAL TITLE:");
        Console.WriteLine($"   {enhancedIssue.OriginalTitle}");
        
        Console.WriteLine($"\n✨ ENHANCED TITLE:");
        Console.WriteLine($"   {enhancedIssue.EnhancedTitle}");
        
        Console.WriteLine($"\n📄 ORIGINAL BODY:");
        Console.WriteLine($"   {(string.IsNullOrEmpty(enhancedIssue.OriginalBody) ? "(empty)" : enhancedIssue.OriginalBody)}");
        
        Console.WriteLine($"\n🚀 ENHANCED BODY:");
        Console.WriteLine($"   {enhancedIssue.EnhancedBody}");
        
        Console.WriteLine($"\n🏷️  SUGGESTED LABELS:");
        Console.WriteLine($"   {string.Join(", ", enhancedIssue.SuggestedLabels)}");
        
        Console.WriteLine($"\n{new string('=', 60)}");
        Console.WriteLine("📝 REVIEW OPTIONS:");
        Console.WriteLine("  • Type 'y' or 'yes' to approve and create the issue");
        Console.WriteLine("  • Type 'n' or 'no' to cancel");
        Console.WriteLine("  • Type 'exit' to quit");
        Console.WriteLine("  • Or describe changes in natural language (e.g., 'make the title shorter', 'add more details to the body', 'change bug to enhancement')");
        Console.WriteLine("\nWhat would you like to do?");
        Console.Write("> ");
        
        string? userResponse = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(userResponse))
        {
            Console.WriteLine("❌ No input provided. Please try again.");
            await context.EmitEventAsync(new() { Id = OutputEvents.ModificationRequested, Data = JsonSerializer.Serialize(new { enhancedIssue, feedback = "No input provided" }) });
            return;
        }
        
        // Check for cancellation
        if (userResponse.ToLower().StartsWith('n'))
        {
            Console.WriteLine("❌ Issue creation cancelled by user");
            await context.EmitEventAsync(new() { Id = OutputEvents.RejectionReceived, Data = "User cancelled" });
            return;
        }
        
        // Check for exit
        if (string.Equals(userResponse, "exit", StringComparison.OrdinalIgnoreCase) || 
            string.Equals(userResponse, "quit", StringComparison.OrdinalIgnoreCase) || 
            string.Equals(userResponse, "q", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("👋 Exiting process...");
            await context.EmitEventAsync(new() { Id = OutputEvents.RejectionReceived, Data = "Exit requested" });
            return;
        }
        
        // Check for approval
        if (string.Equals(userResponse, "y", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(userResponse, "yes", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(userResponse, "approve", StringComparison.OrdinalIgnoreCase))
        {
            ProcessProgressTracker.MarkStepCompleted(3, "User approved the enhanced issue");
            await context.EmitEventAsync(new() { Id = OutputEvents.ApprovalReceived, Data = enhancedIssueJson });
            return;
        }
        
        // Otherwise, treat as modification request
        Console.WriteLine("🔄 Processing your modification request...");
        var modificationRequest = new { enhancedIssue, feedback = userResponse };
        await context.EmitEventAsync(new() { Id = OutputEvents.ModificationRequested, Data = JsonSerializer.Serialize(modificationRequest) });
    }
}

// Step 3.5: Process User Feedback
public class ProcessUserFeedbackStep : KernelProcessStep<ProcessUserFeedbackStep.FeedbackState>
{
    public static class ProcessStepFunctions
    {
        public const string ProcessFeedback = nameof(ProcessFeedback);
    }

    public static class OutputEvents
    {
        public const string IssueModified = "IssueModified";
    }

    public class FeedbackState 
    { 
        public ChatHistory? History { get; set; } 
    }

    private FeedbackState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<FeedbackState> state)
    {
        _state = state.State!;
        _state.History ??= new ChatHistory(SystemPrompt);
        return base.ActivateAsync(state);
    }

    private const string SystemPrompt = @"You are an expert GitHub issue editor. Your job is to modify GitHub issues based on user feedback while preserving the original intent and structure.

When given an issue and user feedback, you should:
1. Carefully analyze what the user wants to change
2. Apply the requested changes while maintaining quality
3. Keep the markdown formatting and structure
4. Preserve important technical details unless explicitly asked to change them

Respond with a JSON object containing the updated issue:
{
  ""enhancedTitle"": ""updated title"",
  ""enhancedBody"": ""updated body with markdown"",
  ""suggestedLabels"": [""label1"", ""label2""]
}

Be precise and only change what the user requested.";

    [KernelFunction(ProcessStepFunctions.ProcessFeedback)]
    public async Task ProcessFeedbackAsync(Kernel kernel, KernelProcessStepContext context, string modificationRequestJson)
    {
        Console.WriteLine("🤖 Processing your feedback with AI...");
        
        var request = JsonSerializer.Deserialize<JsonElement>(modificationRequestJson);
        var enhancedIssue = JsonSerializer.Deserialize<EnhancedIssue>(request.GetProperty("enhancedIssue").GetRawText())!;
        var feedback = request.GetProperty("feedback").GetString()!;
        
        // Add user message with the modification request
        var userMessage = $@"Current GitHub Issue:
Title: {enhancedIssue.EnhancedTitle}
Body: {enhancedIssue.EnhancedBody}
Labels: {string.Join(", ", enhancedIssue.SuggestedLabels)}

User Feedback: {feedback}

Please modify the issue based on this feedback while keeping the quality high.";

        _state.History!.AddUserMessage(userMessage);
        
        // Use AI to process the feedback and modify the issue
        var chatService = kernel.GetRequiredService<IChatCompletionService>();
        var result = await chatService.GetChatMessageContentAsync(_state.History);
        
        // Create modified issue object
        var modifiedIssue = new EnhancedIssue
        {
            OriginalTitle = enhancedIssue.OriginalTitle,
            OriginalBody = enhancedIssue.OriginalBody,
            Repository = enhancedIssue.Repository,
            Owner = enhancedIssue.Owner,
            EnhancedTitle = enhancedIssue.EnhancedTitle, // Fallback
            EnhancedBody = result.Content ?? enhancedIssue.EnhancedBody,
            SuggestedLabels = enhancedIssue.SuggestedLabels
        };

        try
        {
            // Try to parse AI response as JSON
            var aiResponse = JsonSerializer.Deserialize<JsonElement>(result.Content!);
            if (aiResponse.TryGetProperty("enhancedTitle", out var titleElement))
                modifiedIssue.EnhancedTitle = titleElement.GetString() ?? enhancedIssue.EnhancedTitle;
            if (aiResponse.TryGetProperty("enhancedBody", out var bodyElement))
                modifiedIssue.EnhancedBody = bodyElement.GetString() ?? result.Content!;
            if (aiResponse.TryGetProperty("suggestedLabels", out var labelsElement))
                modifiedIssue.SuggestedLabels = labelsElement.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToList();
        }
        catch
        {
            // If JSON parsing fails, use the raw content as enhanced body
            modifiedIssue.EnhancedBody = result.Content!;
        }

        Console.WriteLine("✅ Issue modified based on your feedback!");
        await context.EmitEventAsync(new() { Id = OutputEvents.IssueModified, Data = JsonSerializer.Serialize(modifiedIssue) });
    }
}

// Step 4: Create GitHub Issue
public class CreateGitHubIssueStep : KernelProcessStep
{
    public static class ProcessStepFunctions
    {
        public const string CreateIssue = nameof(CreateIssue);
    }

    public static class OutputEvents
    {
        public const string IssueCreated = "IssueCreated";
        public const string CreationFailed = "CreationFailed";
    }

    [KernelFunction(ProcessStepFunctions.CreateIssue)]
    public async Task CreateIssueAsync(KernelProcessStepContext context, string enhancedIssueJson)
    {
        ProcessProgressTracker.ShowStepProgress(4, "Calling GitHub REST API");
        
        var enhancedIssue = JsonSerializer.Deserialize<EnhancedIssue>(enhancedIssueJson)!;
        
        // Get GitHub token from environment
        string? githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        if (string.IsNullOrEmpty(githubToken))
        {
            Console.WriteLine("❌ GITHUB_TOKEN environment variable not set");
            await context.EmitEventAsync(new() { Id = OutputEvents.CreationFailed, Data = "Missing GitHub token" });
            return;
        }

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "SK-Process-Demo");

            var requestBody = new
            {
                title = enhancedIssue.EnhancedTitle,
                body = enhancedIssue.EnhancedBody,
                labels = enhancedIssue.SuggestedLabels.ToArray()
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string url = $"https://api.github.com/repos/{enhancedIssue.Owner}/{enhancedIssue.Repository}/issues";
            var response = await httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var issueResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                var createdIssue = new CreatedIssue
                {
                    Id = issueResponse.GetProperty("id").GetInt64(),
                    Number = issueResponse.GetProperty("number").GetInt32(),
                    Url = issueResponse.GetProperty("html_url").GetString()!,
                    Title = issueResponse.GetProperty("title").GetString()!,
                    Body = issueResponse.GetProperty("body").GetString() ?? "",
                    Repository = enhancedIssue.Repository,
                    Owner = enhancedIssue.Owner,
                    State = issueResponse.GetProperty("state").GetString() ?? "open",
                    CreatedAt = issueResponse.GetProperty("created_at").GetString() ?? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Author = issueResponse.GetProperty("user").GetProperty("login").GetString() ?? "unknown",
                    Labels = []
                };

                // Extract labels if present - labels can be strings or objects with "name" property
                if (issueResponse.TryGetProperty("labels", out var labelsArray))
                {
                    createdIssue.Labels = labelsArray.EnumerateArray()
                        .Select(label => 
                        {
                            // Handle both string labels and object labels
                            if (label.ValueKind == JsonValueKind.String)
                            {
                                return label.GetString() ?? "";
                            }
                            else if (label.ValueKind == JsonValueKind.Object && label.TryGetProperty("name", out var nameProperty))
                            {
                                return nameProperty.GetString() ?? "";
                            }
                            return "";
                        })
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList();
                }

                ProcessProgressTracker.MarkStepCompleted(4, $"GitHub issue created: #{createdIssue.Number}");

                await context.EmitEventAsync(new() { Id = OutputEvents.IssueCreated, Data = JsonSerializer.Serialize(createdIssue) });
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Failed to create issue: {response.StatusCode} - {error}");
                await context.EmitEventAsync(new() { Id = OutputEvents.CreationFailed, Data = $"HTTP {response.StatusCode}: {error}" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error creating GitHub issue: {ex.Message} {ex.StackTrace}");
            await context.EmitEventAsync(new() { Id = OutputEvents.CreationFailed, Data = ex.Message });
        }
    }
}

// Step 5: Confirmation
public class IssueConfirmationStep : KernelProcessStep
{
    public static class ProcessStepFunctions
    {
        public const string ShowConfirmation = nameof(ShowConfirmation);
        public const string ShowError = nameof(ShowError);
    }

    [KernelFunction(ProcessStepFunctions.ShowConfirmation)]
    public void ShowConfirmation(string createdIssueJson)
    {
        ProcessProgressTracker.ShowStepProgress(5, "Displaying final results");
        var createdIssue = JsonSerializer.Deserialize<CreatedIssue>(createdIssueJson)!;
        
        ProcessProgressTracker.MarkStepCompleted(5, "Workflow completed successfully!");
        
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("🎉 GITHUB ISSUE CREATED SUCCESSFULLY! 🎉");
        Console.WriteLine(new string('=', 70));
        
        // Show comprehensive issue information
        ShowIssueDetails(createdIssue);
        
        // Show process overview
        ShowProcessOverview(createdIssue);
    }

    private void ShowIssueDetails(CreatedIssue createdIssue)
    {
        Console.WriteLine("\n📊 ISSUE INFORMATION");
        Console.WriteLine(new string('─', 70));
        
        // Repository and Issue Identification
        Console.WriteLine($"🏢 Repository: {createdIssue.Owner}/{createdIssue.Repository}");
        Console.WriteLine($"🆔 Issue ID: {createdIssue.Id}");
        Console.WriteLine($"#️⃣  Issue Number: #{createdIssue.Number}");
        Console.WriteLine($"🏷️  Status: {createdIssue.State.ToUpper()}");
        
        // Issue Content
        Console.WriteLine($"\n📝 Title: {createdIssue.Title}");
        Console.WriteLine($"👤 Created by: {createdIssue.Author}");
        Console.WriteLine($"📅 Created at: {FormatDateTime(createdIssue.CreatedAt)}");
        
        // Labels
        if (createdIssue.Labels.Count > 0)
        {
            Console.WriteLine($"🏷️  Labels: {string.Join(", ", createdIssue.Labels)}");
        }
        else
        {
            Console.WriteLine("🏷️  Labels: None");
        }
        
        // URLs and Access
        Console.WriteLine($"\n🔗 Direct URL: {createdIssue.Url}");
        Console.WriteLine($"🌐 API URL: https://api.github.com/repos/{createdIssue.Owner}/{createdIssue.Repository}/issues/{createdIssue.Number}");
        
        // Quick Actions
        Console.WriteLine($"\n⚡ QUICK ACTIONS:");
        Console.WriteLine($"   • View Issue: {createdIssue.Url}");
        Console.WriteLine($"   • Edit Issue: {createdIssue.Url.Replace("/issues/", "/issues/")}/edit");
        Console.WriteLine($"   • Add Comment: {createdIssue.Url}#issuecomment-new");
        Console.WriteLine($"   • Repository: https://github.com/{createdIssue.Owner}/{createdIssue.Repository}");
        
        // Body Preview
        if (!string.IsNullOrEmpty(createdIssue.Body))
        {
            Console.WriteLine($"\n📄 Body Preview:");
            var bodyPreview = createdIssue.Body.Length > 200 
                ? createdIssue.Body[..200] + "..." 
                : createdIssue.Body;
            
            // Split into lines for better display
            var lines = bodyPreview.Split('\n');
            foreach (var line in lines.Take(5)) // Show first 5 lines
            {
                Console.WriteLine($"   {line}");
            }
            if (lines.Length > 5)
            {
                Console.WriteLine($"   ... ({lines.Length - 5} more lines)");
            }
        }
        
        Console.WriteLine(new string('─', 70));
    }

    private string FormatDateTime(string isoDateTime)
    {
        if (DateTime.TryParse(isoDateTime, out var date))
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss UTC");
        }
        return isoDateTime;
    }

    private void ShowProcessOverview(CreatedIssue createdIssue)
    {
        Console.WriteLine("\n" + new string('─', 60));
        Console.WriteLine("📊 PROCESS OVERVIEW");
        Console.WriteLine(new string('─', 60));
        Console.WriteLine("\n🔄 Workflow Steps Completed:");
        Console.WriteLine("   1. ✅ Input Validation - Parsed repository and issue details");
        Console.WriteLine("   2. ✅ AI Enhancement - Improved title and body formatting");
        Console.WriteLine("   3. ✅ Human Review - User approved the enhanced content");
        Console.WriteLine("   4. ✅ GitHub API Call - Successfully created the issue");
        Console.WriteLine("   5. ✅ Confirmation - Process completed successfully");
        
        Console.WriteLine("\n🏗️ Process Framework Features Used:");
        Console.WriteLine("   • Event-driven step chaining");
        Console.WriteLine("   • Stateful AI enhancement step");
        Console.WriteLine("   • Human-in-the-loop validation");
        Console.WriteLine("   • External API integration");
        Console.WriteLine("   • Comprehensive error handling");
        
        Console.WriteLine("\n📈 Results:");
        Console.WriteLine($"   • Issue Created: #{createdIssue.Number}");
        Console.WriteLine($"   • Repository Integration: Complete");
        Console.WriteLine($"   • AI Enhancement: Applied");
        Console.WriteLine($"   • User Approval: Received");
        
        Console.WriteLine($"\n🎯 Next Steps:");
        Console.WriteLine($"   • Visit the issue: {createdIssue.Url}");
        Console.WriteLine($"   • Add comments or assign team members");
        Console.WriteLine($"   • Link related PRs or issues");
        Console.WriteLine($"   • Track progress and resolution");
        
        Console.WriteLine("\n" + new string('─', 60));
    }

    [KernelFunction(ProcessStepFunctions.ShowError)]
    public void ShowError(string error)
    {
        Console.WriteLine("\n" + new string('=', 30));
        Console.WriteLine("❌ ISSUE CREATION FAILED ❌");
        Console.WriteLine(new string('=', 30));
        Console.WriteLine($"\nError: {error}");
        Console.WriteLine($"\n💡 Please check:");
        Console.WriteLine($"   • GITHUB_TOKEN environment variable is set");
        Console.WriteLine($"   • Token has 'repo' permissions");
        Console.WriteLine($"   • Repository exists and you have access");
        Console.WriteLine($"   • Repository format is correct (owner/repo)");
    }
}