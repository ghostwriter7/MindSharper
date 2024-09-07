using MindSharper.Application.Decks.Dtos;
using System.Net.Http.Json;
using MindSharper.Application.Common;

namespace MindSharper.Presentation.UI.Data;

public class DeckService : IDeckService
{
    public async Task<PagedResult<MinimalDeckDto>> GetDecksAsync()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer CfDJ8BjnyNePyPFIo4je5y5RYUqFQuoXoAtjOl86sW0ncHRE25TxTy3EWgu6u6vklriMm3hPRAG6jncTgKFHUiIpv-W7DIHx3ySgHwDr986SAAMqH9uUfDIFV_JgrfWhA_N2fId7km25xCr9eF9yYp0hNLgaoe7RBnDEvroLV1tbawBVFKkc-dHNIuHzOhs7o2c1gatpQgL0iCs_LnBUfXkvTMEiKmJmmsDn5X297mJ2zBBG3l8ROd757hpiduyKtTdksolfXyCCmdR5-tOKG2cXInFJd96S2ivkpC_S6u4PubdLp1KWixeWsWjoUef29gbtWPQQkOdszxIyzIh7BJr3WgfrjmiLc3ydrqV-M1hhMRc9T4FYJwQOgG9mbsqjiW9pV6UjR-GOxKwV3olILCkUGD7rt109IEJ8HJVhqKqSmrgZZmKxv75KM9BsOhYYrl_Hwk8ylhPZr1hZwAgE5HEqkO2h0bppUdT3rY3pT02Akkp6nnD10cw0KepJtgqlFRolBptNmFVVCyhi9746d1ZJ-1lLu8JV13M4W2RFexUS3COYo3T6uUaRDRZodtgQjqkH7MmwiUfLa0ZnTFRT_kp-kmi_N9vLrY5vMWKnmFJ490PHp4fRZ8uB2TBgyCPpbE1h85ACcnr6CdmH_aKi70mhCObWliKYb7BF7uJLvkwvTbOaNN8dU6IezhVkUl6k4XPQlA");
        var pagedResult = await client.GetFromJsonAsync<PagedResult<MinimalDeckDto>>("http://localhost:5273/api/decks?pageNumber=1&pageSize=5");
        
        return pagedResult;
    }
}