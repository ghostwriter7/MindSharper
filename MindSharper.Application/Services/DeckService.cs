using System.Data.Common;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Services;

public class DeckService(IDeckRepository repository, ILogger<DeckService> logger, IMapper mapper) : IDeckService
{
    public async Task<DeckDto?> GetDeckByIdAsync(int deckId)
    {
        logger.LogInformation($"Retrieving {nameof(Deck)} by ID: {deckId}");
        var deck = await repository.GetDeckByIdAsync(deckId)
            ?? throw new NotFoundException(nameof(Deck), deckId.ToString());
        var deckDto = mapper.Map<DeckDto>(deck);
        return deckDto;
    }

    public async Task<IEnumerable<MinimalDeckDto>> GetDecksAsync()
    {
        logger.LogInformation($"Retrieving all {nameof(Deck)}");
        var decks = await repository.GetDecksAsync();
        var deckDtos = mapper.Map<IEnumerable<MinimalDeckDto>>(decks);
        return deckDtos;
    }

    public async Task DeleteDeckAsync(int deckId)
    {
        logger.LogWarning($"Attempt to delete {nameof(Deck)} with ID: {deckId}");
        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        await repository.DeleteDeckAsync(deck);
        logger.LogWarning($"{nameof(Deck)} ({deckId}) has been successfully deleted");
    }

    public async Task<int> CreateDeckAsync(CreateDeckDto createDeckDto)
    {
        logger.LogInformation($"Creating a {nameof(Deck)}");
        var deck = mapper.Map<Deck>(createDeckDto);
        deck.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

        try
        {
            var deckId = await repository.CreateDeckAsync(deck);
            return deckId;
        }
        catch (Exception exception) when (IsUniqueConstraintViolationException(exception))
        {
            throw new DuplicateResourceException(nameof(Deck), nameof(Deck.Name), deck.Name);
        }
    }

    public async Task UpdateDeckNameAsync(int deckId, string name)
    {
        logger.LogInformation($"Updating {nameof(Deck)} ({deckId}) name");
        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        deck.Name = name;
        await repository.UpdateDeckAsync(deck);
    }

    private bool IsUniqueConstraintViolationException(Exception exception)
    {
        return exception.InnerException is DbException innerException &&
               innerException.Message.Contains("duplicate key");
    }
}