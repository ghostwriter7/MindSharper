using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MindSharper.Domain.Entities;

namespace MindSharper.Infrastructure.Persistance;

internal class MindSharperDatabaseContext(DbContextOptions<MindSharperDatabaseContext> options) : DbContext(options)
{
    internal DbSet<Deck> Decks { get; set; }
    internal DbSet<Flashcard> Flashcards { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Deck>()
            .HasMany<Flashcard>(d => d.Flashcards)
            .WithOne()
            .HasForeignKey(f => f.DeckId);
    }
}