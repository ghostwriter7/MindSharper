namespace MindSharper.Application.Users;

public record CurrentUser(string Id, string Email, IEnumerable<string> Roles)
{
    public bool HasRole(string role) => Roles.Contains(role);
};