using Elsa.Server.Web.Options;

namespace Elsa.Server.Web.Providers;

public interface IChatProvider
{
    Task<ProviderResult> SendAsync(
        ChatProviderOptions options,
        string systemPrompt,
        string userMessage,
        List<object> skills,
        CancellationToken ct);
}

public class ProviderResult
{
    public string Response { get; set; } = string.Empty;
    public string ModelUsed { get; set; } = string.Empty;
    public long LatencyMs { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}
