using System.Text.Json;

namespace MindSharper.Presentation.UI.Auth;

public record SignInDto(
    string TokenType,
    string AccessToken,
    int ExpiresIn,
    string RefreshToken);