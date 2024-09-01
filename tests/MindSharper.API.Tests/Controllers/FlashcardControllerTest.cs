using System;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MindSharper.API.Controllers;
using MindSharper.Application.Flashcards.Commands.CreateFlashcard;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.API.Tests.Controllers;

[TestSubject(typeof(FlashcardController))]
public class FlashcardControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly Mock<IDeckRepository> _deckRepositoryMock = new();
    private readonly Mock<IFlashcardRepository> _flashcardRepositoryMock = new();
    private readonly HttpClient _client;
    private readonly int _deckId = 1;

    public FlashcardControllerTest(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.Replace(ServiceDescriptor.Scoped<IDeckRepository>((_) => _deckRepositoryMock.Object));
                services.Replace(
                    ServiceDescriptor.Scoped<IFlashcardRepository>((_) => _flashcardRepositoryMock.Object));
            });
        });
        _client = _webApplicationFactory.CreateClient();
    }

    [Fact]
    public async Task CreateFlashcard_ForValidRequest_ShouldReturn201CreatedAt()
    {
        var command = new CreateFlashcardCommand()
        {
            Frontside = "Any question",
            Backside = "Any answer"
        };
        var flashcardId = 100;
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(_deckId))
            .ReturnsAsync(new Deck() { Id = _deckId });
        _flashcardRepositoryMock.Setup(repo => repo.CreateFlashcardAsync(It.IsAny<Flashcard>()))
            .ReturnsAsync(flashcardId);
        
        var results = await _client.PostAsync($"api/decks/{_deckId}/flashcards", JsonContent.Create(command));

        results.StatusCode.Should().Be(HttpStatusCode.Created);
        results.Headers.Location.PathAndQuery.Should().Be("/api/decks/1/flashcards/100");
    }

    [Fact]
    public async Task CreateFlashcard_ForNonExistingFlashcardId_ShouldReturn404NotFound()
    {
        var command = new CreateFlashcardCommand()
        {
            Frontside = "Any question",
            Backside = "Any answer"
        };
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(_deckId))
            .Throws(() => new NotFoundException(nameof(Deck), _deckId.ToString()));
        
        var results = await _client.PostAsync($"api/decks/{_deckId}/flashcards", JsonContent.Create(command));

        results.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateFlashcard_ForInvalidRequest_ShouldReturn400BadRequest()
    {
        var command = new CreateFlashcardCommand()
        {
            Frontside = "Any question",
            Backside = "Any answer"
        };
        var dbExceptionMock = new Mock<DbException>();
        dbExceptionMock.Setup(ex => ex.Message).Returns("duplicate key");
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(_deckId))
            .ReturnsAsync(new Deck() { Id = _deckId });
        _flashcardRepositoryMock.Setup(repo => repo.CreateFlashcardAsync(It.IsAny<Flashcard>()))
            .Throws(() => new Exception(null, dbExceptionMock.Object));
        
        var results = await _client.PostAsync($"api/decks/{_deckId}/flashcards", JsonContent.Create(command));
        
        results.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}