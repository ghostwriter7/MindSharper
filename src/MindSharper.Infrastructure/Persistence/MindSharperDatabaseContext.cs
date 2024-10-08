﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MindSharper.Domain.Entities;

namespace MindSharper.Infrastructure.Persistence;

internal class MindSharperDatabaseContext(DbContextOptions<MindSharperDatabaseContext> options) : IdentityDbContext<User>(options)
{
    internal DbSet<Deck> Decks { get; set; }
    internal DbSet<Flashcard> Flashcards { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany<Deck>(user => user.OwnedDecks)
            .WithOne(deck => deck.Owner)
            .HasForeignKey(deck => deck.UserId);
        
        modelBuilder.Entity<Deck>()
            .HasMany<Flashcard>(d => d.Flashcards)
            .WithOne()
            .HasForeignKey(f => f.DeckId);

        modelBuilder.Entity<Deck>()
            .HasIndex(deck => deck.Name)
            .IsUnique();

        modelBuilder.Entity<Flashcard>()
            .HasIndex(flashcard => new { flashcard.DeckId, flashcard.Frontside })
            .IsUnique();
    }
}