using MediatR;
using Microsoft.AspNetCore.Mvc;
using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Application.Flashcards.Queries.GetFlashcards;

namespace MindSharper.API.Controllers;

[ApiController]
[Route("api/decks/{deckId:int}/flashcards")]
public class FlashcardController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateFlashcard([FromRoute] int deckId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{flashcardId:int}")]
    public async Task<ActionResult<FlashcardDto>> GetFlashcardById([FromRoute] int deckId, [FromRoute] int flashcardId)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FlashcardDto>>> GetFlashbardsByDeckId([FromRoute] int deckId)
    {
        var results = await mediator.Send(new GetFlashcardsQuery(deckId));
        return Ok(results);
    }

    [HttpDelete("{flashcardId:int}")]
    public async Task<IActionResult> DeleteFlashcard([FromRoute] int deckId, [FromRoute] int flashcardId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateFlashcard([FromRoute] int deckId)
    {
        throw new NotImplementedException();
    }
}