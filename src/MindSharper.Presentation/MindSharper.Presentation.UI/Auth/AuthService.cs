namespace MindSharper.Presentation.UI.Auth;

public class AuthService(IHttpClientFactory httpClientFactory) : IAuthService
{
    public async Task<SignInDto> SignInAsync(string email, string password)
    {
        using var httpClient = httpClientFactory.CreateClient();
        var httpResponse = await httpClient.PostAsJsonAsync("http://localhost:5273/api/identity/login", new { email, password });
        var signInDto = await httpResponse.Content.ReadFromJsonAsync<SignInDto>();
        return signInDto;
    }
}