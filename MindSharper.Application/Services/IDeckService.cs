using MindSharper.Application.Decks.Dtos;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Services;

public interface IDeckService
{
    Task<DeckDto?> GetDeckByIdAsync(int deckId);
    Task<IEnumerable<DeckDto>> GetDecksAsync();
    Task DeleteDeckAsync(int deckId);
    Task<int> CreateDeckAsync(CreateDeckDto deck);
    Task UpdateDeckNameAsync(int deckId, string name);
}