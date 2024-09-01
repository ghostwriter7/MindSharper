using Microsoft.EntityFrameworkCore;
using MindSharper.Domain.Entities;
using MindSharper.Infrastructure.Persistence;

namespace MindSharper.Infrastructure.Seeders;

internal class DatabaseSeeder(MindSharperDatabaseContext context) : IDatabaseSeeder
{
    public async Task Seed()
    {
        if (await context.Database.CanConnectAsync())
        {
            if (!context.Decks.Any())
            {
                context.Decks.AddRange(GetSampleDecks());
                await context.SaveChangesAsync();
            }
        }
    }

    private Deck[] GetSampleDecks()
    {
        return
        [
            new ()
            {
                Name = "C#",
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Flashcards = [
                    new ()
                    {
                        Frontside = "What is LINQ?",
                        Backside = "Language INtegrated Query. A common solution for querying in-memory objects, databases, xml, any data.",
                        CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                    }
                ]
            },
            new ()
            {
                Name = "Angular",
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Flashcards = [
                    new ()
                    {
                        Frontside = "What does every Component extend?",
                        Backside = "A Directive",
                        CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                    },
                    new ()
                    {
                        Frontside = "What are two available change detection modes?",
                        Backside = "OnPush (recommended) and Default",
                        CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                    }
                ]
            }
        ];
    }
}