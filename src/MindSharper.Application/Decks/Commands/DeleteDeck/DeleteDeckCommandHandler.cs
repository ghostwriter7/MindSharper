using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Commands.DeleteDeck;

public class DeleteDeckCommandHandler(
    ILogger<DeleteDeckCommandHandler> logger,
    IDeckRepository repository,
    IUserContext userContext,
    IResourceAuthorizationService<Deck> deckAuthorizationService) : IRequestHandler<DeleteDeckCommand>
{
    public async Task Handle(DeleteDeckCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()!;
        var deckId = request.DeckId;
        logger.LogWarning("Attempt to delete {resource} with ID: {deckId} by User {userId}", nameof(Deck),
            request.DeckId, currentUser.Id);
        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        if (!deckAuthorizationService.IsAuthorized(deck, ResourceOperation.Delete))
            throw new UnauthorizedException(nameof(Deck), deck.Id, currentUser.Id);

        await repository.DeleteDeckAsync(deck);
        logger.LogWarning($"{nameof(Deck)} ({deckId}) has been successfully deleted");
    }
}