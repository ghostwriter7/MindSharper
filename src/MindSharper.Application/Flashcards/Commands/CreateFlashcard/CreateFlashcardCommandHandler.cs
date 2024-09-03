using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Helpers;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Flashcards.Commands.CreateFlashcard;

public class CreateFlashcardCommandHandler(
    ILogger<CreateFlashcardCommandHandler> logger,
    IMapper mapper,
    IDeckRepository deckRepository,
    IFlashcardRepository flashcardRepository,
    IUserContext userContext,
    IResourceAuthorizationService<Deck> authorizationService) : IRequestHandler<CreateFlashcardCommand, int>
{
    public async Task<int> Handle(CreateFlashcardCommand request, CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser()!;
        logger.LogInformation("Attempt to create a Flashcard for Deck ID: {DeckId} by User ID: {UserId}",
            request.DeckId, currentUser.Id);
        var deck = await deckRepository.GetDeckByIdAsync(request.DeckId)
                   ?? throw new NotFoundException(nameof(Deck), request.DeckId.ToString());

        if (!authorizationService.IsAuthorized(deck, ResourceOperation.Update))
            throw new UnauthorizedException(nameof(Deck), deck.Id, currentUser.Id);
        
        var flashcard = mapper.Map<Flashcard>(request);
        flashcard.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

        try
        {
            var flashcardId = await flashcardRepository.CreateFlashcardAsync(flashcard);
            return flashcardId;
        }
        catch (Exception ex) when (ExceptionHelper.IsUniqueConstraintViolationException(ex))
        {
            throw new DuplicateResourceException(nameof(Flashcard), nameof(Flashcard.Frontside), flashcard.Frontside);
        }
    }
}