using Elsa.Server.Web.Data;
using Elsa.Server.Web.Options;

namespace Elsa.Server.Web.Providers;

public class ChatOrchestrator
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChatOrchestrator> _logger;
    private readonly ChatAuditService _auditService;

    public ChatOrchestrator(
        IConfiguration configuration,
        ILogger<ChatOrchestrator> logger,
        ChatAuditService auditService)
    {
        _configuration = configuration;
        _logger = logger;
        _auditService = auditService;
    }

    public async Task<ChatResult> ProcessAsync(
        string agentName,
        string userMessage,
        List<object> skills,
        Guid? conversationId = null,
        string? userId = null)
    {
        var agents = _configuration.GetSection("ChatAgents:Agents").Get<List<AgentOptions>>();
        var agent = agents?.FirstOrDefault(a => a.Name == agentName);
        if (agent == null)
            return ChatResult.Failure($"Agente '{agentName}' nao encontrado.");

        var providers = _configuration.GetSection("ChatAgents:Providers")
            .Get<List<ChatProviderOptions>>()
            ?.OrderBy(p => p.Priority)
            .ToList() ?? new();

        if (providers.Count == 0)
            return ChatResult.Failure("Nenhum provedor configurado.");

        var requestId = Guid.NewGuid();
        ProviderResult? lastResult = null;
        string lastProviderName = "none";

        foreach (var providerConfig in providers)
        {
            lastProviderName = providerConfig.Name;

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(providerConfig.TimeoutSeconds));
                var provider = new OpenAICompatibleProvider();
                var result = await provider.SendAsync(
                    providerConfig, agent.SystemPrompt, userMessage, skills, cts.Token);

                _logger.LogInformation(
                    "Agent {Agent} | Provider {Provider} | Model {Model} | Latency {Latency}ms",
                    agentName, providerConfig.Name, result.ModelUsed, result.LatencyMs);

                await _auditService.LogAsync(new AuditEntry
                {
                    RequestId = requestId,
                    ConversationId = conversationId,
                    UserId = userId,
                    AgentName = agentName,
                    UserMessage = userMessage,
                    BotResponse = result.Response,
                    ProviderUsed = providerConfig.Name,
                    ModelUsed = result.ModelUsed,
                    LatencyMs = result.LatencyMs,
                    Status = "completed"
                });

                return new ChatResult
                {
                    Response = result.Response,
                    ProviderUsed = providerConfig.Name,
                    ModelUsed = result.ModelUsed,
                    LatencyMs = result.LatencyMs,
                    Success = true
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Agent {Agent} | Provider {Provider} TIMEOUT after {Timeout}s",
                    agentName, providerConfig.Name, providerConfig.TimeoutSeconds);

                lastResult = new ProviderResult
                {
                    Success = false,
                    Error = $"Timeout apos {providerConfig.TimeoutSeconds}s"
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex,
                    "Agent {Agent} | Provider {Provider} HTTP ERROR",
                    agentName, providerConfig.Name);

                lastResult = new ProviderResult { Success = false, Error = ex.Message };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Agent {Agent} | Provider {Provider} ERROR",
                    agentName, providerConfig.Name);

                lastResult = new ProviderResult { Success = false, Error = ex.Message };
            }
        }

        var errorMsg = lastResult?.Error ?? "Todos os provedores falharam";

        await _auditService.LogAsync(new AuditEntry
        {
            RequestId = requestId,
            ConversationId = conversationId,
            UserId = userId,
            AgentName = agentName,
            UserMessage = userMessage,
            BotResponse = errorMsg,
            ProviderUsed = lastProviderName,
            ModelUsed = "",
            LatencyMs = 0,
            Status = "error"
        });

        return ChatResult.Failure(errorMsg);
    }
}

public class ChatResult
{
    public string Response { get; set; } = string.Empty;
    public string ProviderUsed { get; set; } = string.Empty;
    public string ModelUsed { get; set; } = string.Empty;
    public long LatencyMs { get; set; }
    public bool Success { get; set; }

    public static ChatResult Failure(string error) => new()
    {
        Response = error,
        Success = false
    };
}
