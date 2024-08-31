using System;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Tests.Fixtures;

public class DeckFixtures
{
    public static Deck GetAnyDeck()
    {
        return new()
        {
            Id = 1,
            Name = "C#",
            Flashcards = [],
            CreatedAt = new DateOnly(2024, 7, 7),
            Rate = 3
        };
    }

    public static DeckDto GetDeckDtoFromDeck(Deck deck)
    {
        return new()
        {
            Id = deck.Id,
            Name = deck.Name,
            Flashcards = [],
            CreatedAt = deck.CreatedAt,
            Rate = deck.Rate
        };
    }
    
    public static MinimalDeckDto GetMinimalDeckDtoFromDeck(Deck deck)
    {
        return new()
        {
            Id = deck.Id,
            Name = deck.Name,
            CreatedAt = deck.CreatedAt,
            Rate = deck.Rate
        };
    }
}