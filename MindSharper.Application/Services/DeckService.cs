using Microsoft.Extensions.Logging;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Services;

public class DeckService(IDeckRepository repository, ILogger<DeckService> logger) : IDeckService
{
    public async Task<Deck?> GetDeckByIdAsync(int deckId)
    {
        logger.LogInformation($"Retrieving {nameof(Deck)} by ID: {deckId}");
        var deck = await repository.GetDeckByIdAsync(deckId)
            ?? throw new NotFoundException(nameof(Deck), deckId.ToString());
        return deck;
    }

    public async Task<IEnumerable<Deck>> GetDecksAsync()
    {
        logger.LogInformation($"Retrieving all {nameof(Deck)}");
        var decks = await repository.GetDecksAsync();
        return decks;
    }

    public async Task DeleteDeckAsync(int deckId)
    {
        logger.LogWarning($"Attempt to delete {nameof(Deck)} with ID: {deckId}");
        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        await repository.DeleteDeckAsync(deck);
        logger.LogWarning($"{nameof(Deck)} ({deckId}) has been successfully deleted");
    }

    public async Task<int> CreateDeckAsync(Deck deck)
    {
        logger.LogInformation($"Creating a {nameof(Deck)}");
        var deckId = await repository.CreateDeckAsync(deck);
        return deckId;
    }

    public async Task UpdateDeckNameAsync(int deckId, string name)
    {
        logger.LogInformation($"Updating {nameof(Deck)} ({deckId}) name");
        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        deck.Name = name;
        await repository.UpdateDeckAsync(deck);
    }
}