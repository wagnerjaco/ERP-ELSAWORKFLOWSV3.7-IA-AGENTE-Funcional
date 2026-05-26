using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ERP.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatbotController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChatbotController> _logger;
    private readonly IConfiguration _configuration;

    public ChatbotController(HttpClient httpClient, ILogger<ChatbotController> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Mensagem é obrigatória.");

        var elsaServerUrl = _configuration["Elsa:ServerUrl"];
        if (string.IsNullOrWhiteSpace(elsaServerUrl))
            return StatusCode(500, "Elsa Server URL não configurada.");

        try
        {
            var payload = new
            {
                message = request.Message,
                conversationId = request.ConversationId,
                userId = request.UserId
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{elsaServerUrl.TrimEnd('/')}/api/agentchat/send";

            _logger.LogInformation("Enviando mensagem para AgentChat: {Url}", url);

            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao chamar AgentChat: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return StatusCode((int)response.StatusCode, new { error = "Erro ao processar mensagem no agente", details = responseContent });
            }

            var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var output = result.TryGetProperty("response", out var outputProp) ? outputProp.GetString() : responseContent;
            var convId = result.TryGetProperty("conversationId", out var convProp) ? convProp.GetString() : null;

            return Ok(new { response = output, raw = responseContent, conversationId = convId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao comunicar com AgentChat");
            return StatusCode(500, new { error = "Erro interno ao processar mensagem", details = ex.Message });
        }
    }

    [HttpGet("history/{conversationId}")]
    public async Task<IActionResult> GetHistory(Guid conversationId)
    {
        var elsaServerUrl = _configuration["Elsa:ServerUrl"];
        if (string.IsNullOrWhiteSpace(elsaServerUrl))
            return StatusCode(500, "Elsa Server URL não configurada.");

        try
        {
            var url = $"{elsaServerUrl.TrimEnd('/')}/api/agentchat/history/{conversationId}";
            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, new { error = "Erro ao buscar historico" });

            return Content(responseContent, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar historico");
            return StatusCode(500, new { error = "Erro interno ao buscar historico", details = ex.Message });
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public Guid? ConversationId { get; set; }
    public string? UserId { get; set; }
}
