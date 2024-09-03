using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MindSharper.Application.Flashcards.Commands.CreateFlashcard;
using MindSharper.Application.Flashcards.Commands.DeleteFlashcard;
using MindSharper.Application.Flashcards.Commands.UpdateFlashcard;
using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Application.Flashcards.Queries.GetFlashcardById;
using MindSharper.Application.Flashcards.Queries.GetFlashcards;

namespace MindSharper.API.Controllers;

[ApiController]
[Route("api/decks/{deckId:int}/flashcards")]
public class FlashcardController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize]
    public async Task<ActionResult<int>> CreateFlashcard([FromRoute] int deckId, [FromBody] CreateFlashcardCommand command)
    {
        command.DeckId = deckId;
        var flashcardId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetFlashcardById), new { deckId, flashcardId }, null);
    }

    [HttpGet("{flashcardId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FlashcardDto?>> GetFlashcardById([FromRoute] int deckId, [FromRoute] int flashcardId)
    {
        var flashcard = await mediator.Send(new GetFlashcardByIdQuery(deckId, flashcardId));
        return Ok(flashcard);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<FlashcardDto>>> GetFlashcardsByDeckId([FromRoute] int deckId)
    {
        var results = await mediator.Send(new GetFlashcardsQuery(deckId));
        return Ok(results);
    }

    [HttpDelete("{flashcardId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFlashcard([FromRoute] int deckId, [FromRoute] int flashcardId)
    {
        await mediator.Send(new DeleteFlashcardCommand(deckId, flashcardId));
        return NoContent();
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize]
    public async Task<IActionResult> UpdateFlashcard([FromRoute] int deckId, [FromBody] UpdateFlashcardCommand command)
    {
        command.DeckId = deckId;
        await mediator.Send(command);
        return NoContent();
    }
}