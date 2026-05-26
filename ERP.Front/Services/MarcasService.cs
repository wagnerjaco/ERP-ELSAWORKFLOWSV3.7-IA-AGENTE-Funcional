using ERP.Front.Models;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ERP.Front.Services;

public class MarcasService
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;
    private readonly string baseUrl;

    public MarcasService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
        this.baseUrl = "api/marca";
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

    public async Task<List<Marca>> GetAllAsync()
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, baseUrl);
        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Marca>>() ?? new List<Marca>();
    }

    public async Task<Marca?> GetByIdAsync(Guid id)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"{baseUrl}/{id}");
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Marca>();
        }
        return null;
    }

    public async Task<Marca?> CreateAsync(MarcaForm marca)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Post, baseUrl);
        request.Content = JsonContent.Create(marca);
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Marca>();
        }
        return null;
    }

    public async Task<Marca?> UpdateAsync(Guid id, MarcaForm marca)
    {
        var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put, $"{baseUrl}/{id}");
        request.Content = JsonContent.Create(marca);
        var response = await httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Marca>();
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