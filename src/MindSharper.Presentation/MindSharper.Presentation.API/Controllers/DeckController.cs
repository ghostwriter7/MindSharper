﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using MindSharper.Application.Decks.Commands.CreateDeck;
using MindSharper.Application.Decks.Commands.DeleteDeck;
using MindSharper.Application.Decks.Commands.UpdateDeckName;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Decks.Queries.GetDeckByIdQuery;
using MindSharper.Application.Decks.Queries.GetDecks;
using MindSharper.Domain.Entities;

namespace MindSharper.Presentation.API.Controllers;

[ApiController]
[Route("api/decks")]
public class DeckController(IMediator mediator) : ControllerBase
{
    [HttpGet("{deckId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<DeckDto?>> GetDeckById([FromRoute] int deckId)
    {
        var deckDto = await mediator.Send(new GetDeckByIdQuery(deckId));
        return Ok(deckDto);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MinimalDeckDto>>> GetDecks([FromQuery] GetDecksQuery getDecksQuery)
    {
        var deckDtos = await mediator.Send(getDecksQuery);
        return Ok(deckDtos);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize]
    public async Task<IActionResult> CreateDeck([FromBody] CreateDeckCommand command)
    {
        var deckId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetDeckById), new { deckId }, null);
    }

    [HttpPatch("name")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> UpdateDeckName([FromBody] UpdateDeckNameCommand command)
    {
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{deckId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> DeleteDeck([FromRoute] int deckId)
    {
        await mediator.Send(new DeleteDeckCommand(deckId));
        return NoContent();
    }
}