using MindSharper.Application.Decks.Dtos;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Services;

public interface IDeckService
{
    Task DeleteDeckAsync(int deckId);
    Task UpdateDeckNameAsync(int deckId, string name);
}