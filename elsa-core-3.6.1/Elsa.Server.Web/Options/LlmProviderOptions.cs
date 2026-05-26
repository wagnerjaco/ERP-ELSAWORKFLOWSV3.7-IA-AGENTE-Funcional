namespace Elsa.Server.Web.Options;

public class LlmProviderOptions
{
    public string Name { get; set; } = null!;
    public string ProviderType { get; set; } = "openai";
    public string BaseUrl { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string DefaultModel { get; set; } = null!;
    public bool IsDefault { get; set; }
}
