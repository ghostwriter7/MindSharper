using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Helpers;
using MindSharper.Application.Users;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Commands.CreateDeck;

public class CreateDeckCommandHandler(
    ILogger<CreateDeckCommandHandler> logger,
    IMapper mapper,
    IDeckRepository repository,
    IUserContext userContext) : IRequestHandler<CreateDeckCommand, int>
{
    public async Task<int> Handle(CreateDeckCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()!;
        logger.LogInformation("Creating a Deck by user ({UserId}) with request {@Request}", currentUser.Id, request);
        var deck = mapper.Map<Deck>(request);
        deck.UserId = currentUser.Id;
        deck.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

        try
        {
            var deckId = await repository.CreateDeckAsync(deck);
            return deckId;
        }
        catch (Exception exception) when (ExceptionHelper.IsUniqueConstraintViolationException(exception))
        {
            throw new DuplicateResourceException(nameof(Deck), nameof(Deck.Name), deck.Name);
        }
    }
}