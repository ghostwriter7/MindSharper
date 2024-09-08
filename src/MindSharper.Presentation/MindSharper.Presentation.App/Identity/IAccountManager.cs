using MindSharper.Presentation.App.Identity.Models;

namespace MindSharper.Presentation.App.Identity;

public interface IAccountManager
{
    public Task<FormResult> LoginAsync(string email, string password);
    public Task LogoutAsync();
    public Task<FormResult> RegisterAsync(string email, string password);
    public Task<bool> CheckAuthenticatedState();
}