using ERP.Front.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ERP.Front.Services;

public class ProdutosService
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;
    private readonly string baseUrl;

    public ProdutosService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
        this.baseUrl = "api/produto";
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

    public async Task<List<Produto>> GetAllAsync()
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, baseUrl);
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Produto>>() ?? new List<Produto>();
    }

    public async Task<Produto?> GetByIdAsync(Guid id)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"{baseUrl}/{id}");
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Produto>();
        }
        return null;
    }

    public async Task<Produto?> CreateAsync(ProdutoForm produto)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, baseUrl);
        request.Content = JsonContent.Create(produto);
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Produto>();
        }
        return null;
    }

    public async Task<Produto?> UpdateAsync(Guid id, ProdutoForm produto)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put, $"{baseUrl}/{id}");
        request.Content = JsonContent.Create(produto);
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Produto>();
        }
        return null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Delete, $"{baseUrl}/{id}");
        var response = await httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<ProdutoParaPedidoDto?> GetParaPedidoAsync(Guid id)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"{baseUrl}/{id}/para-pedido");
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProdutoParaPedidoDto>();
        }
        return null;
    }
}