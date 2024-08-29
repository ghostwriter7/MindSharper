using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using MindSharper.Application.Services;
using MindSharper.Domain.Entities;

namespace MindSharper.API.Controllers;

[ApiController]
[Route("api/decks")]
public class DeckController(IDeckService deckService) : ControllerBase
{
    [HttpGet("{deckId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Deck?>> GetDeckById([FromRoute] int deckId)
    {
        var deck = await deckService.GetDeckByIdAsync(deckId);
        return Ok(deck);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Deck>>> GetDecks()
    {
        var decks = await deckService.GetDecksAsync();
        return Ok(decks);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateDeck([FromBody] Deck deck)
    {
        var deckId = await deckService.CreateDeckAsync(deck);
        return CreatedAtAction(nameof(GetDeckById), new { id = deckId });
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