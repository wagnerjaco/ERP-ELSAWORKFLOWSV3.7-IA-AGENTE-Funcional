using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Elsa.Server.Web.Options;

namespace Elsa.Server.Web.Providers;

public class OpenAICompatibleProvider : IChatProvider
{
    private static readonly JsonSerializerOptions JsonReadOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task<ProviderResult> SendAsync(
        ChatProviderOptions options,
        string systemPrompt,
        string userMessage,
        List<object> skills,
        CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();

        var functions = BuildFunctionDefinitions(skills);
        var toolsJson = BuildToolsJson(functions);

        var json = BuildChatRequestJson(options.DefaultModel, systemPrompt, userMessage, toolsJson);
        System.Console.Error.WriteLine($"OPENAI_DEBUG: Request JSON: {Truncate(json, 2000)}");
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", options.ApiKey);

        var url = options.BaseUrl.TrimEnd('/') + "/chat/completions";

        var response = await httpClient.PostAsync(url, content, ct);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(ct);
        System.Console.Error.WriteLine($"OPENAI_DEBUG: First response: {Truncate(responseJson, 500)}");
        var result = JsonSerializer.Deserialize<ChatCompletionResponse>(responseJson, JsonReadOptions);

        if (result?.Choices == null || result.Choices.Length == 0)
            return new ProviderResult { Success = false, Error = "Resposta vazia do provedor" };

        var choice = result.Choices[0];
        var message = choice.Message ?? new MessageContent();

        if (message.ToolCalls?.Count > 0)
        {
            System.Console.Error.WriteLine($"OPENAI_DEBUG: {message.ToolCalls.Count} tool call(s) received");

            var results = new List<string>();
            foreach (var toolCall in message.ToolCalls)
            {
                var funcName = toolCall.Function?.Name ?? "";
                var args = toolCall.Function?.Arguments ?? "{}";

                System.Console.Error.WriteLine($"OPENAI_DEBUG: Executing {funcName}({args})");
                var skillResult = await ExecuteSkillFunction(skills, funcName, args);

                var label = funcName switch
                {
                    "ConsultarEstoque" => "Consulta de Estoque",
                    "ConsultarPedidos" => "Consulta de Pedidos",
                    "ConsultarFornecedores" => "Consulta de Fornecedores",
                    _ => funcName
                };
                results.Add($"{label}:\n{skillResult}");
            }

            sw.Stop();
            return new ProviderResult
            {
                Response = string.Join("\n\n", results),
                ModelUsed = options.DefaultModel,
                LatencyMs = sw.ElapsedMilliseconds,
                Success = true
            };
        }

        sw.Stop();
        return new ProviderResult
        {
            Response = message.Content ?? "",
            ModelUsed = options.DefaultModel,
            LatencyMs = sw.ElapsedMilliseconds,
            Success = true
        };
    }

    private static string JsEncode(string s) => JsonSerializer.Serialize(s);

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "...";

