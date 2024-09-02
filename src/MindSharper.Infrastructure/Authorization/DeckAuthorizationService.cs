using Microsoft.Extensions.Logging;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Interfaces;

namespace MindSharper.Infrastructure.Authorization;

public class DeckAuthorizationService(
    ILogger<DeckAuthorizationService> logger,
    IUserContext userContext) : IResourceAuthorizationService<Deck>
{
    public bool IsAuthorized(Deck deck, ResourceOperation operation)
    {
        var currentUser = userContext.GetCurrentUser()!;
        logger.LogInformation("Attempt to authorize user (id: {UserId}) to perform {Operation} on Deck {DeckId}",
            currentUser.Id, operation, deck.Id);

        switch (operation)
        {
            case ResourceOperation.Create:
                logger.LogInformation("Create operation - authorization successful");
                return true;
            case ResourceOperation.Read or ResourceOperation.Delete or ResourceOperation.Update when deck.UserId == currentUser.Id:
                logger.LogInformation("Read or Delete or Update operation by the owner - authorization successful");
                return true;
            default:
                logger.LogInformation("Authorization unsuccessful");
                return false;
        }
    }
}