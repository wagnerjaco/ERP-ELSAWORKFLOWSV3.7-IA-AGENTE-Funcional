using ERP.Front.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ERP.Front.Services;

public class PedidoComprasService
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;
    private readonly string baseUrl;

    public PedidoComprasService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
        this.baseUrl = "api/pedidocompra";
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

    public async Task<List<PedidoCompra>> GetAllAsync()
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, baseUrl);
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<PedidoCompra>>() ?? new List<PedidoCompra>();
    }

    public async Task<PedidoCompra?> GetByIdAsync(Guid id)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"{baseUrl}/{id}");
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PedidoCompra>();
        }
        return null;
    }

    public async Task<PedidoCompra?> CreateAsync(PedidoCompraForm pedido)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, baseUrl);
        request.Content = JsonContent.Create(pedido);
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PedidoCompra>();
        }
        return null;
    }

    public async Task<PedidoCompra?> UpdateAsync(Guid id, PedidoCompraForm pedido)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put, $"{baseUrl}/{id}");
        request.Content = JsonContent.Create(pedido);
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PedidoCompra>();
        }
        return null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Delete, $"{baseUrl}/{id}");
        var response = await httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AprovarAsync(Guid id, string email)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, $"{baseUrl}/{id}/aprovar");
        request.Content = JsonContent.Create(new { Email = email });
        var response = await httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}