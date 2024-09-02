using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Decks.Commands.UpdateDeckName;

public class UpdateDeckNameCommandHandler(
    ILogger<UpdateDeckNameCommandHandler> logger,
    IDeckRepository repository,
    IUserContext userContext,
    IResourceAuthorizationService<Deck> authorizationService) : IRequestHandler<UpdateDeckNameCommand>
{
    public async Task Handle(UpdateDeckNameCommand request, CancellationToken cancellationToken)
    {
        var (id, _, _) = userContext.GetCurrentUser()!;
        var (deckId, name) = request;
        logger.LogInformation("Attempt to update Deck's ({DeckId}) name by User {UserId}", deckId, id);

        var deck = await repository.GetDeckByIdAsync(deckId)
                   ?? throw new NotFoundException(nameof(Deck), deckId.ToString());

        if (!authorizationService.IsAuthorized(deck, ResourceOperation.Update))
            throw new UnauthorizedException(nameof(Deck), deck.Id, id);

        deck.Name = name;
        await repository.UpdateDeckAsync(deck);
    }
}