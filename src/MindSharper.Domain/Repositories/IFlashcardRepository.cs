using MindSharper.Domain.Entities;

namespace MindSharper.Domain.Repositories;

public interface IFlashcardRepository : IBaseRepository
{
    Task<Flashcard?> GetFlashcardByIdAsync(int deckId, int flashcardId);
    Task<IEnumerable<Flashcard>> GetFlashcardsAsync(int deckId);
    Task DeleteFlashcardAsync(Flashcard flashcard);
    Task UpdateFlashcardAsync(Flashcard flashcard);
    Task<int> CreateFlashcardAsync(Flashcard flashcard);
}