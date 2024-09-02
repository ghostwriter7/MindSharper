using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Commands.UpdateDeckName;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Commands.UpdateDeckName;

[TestSubject(typeof(UpdateDeckNameCommandHandler))]
public class UpdateDeckNameCommandHandlerTest
{
    private readonly Mock<ILogger<UpdateDeckNameCommandHandler>> _loggerMock = new();
    private readonly Mock<IDeckRepository> _repositoryMock = new();
    private readonly UpdateDeckNameCommandHandler _handler;
    private readonly UpdateDeckNameCommand _command = new(100, "New name");
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IResourceAuthorizationService<Deck>> _authorizationServiceMock = new();

    public UpdateDeckNameCommandHandlerTest()
    {
        _handler = new UpdateDeckNameCommandHandler(_loggerMock.Object, _repositoryMock.Object, _userContextMock.Object,
            _authorizationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ForValidRequest_ShouldUpdateName()
    {
        var userId = Guid.NewGuid().ToString();
        var deck = DeckFixtures.GetAnyDeck();
        deck.UserId = userId;
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync(deck);
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser(userId, null, null));
        _authorizationServiceMock.Setup(authService => authService.IsAuthorized(deck, ResourceOperation.Update))
            .Returns(true);
        
        await _handler.Handle(_command, CancellationToken.None);

        deck.Name.Should().Be(_command.Name);
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _authorizationServiceMock.Verify(authService => authService.IsAuthorized(deck, ResourceOperation.Update), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateDeckAsync(deck), Times.Once);
    }


    [Fact]
    public void Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser("any id", null, null));
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync((Deck)null);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<NotFoundException>();
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateDeckAsync(It.IsAny<Deck>()), Times.Never);
    }
    
    [Fact]
    public void Handle_ForDeckOwnedByAnotherUser_ShouldThrowUnauthroizedException()
    {
        var deck = DeckFixtures.GetAnyDeck();
        deck.UserId = Guid.NewGuid().ToString();
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser(Guid.NewGuid().ToString(), null, null));
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync(deck);
        _authorizationServiceMock.Setup(authService => authService.IsAuthorized(deck, ResourceOperation.Update))
            .Returns(false);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<UnauthorizedException>();
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _authorizationServiceMock.Verify(authService => authService.IsAuthorized(deck, ResourceOperation.Update), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateDeckAsync(It.IsAny<Deck>()), Times.Never);
    }
}