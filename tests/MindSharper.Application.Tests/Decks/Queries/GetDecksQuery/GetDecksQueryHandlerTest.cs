using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Decks.Queries.GetDecksQuery;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Queries.GetDecks;

[TestSubject(typeof(GetDecksQueryHandler))]
public class GetDecksQueryHandlerTest
{
    private readonly Mock<ILogger<GetDecksQueryHandler>> _loggerMock = new();
    private readonly Mock<IDeckRepository> _deckRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetDecksQueryHandler _handler;
    
    public GetDecksQueryHandlerTest()
    {
        _handler = new GetDecksQueryHandler(_loggerMock.Object, _deckRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ForExistingDeckId_ShouldReturnDeckDto()
    {
        var deck = DeckFixtures.GetAnyDeck();
        var deckDto = DeckFixtures.GetDeckDtoFromDeck(deck);
        var query = new GetDecksQuery() { DeckId = deck.Id };
        
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(query.DeckId))
            .ReturnsAsync(deck);
        _mapperMock.Setup(mapper => mapper.Map<DeckDto>(deck))
            .Returns(deckDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(query.DeckId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<DeckDto>(deck), Times.Once);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(deckDto);
    }
    
    [Fact]
    public async Task Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        var deckId = 1;
        var query = new GetDecksQuery() { DeckId = deckId };
        
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(deckId))
            .ReturnsAsync((Deck) null);

        Func<Task> action = async () => await _handler.Handle(query, CancellationToken.None);
        action.Should()
            .ThrowAsync<NotFoundException>();
        
        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(query.DeckId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<DeckDto>(It.IsAny<Deck>()), Times.Never);
    }
}