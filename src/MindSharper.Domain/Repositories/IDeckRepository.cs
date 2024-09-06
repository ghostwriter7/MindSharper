using MindSharper.Domain.Entities;

namespace MindSharper.Domain.Repositories;

public interface IDeckRepository : IBaseRepository
{
    Task<Deck?> GetDeckByIdAsync(int deckId);
    Task<(IEnumerable<Deck>, int)> GetDecksByUserIdAsync(string userId, int pageNumber, int pageSize);
    Task<int> CreateDeckAsync(Deck deck);
    Task DeleteDeckAsync(Deck deck);
    Task UpdateDeckAsync(Deck deck);
}