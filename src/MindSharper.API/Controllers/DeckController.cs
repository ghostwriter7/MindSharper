using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Decks.Queries.GetDeckByIdQuery;
using MindSharper.Application.Decks.Queries.GetDecks;
using MindSharper.Application.Services;
using MindSharper.Domain.Entities;

namespace MindSharper.API.Controllers;

[ApiController]
[Route("api/decks")]
public class DeckController(IDeckService deckService, IMediator mediator) : ControllerBase
{
    [HttpGet("{deckId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeckDto?>> GetDeckById([FromRoute] int deckId)
    {
        var deckDto = await mediator.Send(new GetDeckByIdQuery() { DeckId = deckId });
        return Ok(deckDto);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MinimalDeckDto>>> GetDecks()
    {
        var deckDtos = await mediator.Send(new GetDecksQuery());
        return Ok(deckDtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDeck([FromBody] CreateDeckDto createDeckDto)
    {
        var deckId = await deckService.CreateDeckAsync(createDeckDto);
        return CreatedAtAction(nameof(GetDeckById), new { deckId }, null);
    }

    [HttpPatch("{deckId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDeckName([FromRoute] int deckId, [FromQuery] string name)
    {
        await deckService.UpdateDeckNameAsync(deckId, name);
        return NoContent();
    }

    [HttpDelete("{deckId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDeck([FromRoute] int deckId)
    {
        await deckService.DeleteDeckAsync(deckId);
        return NoContent();
    }
}