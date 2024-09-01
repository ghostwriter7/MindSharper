using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MindSharper.API.Middlewares;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using Moq;
using Xunit;

namespace MindSharper.API.Tests.Middlewares;

[TestSubject(typeof(ErrorHandlingMiddleware))]
public class ErrorHandlingMiddlewareTest
{
    private readonly Mock<ILogger<ErrorHandlingMiddleware>> _loggerMock = new();
    private readonly ErrorHandlingMiddleware _errorHandlingMiddleware;
    private readonly HttpContext _httpContext = new DefaultHttpContext();

    public ErrorHandlingMiddlewareTest()
    {
        _errorHandlingMiddleware = new ErrorHandlingMiddleware(_loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoExceptionThrown_ShouldCallNextDelegate()
    {
        var requestDelegateMock = new Mock<RequestDelegate>();

        await _errorHandlingMiddleware.InvokeAsync(_httpContext, requestDelegateMock.Object);

        requestDelegateMock.Verify(requestDelegate => requestDelegate.Invoke(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenDuplicateResourceExceptionThrown_ShouldReturn404BadRequest()
    {
        RequestDelegate requestDelegate = (_) => throw new DuplicateResourceException(nameof(Deck), nameof(Deck.Name), "C#");

        await _errorHandlingMiddleware.InvokeAsync(_httpContext, requestDelegate);

        _httpContext.Response.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundExceptionThrown_ShouldReturn400NotFound()
    {
        RequestDelegate requestDelegate = (_) => throw new NotFoundException(nameof(Deck), 1.ToString());

        await _errorHandlingMiddleware.InvokeAsync(_httpContext, requestDelegate);

        _httpContext.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedExceptionThrown_ShouldReturn403Unauthorized()
    {
        RequestDelegate requestDelegate =
            (_) => throw new UnauthorizedException(nameof(Deck), 1, Guid.NewGuid().ToString());

        await _errorHandlingMiddleware.InvokeAsync(_httpContext, requestDelegate);

        _httpContext.Response.StatusCode.Should().Be(403);
    }
    
    [Fact]
    public async Task InvokeAsync_WhenAnyExceptionThrown_ShouldReturn500InternalServerError()
    {
        RequestDelegate requestDelegate = (_) => throw new Exception("Oops!");

        await _errorHandlingMiddleware.InvokeAsync(_httpContext, requestDelegate);

        _httpContext.Response.StatusCode.Should().Be(500);
    }
}