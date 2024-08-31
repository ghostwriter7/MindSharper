using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Flashcards.Commands.CreateFlashcard;

public class CreateFlashcardCommandHandler(ILogger<CreateFlashcardCommandHandler> logger,
    IMapper mapper,
    IDeckRepository deckRepository,
    IFlashcardRepository flashcardRepository) : IRequestHandler<CreateFlashcardCommand, int>
{
    public async Task<int> Handle(CreateFlashcardCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Creating a {nameof(Flashcard)} for {nameof(Deck)} ({request.DeckId})");
        var deck = await deckRepository.GetDeckByIdAsync(request.DeckId)
                   ?? throw new NotFoundException(nameof(Deck), request.DeckId.ToString());

        var flashcard = mapper.Map<Flashcard>(request);
        flashcard.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

        var flashcardId = await flashcardRepository.CreateFlashcardAsync(flashcard);
        return flashcardId;
    }
}