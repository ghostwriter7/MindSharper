using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Commands.UpdateFlashcard;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
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

    public UpdateFlashcardCommandHandlerTest()
    {
        _handler = new UpdateFlashcardCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_ForValidRequest_ShouldUpdateFlashcard()
    {
        var existingFlashcard = new Flashcard()
        {
            Id = _command.FlashcardId,
            DeckId = _command.DeckId,
            Frontside = "Old question",
            Backside = "Old answer"
        };

        _repositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId))
            .ReturnsAsync(existingFlashcard);

        await _handler.Handle(_command, CancellationToken.None);

        existingFlashcard.Frontside.Should().Be(_command.Frontside);
        existingFlashcard.Backside.Should().Be(_command.Backside);
        _repositoryMock.Verify(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateFlashcardAsync(It.IsAny<Flashcard>()), Times.Once);
    }

    [Fact]
    public void Handle_ForNonExistingFlashcardId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId))
            .Throws(() => new NotFoundException(nameof(Flashcard), _command.FlashcardId.ToString()));

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Resource {nameof(Flashcard)} identified by {_command.FlashcardId} does not exist.");
        _repositoryMock.Verify(repo => repo.UpdateFlashcardAsync(It.IsAny<Flashcard>()), Times.Never);
    }
}