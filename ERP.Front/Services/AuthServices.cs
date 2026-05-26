using ERP.Front.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;

namespace ERP.Front.Services;

public class AuthService
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jsRuntime;
    private readonly NavigationManager navigation;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, NavigationManager navigation)
    {
        this.httpClient = httpClient;
        this.jsRuntime = jsRuntime;
        this.navigation = navigation;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonSerializer.Deserialize<JsonElement>(json);

                var token = doc.GetProperty("token").GetString();
                var usuarioElement = doc.GetProperty("usuario");

                if (token != null)
                {
                    var user = new UserData
                    {
                        Id = usuarioElement.GetProperty("id").GetString() ?? "",
                        UsuarioLogin = usuarioElement.GetProperty("usuarioLogin").GetString() ?? "",
                        Nome = usuarioElement.GetProperty("nome").GetString() ?? "",
                        Email = usuarioElement.GetProperty("email").GetString() ?? "",
                        Permissao = Enum.Parse<Permissao>(usuarioElement.GetProperty("permissao").GetString() ?? "USUARIO")
                    };

                    await SaveToken(token);
                    await SaveUserData(user);

                    var newConvId = Guid.NewGuid().ToString();
                    await jsRuntime.InvokeVoidAsync("localStorage.setItem", "chatbot_conversationId", newConvId);

                    return LoginResult.Success(user);
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return LoginResult.Failure($"Erro ao fazer login: {response.StatusCode} - {errorContent}");
        }
        catch (HttpRequestException ex)
        {
            return LoginResult.Failure($"Erro de conexão com a API: {ex.Message}");
        }
        catch (Exception ex)
        {
            return LoginResult.Failure($"Erro inesperado: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userData");
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", "chatbot_conversationId");
        navigation.NavigateTo("/login");
    }

    public async Task<string?> GetTokenAsync()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    private async Task SaveToken(string token)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
    }

    private async Task SaveRefreshToken(string refreshToken)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", refreshToken);
    }

    private async Task SaveUserData(UserData user)
    {
        var userJson = JsonSerializer.Serialize(user);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "userData", userJson);
    }

    public async Task<UserData?> GetUserDataAsync()
    {
        var userData = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "userData");
        if (string.IsNullOrEmpty(userData))
            return null;

        return JsonSerializer.Deserialize<UserData>(userData);
    }

    public async Task<LoginResult> RegisterAsync(RegisterRequest registerRequest)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/auth/register", registerRequest);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonSerializer.Deserialize<JsonElement>(json);

                var token = doc.GetProperty("token").GetString();
                var usuarioElement = doc.GetProperty("usuario");

                if (token != null)
                {
                    var user = new UserData
                    {
                        Id = usuarioElement.GetProperty("id").GetString() ?? "",
                        UsuarioLogin = usuarioElement.GetProperty("usuarioLogin").GetString() ?? "",
                        Nome = usuarioElement.GetProperty("nome").GetString() ?? "",
                        Email = usuarioElement.GetProperty("email").GetString() ?? "",
                        Permissao = Enum.Parse<Permissao>(usuarioElement.GetProperty("permissao").GetString() ?? "USUARIO")
                    };

                    await SaveToken(token);
                    await SaveUserData(user);

                    return LoginResult.Success(user);
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return LoginResult.Failure($"Erro ao cadastrar: {response.StatusCode} - {errorContent}");
        }
        catch (HttpRequestException ex)
        {
            return LoginResult.Failure($"Erro de conexão com a API: {ex.Message}");
        }
        catch (Exception ex)
        {
            return LoginResult.Failure($"Erro inesperado: {ex.Message}");
        }
    }
}

// 📦 Classe de resultado padronizada
//public class LoginResult
//{
//    public bool IsSuccess { get; private set; }
//    public string? ErrorMessage { get; private set; }
//    public UserData? User { get; private set; }

//    public static LoginResult Success(UserData user) => new()
//    {
//        IsSuccess = true,
//        User = user
//    };

//    public static LoginResult Failure(string message) => new()
//    {
//        IsSuccess = false,
//        ErrorMessage = message
//    };

//}
//public class LoginResponse
//{
//    public string Token { get; set; } = string.Empty;
//    public string RefreshToken { get; set; } = string.Empty;
//    public DateTime ExpiresIn { get; set; }
//    public UserData User { get; set; } = new();
//}