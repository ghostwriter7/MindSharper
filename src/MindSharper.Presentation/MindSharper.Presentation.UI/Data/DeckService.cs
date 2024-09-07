using MindSharper.Application.Decks.Dtos;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MindSharper.Application.Common;

namespace MindSharper.Presentation.UI.Data;

public class DeckService(IHttpClientFactory httpClientFactory) : IDeckService
{
    public async Task<PagedResult<MinimalDeckDto>> GetDecksAsync()
    {
        using var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer CfDJ8BjnyNePyPFIo4je5y5RYUqcC3Sa26wUjDAyXpq9YlEIY_Xh1OrUSHRW5zrccgX4yH4-U9ooZpK64uuUZ6QksCDp5vuEB8DOxgaq5h9KNi4Jt5gLdo_nymXrkbyN3YO1_WAzIvRYiOSuU3BJgoD8XT1zYTRlTtjHi-JwEkpelHkCZIEAO9MdTp-5RkdhuWaaID0FzFFdo-bWZT8bNahh-Otkq-yFulzbcF0QTKIepSEU4wz9qrFqBkJIQNJVd34CYo8F5cv3xXjOsLcRzC2iwiQ7Bl_KllNl5CwX7yKAh73nh7E2hor2JJ5N2CEIzax8XBUGF_tDLPCkV9Kidt2pTbAaQhiv3WLF9UJtvnq39YFueHE6YXKstcmNEPUH61eI1p8pPtxNJqGZIY3HmFUSng1QXkqsSYZfZaPU1otRQfDaJZuj4D1vAxgibhCrljl4pEz0keveWlZDoZZnN8MacouNYeXfcEyy4TgbebXzq89x0jdoDaVn0PrCe3amx2EsH2klKjRIXX-bBDsXLUTqsk4dMWGGsNLEXbfmgVNep0PJ0oCjPrB9sLfc6Jdf0JfoR1qUXvk2XmQGbpd6x8DHbXZcZTx6gmwWT9wANzDOsd_V3XqpxgvDxaBqzW5ghUeo9Pr2zpBaf12SEAaCFht_oh9-QadFqfSm3Asg68GIOFgNUIW_mxY-TBNhGzZt-qYIUQ");
        var pagedResult = await httpClient.GetFromJsonAsync<PagedResult<MinimalDeckDto>>("http://localhost:5273/api/decks?pageNumber=1&pageSize=5");
        
        return pagedResult;
    }
}