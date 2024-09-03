using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Decks.Queries.GetDeckByIdQuery;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Application.Users;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;
using MindSharper.Infrastructure.Authorization;
using MindSharper.Tests.Common.Helpers;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Queries.GetDeckById;

[TestSubject(typeof(GetDeckByIdQueryHandler))]
public class GetDeckByIdQueryHandlerTest
{
    private readonly Mock<ILogger<GetDeckByIdQueryHandler>> _loggerMock = new();
    private readonly Mock<IDeckRepository> _deckRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly IResourceAuthorizationService<Deck> _authorizationService;
    private readonly GetDeckByIdQueryHandler _handler;
    private readonly int _deckId = 1;
    private readonly GetDeckByIdQuery _query;

    public GetDeckByIdQueryHandlerTest()
    {
        _authorizationService = new DeckAuthorizationService(new Mock<ILogger<DeckAuthorizationService>>().Object,
            _userContextMock.Object);
        _query = new GetDeckByIdQuery(_deckId);
        _handler = new GetDeckByIdQueryHandler(_loggerMock.Object, _deckRepositoryMock.Object, _mapperMock.Object,
            _userContextMock.Object, _authorizationService);
    }

    [Fact]
    public async Task Handle_ForExistingDeckIdOwnedByUser_ShouldReturnDeckDto()
    {
        var userId = Guid.NewGuid().ToString();
        var deck = DeckFixtures.GetAnyDeck();
        deck.UserId = userId;
        var deckDto = DeckFixtures.GetDeckDtoFromDeck(deck);

        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser(userId, null, null));
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, _query.DeckId, deck);
        _mapperMock.Setup(mapper => mapper.Map<DeckDto>(deck))
            .Returns(deckDto);

        var result = await _handler.Handle(_query, CancellationToken.None);

        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(_query.DeckId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<DeckDto>(deck), Times.Once);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(deckDto);
    }

    [Fact]
    public async Task Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser(Guid.NewGuid().ToString(), null, null));
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, _query.DeckId, null);

        Func<Task> action = async () => await _handler.Handle(_query, CancellationToken.None);
        action.Should()
            .ThrowAsync<NotFoundException>();

        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(_query.DeckId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<DeckDto>(It.IsAny<Deck>()), Times.Never);
    }
    
    [Fact]
    public async Task Handle_ForDeckOwnedByAnotherUser_ShouldThrowUnauthorizedException()
    {
        var userId = Guid.NewGuid().ToString();
        var deck = DeckFixtures.GetAnyDeck();
        deck.UserId = userId;
        var deckDto = DeckFixtures.GetDeckDtoFromDeck(deck);
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser(Guid.NewGuid().ToString(), null, null));
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, _query.DeckId, deck);

        Func<Task> action = async () => await _handler.Handle(_query, CancellationToken.None);
        action.Should()
            .ThrowAsync<UnauthorizedException>();

        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(_query.DeckId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<DeckDto>(It.IsAny<Deck>()), Times.Never);
    }
}