namespace Elsa.Server.Web.Options;

public class ChatAgentOptions
{
    public List<ChatProviderOptions> Providers { get; set; } = new();
    public List<AgentOptions> Agents { get; set; } = new();
}

public class ChatProviderOptions
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "openai-compatible";
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultModel { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 60;
    public int Priority { get; set; } = 99;
}

public class AgentOptions
{
    public string Name { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
}
