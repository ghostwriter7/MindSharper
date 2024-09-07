using System;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MindSharper.Presentation.API.Controllers;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Users;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Repositories;
using MindSharper.Infrastructure.Authorization;
using MindSharper.Tests.Common.Helpers;
using Moq;
using Xunit;

namespace MindSharper.Presentation.API.Tests.Controllers;

[TestSubject(typeof(DeckController))]
public class DeckControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly Mock<IDeckRepository> _deckRepositoryMock = new();
    private readonly HttpClient _client;
    private readonly CurrentUser _currentUser = new CurrentUser(Guid.NewGuid().ToString(), null, null);

    public DeckControllerTest(WebApplicationFactory<Program> webApplicationFactory)
    {
        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(userContext => userContext.GetCurrentUser())
            .Returns(_currentUser);

        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.Replace(ServiceDescriptor.Scoped(typeof(IUserContext), _ => userContextMock.Object));
                services.Replace(ServiceDescriptor.Scoped(typeof(IDeckRepository),
                    _ => _deckRepositoryMock.Object));
            });
        });
        _client = _webApplicationFactory.CreateClient();
    }


    [Fact]
    public async Task GetDeckById_ForValidRequest_ShouldReturn200Ok()
    {
        var deck = new Deck()
        {
            Id = 1,
            Name = "C#",
            CreatedAt = new DateOnly(2024, 7, 7),
            Flashcards = [],
            Rate = 3,
            UserId = _currentUser.Id
        };
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, deck.Id, deck);

        var result = await _client.GetAsync($"api/decks/{deck.Id}");
        var content = await result.Content.ReadFromJsonAsync<DeckDto>();

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().BeEquivalentTo(new DeckDto()
        {
            Name = deck.Name,
            CreatedAt = deck.CreatedAt,
            Flashcards = [],
            Rate = deck.Rate,
            Id = deck.Id
        });
    }

    [Fact]
    public async Task GetDeckById_ForNonExistingResource_ShouldReturn404NotFound()
    {
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, 1, null);

        var result = await _client.GetAsync("api/decks/1");

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDecks_ForValidRequest_ShouldReturn200Ok()
    {
        _deckRepositoryMock.Setup(repo => repo.GetDecksByUserIdAsync(It.IsAny<string>(), 1, 5)).ReturnsAsync(([], 0));

        var result = await _client.GetAsync("api/decks?pageSize=5&pageNumber=1");

        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateDeck_ForValidRequest_ShouldReturn201Created()
    {
        var deckId = 100;
        _deckRepositoryMock.Setup(repo => repo.CreateDeckAsync(It.IsAny<Deck>()))
            .ReturnsAsync(deckId);

        var result = await _client.PostAsync("api/decks", JsonContent.Create(new CreateDeckDto() { Name = "C#" }));

        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Headers.Location.PathAndQuery.Should().Be($"/api/decks/{deckId}");
    }

    [Fact]
    public async Task CreateDeck_ForNullName_ShouldReturn400BadRequest()
    {
        var result = await _client.PostAsync("api/decks", JsonContent.Create(new CreateDeckDto()));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDeck_ForExistingResourceWithTheSameName_ShouldReturn400BadRequest()
    {
        var dbExceptionMock = new Mock<DbException>();
        dbExceptionMock.Setup(ex => ex.Message).Returns("duplicate key");

        _deckRepositoryMock.Setup(repo => repo.CreateDeckAsync(It.IsAny<Deck>()))
            .Throws(() => new Exception(null, dbExceptionMock.Object));

        var result = await _client.PostAsync("api/decks", JsonContent.Create(new CreateDeckDto() { Name = "C#" }));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteDeck_ForExistingResource_ShouldReturn204NoContet()
    {
        var deckId = 100;
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, deckId,
            new Deck() { Id = deckId, UserId = _currentUser.Id });

        var result = await _client.DeleteAsync($"api/decks/{deckId}");

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteDeck_ForNonExistingResource_ShouldReturn404NotFound()
    {
        var deckId = 100;
        SetupHelper.SetUpGetDeckByIdAsync(_deckRepositoryMock, deckId, null);

        var result = await _client.DeleteAsync($"api/decks/{deckId}");

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}