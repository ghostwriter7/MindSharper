using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Decks.Queries.GetDeckByIdQuery;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Queries.GetDeckById;

[TestSubject(typeof(GetDeckByIdQueryHandler))]
public class GetDeckByIdQueryHandlerTest
{
    private readonly Mock<ILogger<GetDeckByIdQueryHandler>> _loggerMock = new();
    private readonly Mock<IDeckRepository> _deckRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetDeckByIdQueryHandler _handler;
    private readonly int _deckId = 1;
    private readonly GetDeckByIdQuery _query;
    
    public GetDeckByIdQueryHandlerTest()
    {
        _query = new GetDeckByIdQuery(_deckId);
        _handler = new GetDeckByIdQueryHandler(_loggerMock.Object, _deckRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ForExistingDeckId_ShouldReturnDeckDto()
    {
        var deck = DeckFixtures.GetAnyDeck();
        var deckDto = DeckFixtures.GetDeckDtoFromDeck(deck);
        
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(_query.DeckId))
            .ReturnsAsync(deck);
        _mapperMock.Setup(mapper => mapper.Map<DeckDto>(deck))
            .Returns(deckDto);

        var result = await _handler.Handle(_query, CancellationToken.None);

        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(_query.DeckId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<DeckDto>(deck), Times.Once);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(deckDto);
    }
    
    [Fact]
    public async Task Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(_deckId))
            .ReturnsAsync((Deck) null);

        Func<Task> action = async () => await _handler.Handle(_query, CancellationToken.None);
        action.Should()
            .ThrowAsync<NotFoundException>();
        
        _deckRepositoryMock.Verify(repo => repo.GetDeckByIdAsync(_query.DeckId), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<DeckDto>(It.IsAny<Deck>()), Times.Never);
    }
}