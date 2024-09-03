using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Commands.UpdateFlashcard;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;
using MindSharper.Tests.Common.Helpers;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Flashcards.Commands.UpdateFlashcard;

[TestSubject(typeof(UpdateFlashcardCommandHandler))]
public class UpdateFlashcardCommandHandlerTest
{
    private readonly Mock<ILogger<UpdateFlashcardCommandHandler>> _loggerMock = new();
    private readonly Mock<IFlashcardRepository> _repositoryMock = new();

    private readonly UpdateFlashcardCommand _command = new UpdateFlashcardCommand()
    {
        FlashcardId = 1,
        DeckId = 1,
        Frontside = "New question",
        Backside = "New answer"
    };

    private readonly UpdateFlashcardCommandHandler _handler;
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IResourceAuthorizationService<Deck>> _authorizationServiceMock = new();
    private readonly Mock<IDeckRepository> _deckRepository = new();
    private readonly CurrentUser _currentUser = new CurrentUser(Guid.NewGuid().ToString(), null, null);
    private const ResourceOperation _operation = ResourceOperation.Update;

    public UpdateFlashcardCommandHandlerTest()
    {
        _handler = new UpdateFlashcardCommandHandler(_loggerMock.Object,
            _repositoryMock.Object,
            _deckRepository.Object, _userContextMock.Object, _authorizationServiceMock.Object);
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(_currentUser);
    }

    [Fact]
    public async Task Handle_ForValidRequest_ShouldUpdateFlashcard()
    {
        var deck = new Deck() { UserId = _currentUser.Id, Id = _command.DeckId };
        var existingFlashcard = new Flashcard()
        {
            Id = _command.FlashcardId,
            DeckId = _command.DeckId,
            Frontside = "Old question",
            Backside = "Old answer"
        };

        SetUpDeckRepository(deck);
        SetupHelper.SetUpAuthorizationService(_authorizationServiceMock, deck, _operation, true);

        _repositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId))
            .ReturnsAsync(existingFlashcard);

        await _handler.Handle(_command, CancellationToken.None);

        existingFlashcard.Frontside.Should().Be(_command.Frontside);
        existingFlashcard.Backside.Should().Be(_command.Backside);
        _repositoryMock.Verify(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateFlashcardAsync(It.IsAny<Flashcard>()), Times.Once);
        _authorizationServiceMock.Verify(authService => authService.IsAuthorized(deck, _operation),
            Times.Once);
    }

    [Fact]
    public void Handle_ForNonExistingFlashcardId_ShouldThrowNotFoundException()
    {
        SetUpDeckRepository(new Deck());
        _repositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId))
            .Throws(() => new NotFoundException(nameof(Flashcard), _command.FlashcardId.ToString()));

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Resource {nameof(Flashcard)} identified by {_command.FlashcardId} does not exist.");
        _repositoryMock.Verify(repo => repo.UpdateFlashcardAsync(It.IsAny<Flashcard>()), Times.Never);
    }

    [Fact]
    public void Handle_ForDeckBelongingToAnotherUser_ShouldThrowUnauthorizedException()
    {
        var deck = new Deck() { UserId = Guid.NewGuid().ToString() };
        SetUpDeckRepository(deck);
        SetupHelper.SetUpAuthorizationService(_authorizationServiceMock, deck, _operation, false);

        var action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<UnauthorizedException>();
        _repositoryMock.Verify(repo => repo.UpdateFlashcardAsync(It.IsAny<Flashcard>()), Times.Never);
        _authorizationServiceMock.Verify(authService => authService.IsAuthorized(deck, _operation),
            Times.Once);
    }

    private void SetUpDeckRepository([CanBeNull] Deck deck) =>
        _deckRepository.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId)).ReturnsAsync(deck);
}