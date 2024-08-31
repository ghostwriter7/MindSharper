using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Commands.DeleteFlashcard;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
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

    public DeleteFlashcardCommandHandlerTest()
    {
        _handler = new DeleteFlashcardCommandHandler(_loggerMock.Object, _flashcardRepositoryMock.Object);
    }

    [Fact]
    public void Handle_ForExistingFlashcardId_ShouldRemoveIt()
    {
        var flashcard = FlashcardFixtures.GetAnyFlashcard();
        _flashcardRepositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId))
            .ReturnsAsync(flashcard);
        
        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().NotThrowAsync();
        _flashcardRepositoryMock.Verify(repo => repo.DeleteFlashcardAsync(flashcard), Times.Once);
    }

    [Fact]
    public void Handle_ForNonExistingFlashcardId_ShouldThrowNotFoundException()
    {
        _flashcardRepositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId))
            .ReturnsAsync((Flashcard)null);
        
        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Resource {nameof(Flashcard)} identified by {_command.FlashcardId} does not exist.");
        _flashcardRepositoryMock.Verify(repo => repo.GetFlashcardByIdAsync(_command.DeckId, _command.FlashcardId), Times.Once);
    }
}