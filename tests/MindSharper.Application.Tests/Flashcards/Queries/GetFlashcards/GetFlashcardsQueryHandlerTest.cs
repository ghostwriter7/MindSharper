using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Application.Flashcards.Queries.GetFlashcards;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Flashcards.Queries.GetFlashcards;

[TestSubject(typeof(GetFlashcardsQueryHandler))]
public class GetFlashcardsQueryHandlerTest
{
    private readonly Mock<ILogger<GetFlashcardsQueryHandler>> _loggerMock = new();
    private readonly Mock<IDeckRepository> _repositoryMock = new();
    private readonly IMapper _mapper;
    private readonly GetFlashcardsQueryHandler _handler;
    private readonly GetFlashcardsQuery _query = new(1);

    public GetFlashcardsQueryHandlerTest()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FlashcardProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        _handler = new(_loggerMock.Object, _mapper, _repositoryMock.Object);
    }
    
    [Fact]
    public async Task Handle_ForExistingDeck_ShouldReturnItsFlashcards()
    {
        var deck = DeckFixtures.GetAnyDeck();
        deck.Flashcards = FlashcardFixtures.GetFlashcards();
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_query.DeckId))
            .ReturnsAsync(deck);

        var results = await _handler.Handle(_query, CancellationToken.None);

        results.Should().NotBeNull();
        results.Should().BeEquivalentTo(_mapper.Map<IEnumerable<FlashcardDto>>(deck.Flashcards));
    }

    [Fact]
    public async Task Handle_ForNonExistingDeckId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(repo => repo.GetDeckByIdAsync(_query.DeckId))
            .ReturnsAsync((Deck)null);

        Func<Task> action = async () => await _handler.Handle(_query, CancellationToken.None);

        action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Resource Deck identified by {_query.DeckId} does not exist.");
        _repositoryMock.Verify(repo => repo.GetDeckByIdAsync(_query.DeckId), Times.Once);
    }
}