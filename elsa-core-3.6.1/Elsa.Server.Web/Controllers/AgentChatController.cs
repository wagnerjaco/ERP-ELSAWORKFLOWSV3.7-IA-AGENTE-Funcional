using Elsa.Server.Web.Data;
using Elsa.Server.Web.Providers;
using Elsa.Server.Web.Skills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elsa.Server.Web.Controllers;

[ApiController]
[Route("api/agentchat")]
[AllowAnonymous]
public class AgentChatController : ControllerBase
{
    private readonly ChatOrchestrator _orchestrator;
    private readonly ChatAuditService _auditService;
    private readonly EstoqueSkill _estoqueSkill;
    private readonly PedidosSkill _pedidosSkill;
    private readonly FornecedoresSkill _fornecedoresSkill;

    public AgentChatController(
        ChatOrchestrator orchestrator,
        ChatAuditService auditService,
        EstoqueSkill estoqueSkill,
        PedidosSkill pedidosSkill,
        FornecedoresSkill fornecedoresSkill)
    {
        _orchestrator = orchestrator;
        _auditService = auditService;
        _estoqueSkill = estoqueSkill;
        _pedidosSkill = pedidosSkill;
        _fornecedoresSkill = fornecedoresSkill;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] AgentChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { error = "Mensagem é obrigatória." });

        var agentName = string.IsNullOrWhiteSpace(request.Agent) ? "ConsultorERP" : request.Agent;
        var conversationId = request.ConversationId ?? Guid.NewGuid();
        var skills = new List<object> { _estoqueSkill, _pedidosSkill, _fornecedoresSkill };

        var result = await _orchestrator.ProcessAsync(agentName, request.Message, skills, conversationId, request.UserId);

        if (!result.Success)
            return StatusCode(503, new { error = result.Response });

        return Ok(new
        {
            response = result.Response,
            provider = result.ProviderUsed,
            latencyMs = result.LatencyMs,
            conversationId
        });
    }

    [HttpGet("history/{conversationId}")]
    public async Task<IActionResult> GetHistory(Guid conversationId)
    {
        var messages = await _auditService.GetHistoryAsync(conversationId);
        var result = messages.Select(m => new
        {
            message = m.UserMessage,
            response = m.BotResponse,
            timestamp = m.RequestId
        }).ToList();

        return Ok(new { messages = result });
    }
}

public class AgentChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? Agent { get; set; }
    public Guid? ConversationId { get; set; }
    public string? UserId { get; set; }
}
