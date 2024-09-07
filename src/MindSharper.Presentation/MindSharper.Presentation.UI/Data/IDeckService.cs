using MindSharper.Application.Common;
using MindSharper.Application.Decks.Dtos;

namespace MindSharper.Presentation.UI.Data;

public interface IDeckService
{
    Task<PagedResult<MinimalDeckDto>> GetDecksAsync();
}