using System;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MindSharper.API.Controllers;
using MindSharper.Application.Flashcards.Commands.CreateFlashcard;
using MindSharper.Application.Flashcards.Commands.UpdateFlashcard;
using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;
using MindSharper.Infrastructure.Authorization;
using MindSharper.Tests.Common.Helpers;
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
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly Mock<IResourceAuthorizationService<Deck>> _authorizationServiceMock = new();
    private const int _deckId = 1;
    private const int _flashcardId = 100;

    public FlashcardControllerTest(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.Replace(ServiceDescriptor.Scoped<IUserContext>((_) => _userContextMock.Object));
                services.Replace(
                    ServiceDescriptor.Scoped<IResourceAuthorizationService<Deck>>((_) =>
                        _authorizationServiceMock.Object));
                services.Replace(ServiceDescriptor.Scoped<IDeckRepository>((_) => _deckRepositoryMock.Object));
                services.Replace(
                    ServiceDescriptor.Scoped<IFlashcardRepository>((_) => _flashcardRepositoryMock.Object));
            });
        });
        _userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(new CurrentUser(Guid.NewGuid().ToString(), null, null));
        _authorizationServiceMock
            .Setup(authService => authService.IsAuthorized(It.IsAny<Deck>(), It.IsAny<ResourceOperation>()))
            .Returns(true);
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
    public async Task CreateFlashcard_ForNonExistingDeckId_ShouldReturn404NotFound()
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
    public async Task CreateFlashcard_ForRequestWithADuplicatedFlashcard_ShouldReturn400BadRequest()
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

    [Fact]
    public async Task UpdateFlashcard_ForValidRequest_ShouldReturn204NoContent()
    {
        var currentUser = new CurrentUser(Guid.NewGuid().ToString(), null, null);
        SetupHelper.SetUpUserContext(_userContextMock, currentUser);
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, _deckId,
            new Deck() { UserId = currentUser.Id, Id = _deckId });
        SetupHelper.SetUpGetFlashcardByIdAsync(_flashcardRepositoryMock, _deckId, _flashcardId,
            new Flashcard() { Frontside = "Some question", Backside = "Some answer" });

        var result = await _client.PatchAsync($"api/decks/{_deckId}/flashcards", JsonContent.Create(new UpdateFlashcardCommand()
        {
            Frontside = "New question",
            Backside = "New answer",
            FlashcardId = _flashcardId
        }));

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateFlashcard_ForNonExistingDeckId_ShouldReturn404NotFound()
    {
        var currentUser = new CurrentUser(Guid.NewGuid().ToString(), null, null);
        SetupHelper.SetUpUserContext(_userContextMock, currentUser);
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, _deckId, null);

        var result = await _client.PatchAsync($"api/decks/{_deckId}/flashcards", JsonContent.Create(new UpdateFlashcardCommand()
        {
            Frontside = "New question",
            Backside = "New answer",
            FlashcardId = _flashcardId
        }));

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateFlashcard_ForNonExistingFlashcardId_ShouldReturn404NotFound()
    {
        var currentUser = new CurrentUser(Guid.NewGuid().ToString(), null, null);
        SetupHelper.SetUpUserContext(_userContextMock, currentUser);
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, _deckId,
            new Deck() { UserId = currentUser.Id, Id = _deckId });
        SetupHelper.SetUpGetFlashcardByIdAsync(_flashcardRepositoryMock, _deckId, _flashcardId, null);

        var result = await _client.PatchAsync($"api/decks/{_deckId}/flashcards", JsonContent.Create(new UpdateFlashcardCommand()
        {
            Frontside = "New question",
            Backside = "New answer",
            FlashcardId = _flashcardId
        }));

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateFlashcard_ForFlashcardBelongingToAnotherUser_ShouldReturn403Forbidden()
    {
        var currentUser = new CurrentUser(Guid.NewGuid().ToString(), null, null);
        SetupHelper.SetUpUserContext(_userContextMock, currentUser);
        var deck = new Deck() { UserId = Guid.NewGuid().ToString(), Id = _deckId };
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, _deckId, deck);
        SetupHelper.SetUpAuthorizationService(_authorizationServiceMock, deck, ResourceOperation.Update, false);

        var result = await _client.PatchAsync($"api/decks/{_deckId}/flashcards", JsonContent.Create(new UpdateFlashcardCommand()
        {
            Frontside = "New question",
            Backside = "New answer",
            FlashcardId = _flashcardId
        }));

        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetFlashcardById_ForValidRequest_ShouldReturn200Ok()
    {
        var flashcard = new Flashcard()
        {
            DeckId = _deckId, Frontside = "Any question", Backside = "Any answer", Id = _flashcardId,
            CreatedAt = new DateOnly(2024, 7, 7)
        };
        _flashcardRepositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_deckId, _flashcardId))
            .ReturnsAsync(flashcard);

        var results = await _client.GetAsync($"api/decks/{_deckId}/flashcards/{_flashcardId}");

        results.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await results.Content.ReadFromJsonAsync<FlashcardDto>();
        content.Should().BeEquivalentTo(new FlashcardDto()
        {
            Backside = flashcard.Backside, Frontside = flashcard.Frontside, Id = flashcard.Id,
            CreatedAt = flashcard.CreatedAt
        });
    }

    [Fact]
    public async Task GetFlashcardById_ForInvalidRequest_ShouldReturn404NotFound()
    {
        _flashcardRepositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_deckId, _flashcardId))
            .Throws(() => new NotFoundException(nameof(Flashcard), _flashcardId.ToString()));

        var results = await _client.GetAsync($"api/decks/{_deckId}/flashcards/{_flashcardId}");

        results.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}