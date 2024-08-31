﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Decks.Queries.GetDecks;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Queries.GetDecks;

[TestSubject(typeof(GetDecksQueryHandler))]
public class GetDecksQueryHandlerTest
{
    [Fact]
    public async Task Handle_ForValidRequest_ShouldReturnListOfMinimalDeckDtos()
    {
        List<Deck> decks = [DeckFixtures.GetAnyDeck()];
        var minimalDeckDtos = decks.Select(DeckFixtures.GetMinimalDeckDtoFromDeck);
        var loggerMock = new Mock<ILogger<GetDecksQueryHandler>>();

        var repositoryMock = new Mock<IDeckRepository>();
        repositoryMock.Setup(repo => repo.GetDecksAsync())
            .ReturnsAsync(decks);
        
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<IEnumerable<MinimalDeckDto>>(decks))
            .Returns(minimalDeckDtos);
        
        var request = new GetDecksQuery();

        var handler = new GetDecksQueryHandler(loggerMock.Object, repositoryMock.Object, mapperMock.Object);

        var results = await handler.Handle(request, CancellationToken.None);

        results.Should().NotBeNull();
        results.Should().BeEquivalentTo(minimalDeckDtos);
        repositoryMock.Verify(repo => repo.GetDecksAsync(), Times.Once);
        mapperMock.Verify(mapper => mapper.Map<IEnumerable<MinimalDeckDto>>(decks), Times.Once);
    }
}