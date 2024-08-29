using Microsoft.EntityFrameworkCore;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Repositories;
using MindSharper.Infrastructure.Persistance;

namespace MindSharper.Infrastructure.Repositories;

internal class DeckRepository(MindSharperDatabaseContext context) : IDeckRepository
{
    public async Task<Deck?> GetDeckByIdAsync(int deckId)
    {
        var deck = await context.Decks.FindAsync(deckId);
        return deck;
    }

    public async Task<IEnumerable<Deck>> GetDecksAsync()
    {
        var decks = await context.Decks.ToListAsync();
        return decks;
    }

    public async Task<int> CreateDeckAsync(Deck deck)
    {
        context.Decks.Add(deck);
        await context.SaveChangesAsync();
        return deck.Id;
    }

    public async Task DeleteDeckAsync(Deck deck)
    {
        context.Decks.Remove(deck);
        await context.SaveChangesAsync();
    }

    public async Task UpdateDeckAsync(Deck deck)
    {
        context.Decks.Update(deck);
        await context.SaveChangesAsync();
    }
}