using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;

namespace MindSharper.Application.Flashcards.Queries.GetFlashcards;

public class GetFlashcardsQueryHandler(
    ILogger<GetFlashcardsQueryHandler> logger,
    IMapper mapper,
    IDeckRepository deckRepository) : IRequestHandler<GetFlashcardsQuery, IEnumerable<FlashcardDto>>
{
    public async Task<IEnumerable<FlashcardDto>> Handle(GetFlashcardsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Retrieving flashcards from deck ({request.DeckId})");
        var deck = await deckRepository.GetDeckByIdAsync(request.DeckId)
                   ?? throw new NotFoundException(nameof(Deck), request.DeckId.ToString());

        var flashcards = mapper.Map<IEnumerable<FlashcardDto>>(deck.Flashcards);
        return flashcards;
    }
}