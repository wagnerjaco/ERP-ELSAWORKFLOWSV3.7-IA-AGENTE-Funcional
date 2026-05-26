namespace Elsa.Server.Web.DB.Class
{
    public class ChatbotLog
    {
        public Int64 Id { get; set; }
        public Guid RequestId { get; set; }
        public Guid? ConversationId { get; set; }
        public string? UserId { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public string UserMessage { get; set; } = string.Empty;
        public string BotResponse { get; set; } = string.Empty;
        public string ProviderUsed { get; set; } = string.Empty;
        public string ModelUsed { get; set; } = string.Empty;
        public string ErrorDetails { get; set; } = string.Empty;
        public int LatencyMs { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
