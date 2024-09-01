namespace MindSharper.Application.Users;

public interface IUserContext
{
    public CurrentUser? GetCurrentUser();
}