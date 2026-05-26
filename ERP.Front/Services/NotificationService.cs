using ERP.Front.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ERP.Front.Services;

public class NotificationService
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;
    private readonly string baseUrl;

    public NotificationService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
        this.baseUrl = "api";
    }

    private async Task<string?> GetTokenAsync()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
    }

    private async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(HttpMethod method, string url)
    {
        var token = await GetTokenAsync();
        var request = new HttpRequestMessage(method, url);
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        return request;
    }

    public async Task<NotificationResponse> GetNotificationsAsync()
    {
        var response = new NotificationResponse();

        try
        {
            var pedidosRequest = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"{baseUrl}/pedidocompra/pendentes");
            var pedidosResponse = await httpClient.SendAsync(pedidosRequest);
            if (pedidosResponse.IsSuccessStatusCode)
            {
                var pedidos = await pedidosResponse.Content.ReadFromJsonAsync<List<PedidoCompra>>() ?? new List<PedidoCompra>();
                response.PedidosPendentes = pedidos.Select(p => new NotificationItem
                {
                    Id = p.Id,
                    Type = "pedido",
                    Title = $"Pedido #{p.NumeroPedido}",
                    Subtitle = p.Fornecedor?.Nome ?? "Fornecedor não informado",
                    SecondaryText = $"{p.Quantidade} {p.Unidade} - {p.ValorTotal:C2}",
                    Url = $"/pedidoscompras/{p.Id}",
                    WindowId = $"pedido-notification-{p.Id}"
                }).ToList();
            }
        }
        catch { }

        try
        {
            var estoqueRequest = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"{baseUrl}/produto/estoque-baixo");
            var estoqueResponse = await httpClient.SendAsync(estoqueRequest);
            if (estoqueResponse.IsSuccessStatusCode)
            {
                var produtos = await estoqueResponse.Content.ReadFromJsonAsync<List<Produto>>() ?? new List<Produto>();
                response.EstoqueBaixo = produtos.Select(p => new NotificationItem
                {
                    Id = p.Id,
                    Type = "estoque",
                    Title = p.Descricao,
                    Subtitle = $"Atual: {p.EstoqueAtual} | Minimo: {p.EstoqueMinimo}",
                    SecondaryText = p.Sku ?? "",
                    Url = $"/pedidoscompras/widget/{p.Id}",
                    WindowId = $"estoque-widget-{p.Id}"
                }).ToList();
            }
        }
        catch { }

        response.TotalCount = response.PedidosPendentes.Count + response.EstoqueBaixo.Count;
        return response;
    }
}

public class NotificationResponse
{
    public List<NotificationItem> PedidosPendentes { get; set; } = new();
    public List<NotificationItem> EstoqueBaixo { get; set; } = new();
    public int TotalCount { get; set; }
}

public class NotificationItem
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string SecondaryText { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string WindowId { get; set; } = string.Empty;
}