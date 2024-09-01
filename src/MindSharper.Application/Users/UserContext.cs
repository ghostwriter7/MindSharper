using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MindSharper.Application.Users;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public CurrentUser? GetCurrentUser()
    {
        var user = httpContextAccessor?.HttpContext?.User
                   ?? throw new InvalidOperationException("User context is not available");

        if (user.Identity is null || !user.Identity.IsAuthenticated)
            return null;

        var id = user.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value;
        var email = user.FindFirst(claim => claim.Type == ClaimTypes.Email)!.Value;
        var roles = user.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value);

        return new CurrentUser(id, email, roles);
    }
}