using ERP.Front.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ERP.Front.Services;

public class FornecedoresService
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;
    private readonly string baseUrl;

    public FornecedoresService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
        this.baseUrl = "api/fornecedor";
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

    public async Task<List<Fornecedor>> GetAllAsync()
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, baseUrl);
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Fornecedor>>() ?? new List<Fornecedor>();
    }

    public async Task<Fornecedor?> GetByIdAsync(Guid id)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"{baseUrl}/{id}");
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Fornecedor>();
        }
        return null;
    }

    public async Task<Fornecedor?> CreateAsync(FornecedorForm fornecedor)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, baseUrl);
        request.Content = JsonContent.Create(fornecedor);
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Fornecedor>();
        }
        return null;
    }

    public async Task<Fornecedor?> UpdateAsync(Guid id, FornecedorForm fornecedor)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put, $"{baseUrl}/{id}");
        request.Content = JsonContent.Create(fornecedor);
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Fornecedor>();
        }
        return null;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Delete, $"{baseUrl}/{id}");
        var response = await httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }
}