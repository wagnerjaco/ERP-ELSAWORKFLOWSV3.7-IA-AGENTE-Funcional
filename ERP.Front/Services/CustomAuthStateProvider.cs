using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace ERP.Front.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime jsRuntime;

    public CustomAuthStateProvider(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "accessToken");

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var userDataJson = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", "userData");

        if (string.IsNullOrEmpty(userDataJson))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "user"),
            new(ClaimTypes.Name, "user"),
            new(ClaimTypes.Role, "Authenticated")
        };

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public void MarkUserAsAuthenticated(string userJson)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "user"),
            new(ClaimTypes.Name, "user"),
            new(ClaimTypes.Role, "Authenticated")
        };

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        var principal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }
}