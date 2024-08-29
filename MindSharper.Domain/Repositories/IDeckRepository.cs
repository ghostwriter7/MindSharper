using MindSharper.Domain.Entities;

namespace MindSharper.Domain.Repositories;

public interface IDeckRepository
{
    Task<Deck?> GetDeckByIdAsync(int deckId);
    Task<IEnumerable<Deck>> GetDecks();
    Task<int> CreateDeck(Deck deck);
    Task DeleteDeck(Deck deck);
    Task UpdateDeck(Deck deck);
}