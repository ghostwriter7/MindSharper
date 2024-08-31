using Microsoft.EntityFrameworkCore;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Repositories;
using MindSharper.Infrastructure.Persistance;

namespace MindSharper.Infrastructure.Repositories;

internal class FlashcardRepository(MindSharperDatabaseContext context) : BaseRepository(context), IFlashcardRepository
{
    public async Task<Flashcard?> GetFlashcardByIdAsync(int deckId, int flashcardId)
    {
        var flashcard = await context.Flashcards.FirstOrDefaultAsync(flashcard => flashcard.DeckId == deckId && flashcard.Id == flashcardId);
        return flashcard;
    }

    public async Task<IEnumerable<Flashcard>> GetFlashcardsAsync(int deckId)
    {
        var flashcards = await context.Flashcards.Where(flashcard => flashcard.DeckId == deckId).ToListAsync();
        return flashcards;
    }

    public async Task DeleteFlashcardAsync(Flashcard flashcard)
    {
        context.Flashcards.Remove(flashcard);
        await context.SaveChangesAsync();
    }

    public async Task UpdateFlashcardAsync(Flashcard flashcard)
    {
        context.Flashcards.Update(flashcard);
        await context.SaveChangesAsync();
    }

    public async Task<int> CreateFlashcardAsync(Flashcard flashcard)
    {
        context.Flashcards.Add(flashcard);
        await context.SaveChangesAsync();
        return flashcard.Id;
    }
}