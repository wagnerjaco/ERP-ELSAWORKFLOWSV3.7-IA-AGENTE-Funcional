using Microsoft.Data.SqlClient;

namespace Elsa.Server.Web.Data;

public class AuditEntry
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
    public long LatencyMs { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ChatAuditService
{
    private readonly IConfiguration _configuration;

    public ChatAuditService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task LogAsync(AuditEntry entry)
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            var sql = @"INSERT INTO ChatbotLog
                (RequestId, ConversationId, UserId, AgentName, UserMessage, BotResponse, ProviderUsed, ModelUsed, LatencyMs, Status, CreatedAt)
                VALUES (@RequestId, @ConversationId, @UserId, @AgentName, @UserMessage, @BotResponse, @ProviderUsed, @ModelUsed, @LatencyMs, @Status, GETDATE())";

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@RequestId", entry.RequestId);
            cmd.Parameters.AddWithValue("@ConversationId", (object?)entry.ConversationId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UserId", (object?)entry.UserId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AgentName", entry.AgentName);
            cmd.Parameters.AddWithValue("@UserMessage", entry.UserMessage);
            cmd.Parameters.AddWithValue("@BotResponse", entry.BotResponse);
            cmd.Parameters.AddWithValue("@ProviderUsed", entry.ProviderUsed);
            cmd.Parameters.AddWithValue("@ModelUsed", entry.ModelUsed);
            cmd.Parameters.AddWithValue("@LatencyMs", entry.LatencyMs);
            cmd.Parameters.AddWithValue("@Status", entry.Status);

            await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Audit log failed: {ex.Message}");
        }
    }

    public async Task<List<AuditEntry>> GetHistoryAsync(Guid conversationId, int limit = 50)
    {
        var results = new List<AuditEntry>();
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            await using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            var sql = @"SELECT Id, RequestId, ConversationId, UserId, AgentName,
                        UserMessage, BotResponse, ProviderUsed,
                        ModelUsed, LatencyMs, Status
                        FROM ChatbotLog
                        WHERE ConversationId = @ConversationId AND Status = 'completed'
                        ORDER BY CreatedAt ASC";

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ConversationId", conversationId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new AuditEntry
                {
                    Id = reader.GetInt64(reader.GetOrdinal("Id")),

                    RequestId = reader.GetGuid(reader.GetOrdinal("RequestId")),

                    AgentName = reader.IsDBNull(reader.GetOrdinal("AgentName")) ? "" : reader.GetString(reader.GetOrdinal("AgentName")),

                    UserMessage = reader.IsDBNull(reader.GetOrdinal("UserMessage")) ? "" : reader.GetString(reader.GetOrdinal("UserMessage")),

                    BotResponse = reader.IsDBNull(reader.GetOrdinal("BotResponse")) ? "" : reader.GetString(reader.GetOrdinal("BotResponse")),

                    ProviderUsed = reader.IsDBNull(reader.GetOrdinal("ProviderUsed")) ? "" : reader.GetString(reader.GetOrdinal("ProviderUsed")),
                    ModelUsed = reader.IsDBNull(reader.GetOrdinal("ModelUsed")) ? "" : reader.GetString(reader.GetOrdinal("ModelUsed")),

                    LatencyMs = reader.IsDBNull(reader.GetOrdinal("LatencyMs")) ? 0 : reader.GetInt32(reader.GetOrdinal("LatencyMs")),

                    Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? "" : reader.GetString(reader.GetOrdinal("Status")),

                    ConversationId = reader.IsDBNull(reader.GetOrdinal("ConversationId")) ? null : reader.GetGuid(reader.GetOrdinal("ConversationId")),
                    UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? null : reader.GetString(reader.GetOrdinal("UserId"))
                });
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetHistory failed: {ex.Message}");
        }
        return results;
    }
}
