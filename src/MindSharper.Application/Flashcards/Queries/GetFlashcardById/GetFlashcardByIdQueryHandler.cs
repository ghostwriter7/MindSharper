using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Application.Flashcards.Queries.GetFlashcards;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Flashcards.Queries.GetFlashcardById;

public class GetFlashcardByIdQueryHandler(
    ILogger<GetFlashcardByIdQueryHandler> logger,
    IMapper mapper,
    IFlashcardRepository flashcardRepository) : IRequestHandler<GetFlashcardByIdQuery, FlashcardDto?>
{
    public async Task<FlashcardDto?> Handle(GetFlashcardByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Retrieving a {nameof(Flashcard)} identified by id: {request.FlashcardId} from {nameof(Deck)} ({request.DeckId})");
        var flashcard = await flashcardRepository.GetFlashcardByIdAsync(request.DeckId, request.FlashcardId)
                        ?? throw new NotFoundException(nameof(Flashcard), request.FlashcardId.ToString());

        var flashcardDto = mapper.Map<FlashcardDto>(flashcard);
        return flashcardDto;
    }
}