    private static string BuildToolsJson(List<FunctionDef> functions)
    {
        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i < functions.Count; i++)
        {
            if (i > 0) sb.Append(',');
            var f = functions[i];
            sb.Append("{\"type\":\"function\",\"function\":{");
            sb.Append("\"name\":").Append(JsEncode(f.Name)).Append(',');
            sb.Append("\"description\":").Append(JsEncode(f.Description)).Append(',');
            sb.Append("\"parameters\":").Append(f.ParametersJson);
            sb.Append("}}");
        }
        sb.Append(']');
        return sb.ToString();
    }

    private static string BuildChatRequestJson(
        string model,
        string systemPrompt,
        string userMessage,
        string toolsJson)
    {
        var sb = new StringBuilder();
        sb.Append("{\"model\":").Append(JsEncode(model)).Append(',');
        sb.Append("\"messages\":[");
        sb.Append("{\"role\":\"system\",\"content\":").Append(JsEncode(systemPrompt)).Append('}');
        sb.Append(",{\"role\":\"user\",\"content\":").Append(JsEncode(userMessage)).Append('}');
        sb.Append("],\"tools\":").Append(toolsJson);
        sb.Append(",\"temperature\":0.1}");
        return sb.ToString();
    }    private List<FunctionDef> BuildFunctionDefinitions(List<object> skills)
    {
        var functions = new List<FunctionDef>();

        foreach (var skill in skills)
        {
            var type = skill.GetType();
            var typeDesc = type.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true)
                .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;

            foreach (var method in type.GetMethods())
            {
                var kernelFuncAttrObj = method.GetCustomAttributes(typeof(Microsoft.SemanticKernel.KernelFunctionAttribute), true)
                    .FirstOrDefault() as Microsoft.SemanticKernel.KernelFunctionAttribute;
                if (kernelFuncAttrObj == null) continue;

                var funcName = kernelFuncAttrObj.Name ?? method.Name;
                var funcDesc = method.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true)
                    .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;

                var paramProps = new Dictionary<string, object>();
                var requiredParams = new List<string>();

                foreach (var param in method.GetParameters())
                {
                    var paramType = param.ParameterType == typeof(string) ? "string" : "number";

                    var prop = new Dictionary<string, object>
                    {
                        ["type"] = paramType
                    };

                    paramProps[param.Name ?? "param"] = prop;
                    requiredParams.Add(param.Name ?? "param");
                }

                functions.Add(new FunctionDef
                {
                    Name = funcName,
                    Description = funcDesc?.Description ?? typeDesc?.Description ?? "",
                    Parameters = new Dictionary<string, object>
                    {
                        ["type"] = "object",
                        ["properties"] = paramProps,
                        ["required"] = requiredParams
                    }
                });
            }
        }

        return functions;
    }

    private async Task<string> ExecuteSkillFunction(List<object> skills, string functionName, string argumentsJson)
    {
        foreach (var skill in skills)
        {
            var type = skill.GetType();
            foreach (var method in type.GetMethods())
            {
                var kernelFuncAttr = method.GetCustomAttributes(typeof(Microsoft.SemanticKernel.KernelFunctionAttribute), true);
                if (kernelFuncAttr.Length == 0) continue;

                var methodName = method.GetCustomAttributes(typeof(Microsoft.SemanticKernel.KernelFunctionAttribute), true)
                    .FirstOrDefault() as Microsoft.SemanticKernel.KernelFunctionAttribute;

                var funcName = methodName?.Name ?? method.Name;
                if (!funcName.Equals(functionName, StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    var args = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(argumentsJson);
                    System.Console.Error.WriteLine($"SKILL_DEBUG: Executing {funcName} with args: {argumentsJson}");
                    var methodParams = method.GetParameters();
                    var callArgs = new List<object?>();

                    foreach (var param in methodParams)
                    {
                        if (args != null && args.TryGetValue(param.Name ?? "", out var jsonVal))
                        {
                            var rawText = jsonVal.GetRawText();
                            object? val = null;

                            try
                            {
                                val = JsonSerializer.Deserialize(rawText, param.ParameterType);
                            }
                            catch
                            {
                                if (jsonVal.ValueKind == JsonValueKind.Object)
                                {
                                    if (jsonVal.TryGetProperty("description", out var descProp) && descProp.ValueKind == JsonValueKind.String)
                                    {
                                        var desc = descProp.GetString() ?? "";
                                        var colonIdx = desc.LastIndexOf(':');
                                        val = colonIdx >= 0 ? desc[(colonIdx + 1)..].Trim() : desc;
                                    }
                                    else if (jsonVal.TryGetProperty("value", out var valueProp) && valueProp.ValueKind == JsonValueKind.String)
                                    {
                                        val = valueProp.GetString();
                                    }
                                    else if (jsonVal.TryGetProperty("type", out var typeProp) && typeProp.ValueKind == JsonValueKind.String)
                                    {
                                        var typeVal = typeProp.GetString() ?? "";
                                        if (typeVal is not ("string" or "number" or "integer" or "boolean" or "object" or "array" or "null"))
                                            val = typeVal;
                                        else
                                            val = rawText;
                                    }
                                    else if (jsonVal.TryGetProperty("name", out var nameProp) && nameProp.ValueKind == JsonValueKind.String)
                                    {
                                        val = nameProp.GetString();
                                    }
                                    else
                                    {
                                        val = rawText;
                                    }
                                }
                                else
                                {
                                    val = jsonVal.ToString();
                                }
                            }

                            callArgs.Add(val);
                        }
                        else if (param.HasDefaultValue)
                        {
                            callArgs.Add(param.DefaultValue);
                        }
                        else
                        {
                            callArgs.Add(param.ParameterType.IsValueType
                                ? Activator.CreateInstance(param.ParameterType)
                                : null);
                        }
                    }

                    var task = (Task?)method.Invoke(skill, callArgs.ToArray());
                    if (task == null) return "Erro ao invocar funcao";

                    await task.ConfigureAwait(false);
                    var resultProperty = task.GetType().GetProperty("Result");
                    return resultProperty?.GetValue(task)?.ToString() ?? "Funcao executada sem retorno";
                }
                catch (Exception ex)
                {
                    return $"Erro ao executar {funcName}: {ex.Message}";
                }
            }
        }

        return $"Funcao {functionName} nao encontrada";
    }
}

public class FunctionDef
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, object> Parameters { get; set; } = new();

    [JsonIgnore]
    public string? ParametersJson
    {
        get
        {
            if (Parameters == null || Parameters.Count == 0) return "{}";
            return JsonSerializer.Serialize(Parameters);
        }
    }
}

public class ChatCompletionResponse
{
    public Choice[]? Choices { get; set; }
}

public class Choice
{
    public MessageContent? Message { get; set; }
}

public class MessageContent
{
    public string? Content { get; set; }
    public string? Reasoning { get; set; }
    public List<ToolCallContent>? ToolCalls { get; set; }
}

public class ToolCallContent
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public ToolFunctionContent? Function { get; set; }
}

public class ToolFunctionContent
{
    public string? Name { get; set; }
    public string? Arguments { get; set; }
}


