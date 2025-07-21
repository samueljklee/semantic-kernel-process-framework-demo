# GitHub Issue Creator Process

This demo showcases a sophisticated GitHub issue creation workflow using the Semantic Kernel Process Framework.

## Process Flow

```
User Input → Validate → AI Enhancement → Human Review → (Natural Language Feedback Loop) → Create Issue → Confirmation
```

## Features

1. **Input Validation**: Parses and validates repository format and issue details
2. **AI Enhancement**: Uses GPT to improve title, body formatting, and suggest labels
3. **Enhanced Human Review**: Interactive review with natural language modification support
4. **Natural Language Feedback**: Users can request changes using plain English
5. **AI-Powered Modifications**: Automatically applies user-requested changes
6. **Review Loop**: Continues until user approves or cancels
7. **GitHub Integration**: Creates actual GitHub issues via REST API
8. **Error Handling**: Comprehensive error handling and user feedback

## Setup

### Required Environment Variables

```bash
export OPENAI_API_KEY="your-openai-api-key"
export GITHUB_TOKEN="your-github-personal-access-token"
```

### GitHub Token Requirements

Your GitHub token needs the following permissions:
- For public repositories: `public_repo` scope
- For private repositories: `repo` scope

## Usage

1. Run the application: `dotnet run`
2. Select option `4. GitHub Issue Creator`
3. Enter input in format: `owner/repo|title|body`

### Example Input

```
microsoft/semantic-kernel|Memory leak in chat completion|The chat completion service seems to have a memory leak after processing many requests. Steps to reproduce: 1. Create multiple chat sessions 2. Process 1000+ requests 3. Monitor memory usage
```

## Process Steps Implementation

### 1. ValidateIssueInputStep
- Parses input format (`owner/repo|title|body`)
- Validates repository format
- Ensures title is not empty
- **Location**: `GitHubIssueProcessSteps.cs:49-91`

### 2. EnhanceIssueStep
- Uses AI to improve title clarity
- Formats body with proper markdown
- Suggests appropriate labels
- Maintains chat history for context
- **Location**: `GitHubIssueProcessSteps.cs:93-169`

### 3. UserReviewStep (Enhanced)
- Displays original vs enhanced version
- Shows suggested labels  
- Supports multiple response types:
  - `y/yes/approve` - Approve and proceed
  - `n/no` - Cancel process
  - `exit/quit/q` - Exit application
  - **Natural language feedback** - e.g., "make title shorter", "add more details", "change bug to feature"
- **Location**: `GitHubIssueProcessSteps.cs:338-430`

### 3.5. ProcessUserFeedbackStep (New!)
- Processes natural language feedback with AI
- Applies user-requested modifications intelligently
- Maintains issue quality and formatting
- Returns to review step for re-approval
- **Location**: `GitHubIssueProcessSteps.cs:432-533`

### 4. CreateGitHubIssueStep
- Makes authenticated REST API call to GitHub
- Handles HTTP errors gracefully
- Returns issue number and URL on success
- **Location**: `GitHubIssueProcessSteps.cs:223-299`

### 5. IssueConfirmationStep (Enhanced)
- Shows comprehensive issue information including:
  - Repository details and issue identification
  - Issue metadata (ID, number, status, creation date, author)
  - Applied labels and URLs
  - Quick action links (view, edit, comment, repository)
  - Body content preview
- Displays complete workflow summary
- Shows error details if creation failed
- **Location**: `GitHubIssueProcessSteps.cs:647-776`

## Error Scenarios Handled

- Invalid input format
- Missing GitHub token
- Repository access denied
- Network connectivity issues
- Rate limiting
- User cancellation

## Process Builder Configuration

The process is wired using event-driven architecture:

```csharp
// Success path: Validate → Enhance → Review → (Feedback Loop) → Create → Confirm
validateStep.OnEvent("InputValidated") → enhanceStep
enhanceStep.OnEvent("IssueEnhanced") → reviewStep  
reviewStep.OnEvent("ApprovalReceived") → createStep
createStep.OnEvent("IssueCreated") → confirmationStep

// Feedback loop: Review → Process Feedback → Review (repeats until approval)
reviewStep.OnEvent("ModificationRequested") → feedbackStep
feedbackStep.OnEvent("IssueModified") → reviewStep

// Error paths
validateStep.OnEvent("ValidationFailed") → errorStep
reviewStep.OnEvent("RejectionReceived") → StopProcess()
createStep.OnEvent("CreationFailed") → errorStep
```

## Key Framework Features Demonstrated

- **Stateless Steps**: Input validation, issue creation, confirmation
- **Stateful Steps**: AI enhancement and feedback processing (maintains chat history)  
- **Enhanced Human-in-the-Loop**: Interactive review with natural language feedback
- **AI-Powered Modifications**: Dynamic content updates based on user feedback
- **Feedback Loops**: Continuous refinement until user satisfaction
- **External API Integration**: GitHub REST API calls
- **Event-driven Architecture**: Clean step transitions with loops
- **Error Handling**: Graceful failure scenarios
- **Process Control**: User can exit or modify at any point

## Example Natural Language Feedback

Users can provide feedback like:
- "Make the title more concise"
- "Add more technical details to the body"
- "Change the labels to include 'documentation'"
- "Make it sound less urgent"
- "Add steps to reproduce the issue"
- "Change from bug to feature request"

The AI intelligently processes these requests and modifies the issue accordingly, then presents the updated version for review.

This implementation showcases how the Semantic Kernel Process Framework excels at building sophisticated AI-enhanced business workflows with human oversight, iterative refinement, and external system integration.