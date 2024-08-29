using MindSharper.Domain.Entities;

namespace MindSharper.Application.Services;

public interface IDeckService
{
    Task<Deck?> GetDeckByIdAsync(int deckId);
    Task<IEnumerable<Deck>> GetDecksAsync();
    Task DeleteDeckAsync(int deckId);
    Task<int> CreateDeckAsync(Deck deck);
    Task UpdateDeckNameAsync(int deckId, string name);
}