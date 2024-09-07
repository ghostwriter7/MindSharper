namespace MindSharper.Presentation.UI.Auth;

public interface IAuthService
{
    Task<SignInDto> SignInAsync(string email, string password);
}