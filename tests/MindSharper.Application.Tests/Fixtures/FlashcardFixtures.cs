using System;
using System.Collections.Generic;
using System.Linq;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Tests.Fixtures;

public static class FlashcardFixtures
{
    public static Flashcard GetAnyFlashcard()
    {
        return new()
        {
            Frontside = "Question",
            Backside = "Answer",
            DeckId = 1,
            Id = 1,
            CreatedAt = new DateOnly(2024, 7, 7)
        };
    }

    public static List<Flashcard> GetFlashcards()
    {
        return Enumerable.Range(1, 5).Select(index =>
        {
            var flashcard = GetAnyFlashcard();
            flashcard.Frontside += index;
            flashcard.Backside += index;
            flashcard.Id = index;
            return flashcard;
        }).ToList();
    }
}