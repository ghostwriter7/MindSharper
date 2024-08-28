using Microsoft.EntityFrameworkCore;
using MindSharper.Domain.Entities;

namespace MindSharper.Infrastructure.Persistance;

internal class MindSharperDatabaseContext : DbContext
{
    internal DbSet<Deck> Decks { get; set; }
    internal DbSet<Flashcard> Flashcards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost;Database=MindSharper;Trusted_Connection=True;Trust Server Certificate=true;");
    }
}