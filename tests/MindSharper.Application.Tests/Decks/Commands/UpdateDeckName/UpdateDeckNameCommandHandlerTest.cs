using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Commands.UpdateDeckName;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
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

    public UpdateDeckNameCommandHandlerTest()
    {
        _handler = new UpdateDeckNameCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ForValidRequest_ShouldUpdateName()
    {
        var deck = DeckFixtures.GetAnyDeck();
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync(deck);

        await _handler.Handle(_command, CancellationToken.None);
        
        deck.Name.Should().Be(_command.Name);
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateDeckAsync(deck), Times.Once);
    }


    [Fact]
    public void Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_command.DeckId))
            .ReturnsAsync((Deck) null);

        Func<Task> action = async () => await _handler.Handle(_command, CancellationToken.None);
        
        action.Should().ThrowAsync<NotFoundException>();
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_command.DeckId), Times.Once);
        _repositoryMock.Verify(repo => repo.UpdateDeckAsync(It.IsAny<Deck>()), Times.Never);
    }
}