using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Commands.DeleteFlashcard;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;
using MindSharper.Tests.Common.Helpers;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Flashcards.Commands.DeleteFlashcard;

[TestSubject(typeof(DeleteFlashcardCommandHandler))]
public class DeleteFlashcardCommandHandlerTest
{
    private readonly Mock<ILogger<DeleteFlashcardCommandHandler>> _loggerMock = new();
    private readonly Mock<IFlashcardRepository> _flashcardRepositoryMock = new();
    private readonly DeleteFlashcardCommand _command = new(1, 2);
    private readonly DeleteFlashcardCommandHandler _handler;
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IDeckRepository> _deckRepositoryMock = new();
    private readonly Mock<IResourceAuthorizationService<Deck>> _authorizationServiceMock = new();
    private readonly CurrentUser _currentUser = new CurrentUser(Guid.NewGuid().ToString(), null, null);
    private const ResourceOperation _operation = ResourceOperation.Update;

    public DeleteFlashcardCommandHandlerTest()
    {
        _handler = new DeleteFlashcardCommandHandler(_loggerMock.Object, _flashcardRepositoryMock.Object,
            _deckRepositoryMock.Object,
            _userContextMock.Object,
            _authorizationServiceMock.Object);

        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(_currentUser);
    }

    [Fact]
    public void Handle_ForExistingFlashcardId_ShouldRemoveIt()
    {
        var deck = DeckFixtures.GetAnyDeck();
        SetUpDeckRepository(deck);
        SetupHelper.SetUpAuthorizationService(_authorizationServiceMock, deck, _operation, true);;
        var flashcard = FlashcardFixtures.GetAnyFlashcard();
        _flashcardRepositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId))
            .ReturnsAsync(flashcard);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().NotThrowAsync();
        _flashcardRepositoryMock.Verify(repo => repo.DeleteFlashcardAsync(flashcard), Times.Once);
        _authorizationServiceMock.Verify(authService => authService.IsAuthorized(deck, _operation), Times.Once);
    }

    [Fact]
    public void Handle_ForNonExistingFlashcardId_ShouldThrowNotFoundException()
    {
        var deck = DeckFixtures.GetAnyDeck();
        SetUpDeckRepository(deck);
        SetupHelper.SetUpAuthorizationService(_authorizationServiceMock, deck, _operation, true);;
        _flashcardRepositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId))
            .ReturnsAsync((Flashcard)null);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Resource {nameof(Flashcard)} identified by {_command.FlashcardId} does not exist.");
        _flashcardRepositoryMock.Verify(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId),
            Times.Once);
        _authorizationServiceMock.Verify(authService => authService.IsAuthorized(deck, _operation), Times.Once);
        _flashcardRepositoryMock.Verify(repo => repo.DeleteFlashcardAsync(It.IsAny<Flashcard>()), Times.Never);
    }

    [Fact]
    public void Handle_ForDeckBelongingToAnotherUser_ShouldThrowUnauthorizedException()
    {
        var deck = DeckFixtures.GetAnyDeck();
        SetUpDeckRepository(deck);
        SetupHelper.SetUpAuthorizationService(_authorizationServiceMock, deck, _operation, false);

        var action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<UnauthorizedException>();
        _authorizationServiceMock.Verify(authService => authService.IsAuthorized(deck, _operation), Times.Once);
        _flashcardRepositoryMock.Verify(repo => repo.DeleteFlashcardAsync(It.IsAny<Flashcard>()), Times.Never);
    }
    
    private void SetUpDeckRepository([CanBeNull] Deck deck) => _deckRepositoryMock
        .Setup(repo => repo.GetDeckByIdAsync(_command.DeckId)).ReturnsAsync(deck);
}