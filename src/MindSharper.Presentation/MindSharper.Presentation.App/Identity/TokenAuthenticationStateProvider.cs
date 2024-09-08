using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using MindSharper.Presentation.App.Identity.Models;
using AccessTokenResult = MindSharper.Presentation.App.Identity.Models.AccessTokenResult;

namespace MindSharper.Presentation.App.Identity;

using AccessTokenResult = Models.AccessTokenResult;

public class TokenAuthenticationStateProvider(IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime)
    : AuthenticationStateProvider, IAccountManager
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");
    private bool _authenticated;
    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;
        var user = _unauthenticated;

        try
        {
            var response = await _httpClient.GetAsync("api/identity/manage/info");
            response.EnsureSuccessStatusCode();

            var userJson = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, _jsonSerializerOptions);

            if (userInfo != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, userInfo.Email),
                    new(ClaimTypes.Email, userInfo.Email)
                };

                claims.AddRange(userInfo.Claims.Where(c => c.Key != ClaimTypes.Name && c.Key != ClaimTypes.Email)
                    .Select(c => new Claim(c.Key, c.Value)));

                var id = new ClaimsIdentity(claims, nameof(TokenAuthenticationStateProvider));
                user = new ClaimsPrincipal(id);
                _authenticated = true;
            }
        }
        catch
        {
        }

        return new AuthenticationState(user);
    }
    

    public async Task<FormResult> LoginAsync(string email, string password)
    {
        try
        {
            var response =
                await _httpClient.PostAsJsonAsync("http://localhost:5273/api/identity/login", new { email, password });

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<AccessTokenResult>();
                await jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", content.AccessToken);
                
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

                return new FormResult()
                {
                    Succeeded = true
                };
            }
        }
        catch
        {
        }

        return null;
    }

    public async Task LogoutAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<FormResult> RegisterAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CheckAuthenticatedState()
    {
        throw new NotImplementedException();
    }
}