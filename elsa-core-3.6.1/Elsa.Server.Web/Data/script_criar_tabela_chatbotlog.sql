IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatbotLog')
BEGIN
    CREATE TABLE ChatbotLog (
        Id              BIGINT IDENTITY(1,1) PRIMARY KEY,
        RequestId       UNIQUEIDENTIFIER NOT NULL,
        AgentName       NVARCHAR(100) NULL,
        UserMessage     NVARCHAR(MAX) NULL,
        BotResponse     NVARCHAR(MAX) NULL,
        ProviderUsed    NVARCHAR(50) NULL,
        ModelUsed       NVARCHAR(100) NULL,
        LatencyMs       INT NULL,
        Status          NVARCHAR(20) NULL,
        ErrorDetails    NVARCHAR(MAX) NULL,
        CreatedAt       DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_ChatbotLog_CreatedAt ON ChatbotLog (CreatedAt DESC);
    CREATE INDEX IX_ChatbotLog_RequestId ON ChatbotLog (RequestId);
    CREATE INDEX IX_ChatbotLog_ProviderUsed ON ChatbotLog (ProviderUsed);
    CREATE INDEX IX_ChatbotLog_AgentName ON ChatbotLog (AgentName);
END
