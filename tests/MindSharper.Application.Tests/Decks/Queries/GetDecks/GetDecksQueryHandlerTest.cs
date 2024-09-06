using System;
using System.Collections.Generic;
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
using MindSharper.Application.Users;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Interfaces;
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
        var userId = Guid.NewGuid().ToString();
        List<Deck> decks = [DeckFixtures.GetAnyDeck()];
        var minimalDeckDtos = decks.Select(DeckFixtures.GetMinimalDeckDtoFromDeck);
        var loggerMock = new Mock<ILogger<GetDecksQueryHandler>>();

        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser(userId, null, null));

        var request = new GetDecksQuery() { PageSize = 5, PageNumber = 1 };

        var repositoryMock = new Mock<IDeckRepository>();
        repositoryMock.Setup(repo => repo.GetDecksByUserIdAsync(userId, request.PageNumber, request.PageSize))
            .ReturnsAsync((decks, 1));

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<IEnumerable<MinimalDeckDto>>(decks))
            .Returns(minimalDeckDtos);


        var handler = new GetDecksQueryHandler(loggerMock.Object, repositoryMock.Object, mapperMock.Object,
            userContextMock.Object);

        var results = await handler.Handle(request, CancellationToken.None);

        results.Should().NotBeNull();
        results.Should().BeEquivalentTo(minimalDeckDtos);
        repositoryMock.Verify(repo => repo.GetDecksByUserIdAsync(userId, request.PageNumber, request.PageSize), Times.Once);
        mapperMock.Verify(mapper => mapper.Map<IEnumerable<MinimalDeckDto>>(decks), Times.Once);
    }
}