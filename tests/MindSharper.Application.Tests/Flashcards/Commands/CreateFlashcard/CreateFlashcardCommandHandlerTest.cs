using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Commands.CreateFlashcard;
using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Flashcards.Commands.CreateFlashcard;

[TestSubject(typeof(CreateFlashcardCommandHandler))]
public class CreateFlashcardCommandHandlerTest
{
    private readonly Mock<ILogger<CreateFlashcardCommandHandler>> _loggerMock = new();
    private readonly IMapper _mapper;
    private readonly Mock<IFlashcardRepository> _flashcardRepositoryMock = new();
    private readonly Mock<IDeckRepository> _deckRepositoryMock = new();
    private readonly CreateFlashcardCommandHandler _handler;
    private readonly CreateFlashcardCommand _command = new CreateFlashcardCommand()
    {
        DeckId = 1,
        Frontside = "Any question",
        Backside = "Any answer"
    };

    public CreateFlashcardCommandHandlerTest()
    {
        var mapperConfig = new MapperConfiguration(config => config.AddProfile<FlashcardProfile>());
        _mapper = mapperConfig.CreateMapper();
        _handler = new CreateFlashcardCommandHandler(_loggerMock.Object, _mapper, _deckRepositoryMock.Object,
            _flashcardRepositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_ForValidRequest_ShouldReturnFlashcardId()
    {
        const int flashcardId = 100;
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync(new Deck());
        _flashcardRepositoryMock.Setup(repo => repo.CreateFlashcardAsync(It.IsAny<Flashcard>()))
            .ReturnsAsync(flashcardId);

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.Should().Be(flashcardId);
        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _flashcardRepositoryMock.Verify(repo => repo.CreateFlashcardAsync(It.IsAny<Flashcard>()), Times.Once);
    }

    [Fact]
    public void Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        const int flashcardId = 100;
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync((Deck)null);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Resource {nameof(Deck)} identified by {_command.DeckId} does not exist.");

        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _flashcardRepositoryMock.Verify(repo => repo.CreateFlashcardAsync(It.IsAny<Flashcard>()), Times.Never);
    }

    [Fact]
    public void Handle_ForDuplicatedFlashcardFrontsideInDeck_ShouldThrowDuplicatedResourceException()
    {
        const int flashcardId = 100;
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync(new Deck());
        var dbExceptionMock = new Mock<DbException>();
        dbExceptionMock.Setup(exception => exception.Message).Returns("duplicate key");
        _flashcardRepositoryMock.Setup(repo => repo.CreateFlashcardAsync(It.IsAny<Flashcard>()))
            .Throws(() => new Exception(null, dbExceptionMock.Object));

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);

        action.Should().ThrowAsync<DuplicateResourceException>()
            .WithMessage($"{nameof(Flashcard)} with {nameof(Flashcard.Frontside)}: {_command.Frontside} already exists");

        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _flashcardRepositoryMock.Verify(repo => repo.CreateFlashcardAsync(It.IsAny<Flashcard>()), Times.Once);
    }
}