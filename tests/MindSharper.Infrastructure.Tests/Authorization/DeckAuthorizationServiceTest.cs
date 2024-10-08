﻿using System;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Interfaces;
using MindSharper.Infrastructure.Authorization;
using Moq;
using Xunit;

namespace MindSharper.Infrastructure.Tests.Authorization;

[TestSubject(typeof(DeckAuthorizationService))]
public class DeckAuthorizationServiceTest
{
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<ILogger<DeckAuthorizationService>> _loggerMock = new();
    private readonly IResourceAuthorizationService<Deck> _resourceAuthorizationService;
    private readonly CurrentUser _currentUser = new CurrentUser(Guid.NewGuid().ToString(), null, null);
    public DeckAuthorizationServiceTest()
    {
        _resourceAuthorizationService = new DeckAuthorizationService(_loggerMock.Object, _userContextMock.Object);
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(_currentUser);
    }

    [Theory]
    [InlineData(ResourceOperation.Read)]
    [InlineData(ResourceOperation.Delete)]
    [InlineData(ResourceOperation.Update)]
    public void IsAuthorized_ForDeckOwner_ShouldReturnTrue(ResourceOperation operation)
    {
        var deck = new Deck() { UserId = _currentUser.Id };
        
        var result = _resourceAuthorizationService.IsAuthorized(deck, operation);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsAuthorized_ForDeckCreation_ShouldAlwaysReturnTrue()
    {
        var result = _resourceAuthorizationService.IsAuthorized(new Deck(), ResourceOperation.Create);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(ResourceOperation.Delete)]
    [InlineData(ResourceOperation.Update)]
    [InlineData(ResourceOperation.Read)]
    public void IsAuthorized_ForNonDeckOwner_ShouldReturnFalse(ResourceOperation operation)
    {
        var deck = new Deck() { UserId = Guid.NewGuid().ToString() };
        
        var result = _resourceAuthorizationService.IsAuthorized(deck, operation);

        result.Should().BeFalse();
    }
}