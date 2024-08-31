using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Commands.CreateDeck;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Commands.CreateDeck;

[TestSubject(typeof(CreateDeckCommandHandler))]
public class CreateDeckCommandHandlerTest
{
    private readonly Mock<ILogger<CreateDeckCommandHandler>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IDeckRepository> _repositoryMock = new();
    private readonly CreateDeckCommandHandler _handler;

    public CreateDeckCommandHandlerTest()
    {
        _handler = new CreateDeckCommandHandler(_loggerMock.Object, _mapperMock.Object, _repositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_ForValidRequest_ShouldReturnTaskId()
    {
        var command = new CreateDeckCommand() { Name = "C#" };
        var deck = new Deck() { Name = command.Name };
        var deckId = 100;
        _mapperMock.Setup(mapper => mapper.Map<Deck>(command))
            .Returns(deck);
        _repositoryMock.Setup(repo => repo.CreateDeckAsync(deck))
            .ReturnsAsync(deckId);

        var result = await _handler.Handle(command, CancellationToken.None);

        deck.CreatedAt.Should().Be(DateOnly.FromDateTime(DateTime.Now));
        result.Should().Be(deckId);
    }

    [Fact]
    public async Task Handle_WithDuplicatedName_ShouldThrowDuplicateResourceException()
    {
        var command = new CreateDeckCommand() { Name = "C#" };
        var deck = new Deck() { Name = command.Name };
        var deckId = 100;
        var dbExceptionMock = new Mock<DbException>();
        dbExceptionMock.Setup(exception => exception.Message).Returns("duplicate key");
        _mapperMock.Setup(mapper => mapper.Map<Deck>(command))
            .Returns(deck);
        _repositoryMock.Setup(repo => repo.CreateDeckAsync(deck))
            .Throws(() => new Exception(null, dbExceptionMock.Object));

        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);
        action.Should().ThrowAsync<DuplicateResourceException>()
            .WithMessage($"Deck with Name: C# already exists");
    }
}