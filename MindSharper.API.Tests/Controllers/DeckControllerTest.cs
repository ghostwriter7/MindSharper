using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MindSharper.API.Controllers;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.API.Tests.Controllers;

[TestSubject(typeof(DeckController))]
public class DeckControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    private readonly Mock<IDeckRepository> _deckRepositoryMock = new();

    public DeckControllerTest(WebApplicationFactory<Program> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.Replace(ServiceDescriptor.Scoped(typeof(IDeckRepository),
                    _ => _deckRepositoryMock.Object));
            });
        });
    }


    [Fact]
    public async Task GetDeckById_ForValidRequest_ShouldReturn200Ok()
    {
        var client = _webApplicationFactory.CreateClient();
        var deck = new Deck()
        {
            Name = "C#",
            CreatedAt = new DateOnly(2024, 7, 7),
            Flashcards = [],
            Rate = 3
        };
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(1)).ReturnsAsync(deck);

        var result = await client.GetAsync("api/decks/1");
        var content = await result.Content.ReadFromJsonAsync<DeckDto>();
        
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Flashcards.Should().BeEmpty();
        content.Name.Should().Be(deck.Name);
        content.CreatedAt.Should().Be(deck.CreatedAt);
        content.Rate.Should().Be(deck.Rate);
    }

    [Fact]
    public async Task GetDeckById_ForNonExistingResource_ShouldReturn404NotFound()
    {
        var client = _webApplicationFactory.CreateClient();
        _deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(1)).ReturnsAsync((Deck)null);

        var result = await client.GetAsync("api/decks/1");

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}