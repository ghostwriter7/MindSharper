using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Commands.CreateDeck;
using MindSharper.Application.Decks.Commands.DeleteDeck;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Commands.DeleteDeck;

[TestSubject(typeof(DeleteDeckCommandHandler))]
public class DeleteDeckCommandHandlerTest
{
    private readonly Mock<ILogger<DeleteDeckCommandHandler>> _loggerMock = new();
    private readonly Mock<IDeckRepository> _repositoryMock = new();
    private readonly DeleteDeckCommandHandler _handler;
    private readonly Mock<IResourceAuthorizationService<Deck>> _authServiceMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly DeleteDeckCommand _command = new(100);

    public DeleteDeckCommandHandlerTest()
    {
        _handler = new DeleteDeckCommandHandler(_loggerMock.Object, _repositoryMock.Object, _userContextMock.Object,
            _authServiceMock.Object);
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser(Guid.NewGuid().ToString(), "any@email.com", null));
    }

    [Fact]
    public void Handle_ForValidRequest_ShouldRemoveIt()
    {
        var deck = DeckFixtures.GetAnyDeck();
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync(deck);
        _authServiceMock.Setup(authService => authService.IsAuthorized(deck, ResourceOperation.Delete))
            .Returns(true);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().NotThrowAsync();
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _repositoryMock.Verify(repo => repo.DeleteDeckAsync(deck), Times.Once);
    }


    [Fact]
    public void Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync((Deck)null);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<NotFoundException>();
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _repositoryMock.Verify(repo => repo.DeleteDeckAsync(It.IsAny<Deck>()), Times.Never);
    }
    
    [Fact]
    public void Handle_ForDeckOwnerByAnotherUser_ShouldThrowUnauthorizedException()
    {
        var deck = DeckFixtures.GetAnyDeck();
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync(deck);
        _authServiceMock.Setup(authService => authService.IsAuthorized(deck, ResourceOperation.Delete))
            .Returns(false);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<UnauthorizedException>();
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _repositoryMock.Verify(repo => repo.DeleteDeckAsync(deck), Times.Never);
    }
}