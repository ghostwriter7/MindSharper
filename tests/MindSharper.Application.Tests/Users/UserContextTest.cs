using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using MindSharper.Application.Users;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Users;

[TestSubject(typeof(UserContext))]
public class UserContextTest
{
    private readonly Mock<HttpContext> _httpContextMock = new();
    private readonly IUserContext _userContext;

    public UserContextTest()
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(context => context.HttpContext).Returns(_httpContextMock.Object);
        _userContext = new UserContext(httpContextAccessor.Object);
    }

    [Fact]
    public void GetCurrentUser_WhenNoUserContextAvailable_ShouldThrowInvalidOperationException()
    {
        _httpContextMock.Setup(httpContext => httpContext.User).Returns((ClaimsPrincipal)null);

        Func<CurrentUser?> action = () => _userContext.GetCurrentUser();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("User context is not available");
    }

    [Fact]
    public void GetCurrentUser_WhenUserContextAvailable_ShouldReturnCurrentUser()
    {
        const string email = "hello@world.com";
        var id = Guid.NewGuid().ToString();
        var role = "Admin";
        var claimsIdentities = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, id),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
        ], "TestAuthenticationType");
        var claimsPrincipal = new ClaimsPrincipal([claimsIdentities]);
        _httpContextMock.Setup(httpContext => httpContext.User).Returns(claimsPrincipal);

        var currentUser = _userContext.GetCurrentUser();

        currentUser.Should().NotBeNull();
        currentUser.Email.Should().Be(email);
        currentUser.Id.Should().Be(id);
        currentUser.Roles.Should().BeEquivalentTo([role]);
    }
}