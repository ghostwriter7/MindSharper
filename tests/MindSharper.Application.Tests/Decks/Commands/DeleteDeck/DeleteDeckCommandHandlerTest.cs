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
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
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
    private readonly DeleteDeckCommand _command = new() { DeckId = 100 };

    public DeleteDeckCommandHandlerTest()
    {
        _handler = new DeleteDeckCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public void Handle_ForExistingDeck_ShouldRemoveIt()
    {
        var deck = DeckFixtures.GetAnyDeck();
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync(deck);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);
        
        action.Should().NotThrowAsync();
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _repositoryMock.Verify(repo => repo.DeleteDeckAsync(deck), Times.Once);
    }


    [Fact]
    public void Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync((Deck) null);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);
        
        action.Should().ThrowAsync<NotFoundException>();
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _repositoryMock.Verify(repo => repo.DeleteDeckAsync(It.IsAny<Deck>()), Times.Never);
    }
}