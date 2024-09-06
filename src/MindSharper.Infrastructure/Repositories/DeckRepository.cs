using Microsoft.EntityFrameworkCore;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Repositories;
using MindSharper.Infrastructure.Persistence;

namespace MindSharper.Infrastructure.Repositories;

internal class DeckRepository(MindSharperDatabaseContext context) : BaseRepository(context), IDeckRepository
{
    public async Task<Deck?> GetDeckByIdAsync(int deckId)
    {
        var deck = await context.Decks
            .Include(deck => deck.Flashcards)
            .Where(deck => deck.Id == deckId)
            .FirstOrDefaultAsync();
        return deck;
    }

    public async Task<(IEnumerable<Deck>, int)> GetDecksByUserIdAsync(string userId, int pageNumber, int pageSize)
    {
        var baseQuery = context.Decks.Where(deck => deck.UserId == userId);

        var total = await baseQuery.CountAsync();

        if (total == 0)
            return ([], 0);
        
        var decks = await baseQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (decks, total);
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