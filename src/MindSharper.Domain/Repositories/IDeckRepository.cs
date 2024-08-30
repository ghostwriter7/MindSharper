using MindSharper.Domain.Entities;

namespace MindSharper.Domain.Repositories;

public interface IDeckRepository : IBaseRepository
{
    Task<Deck?> GetDeckByIdAsync(int deckId);
    Task<IEnumerable<Deck>> GetDecksAsync();
    Task<int> CreateDeckAsync(Deck deck);
    Task DeleteDeckAsync(Deck deck);
    Task UpdateDeckAsync(Deck deck);
}