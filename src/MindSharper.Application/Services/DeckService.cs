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
    public async Task DeleteDeckAsync(int deckId)
    {
        logger.LogWarning($"Attempt to delete {nameof(Deck)} with ID: {deckId}");
        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        await repository.DeleteDeckAsync(deck);
        logger.LogWarning($"{nameof(Deck)} ({deckId}) has been successfully deleted");
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