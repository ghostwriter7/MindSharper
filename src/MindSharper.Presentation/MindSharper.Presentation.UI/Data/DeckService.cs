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
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer CfDJ8BjnyNePyPFIo4je5y5RYUp_iomKHDneQhxChvBzPwIZftrh6Oa0rTcszP2w4ESiSa0BmO54GMEn64ZuFVy7uLOV2SqVpJqSoRHH3h-_cC-jzI-jvp3vHQMl-Ri5P--3NxiSsskTr1AAND4dkq4fdmgXscrWrTONTsg-fTxq8--3hsKsIRBMC_zohYEk4OmfkEufK48M-kfDI_YZOwX9cVIKv1bP1IXTQ3_6ruFPGUOVnjI1Q5WE9kh0mp1_6KytU3-xFH7S6YTwHVqG-YkssjS7OPoKinbWAuCcDt1epAZtq5Ygzm8UXBQEmyIKDMXGQLFyIo3WaarzcP0cft22rq6iEMQvf_CabbsfElYNlR9POBIAFWHKgVYdy1aHQx1GmqNsss169o2mtc27RbJf54zCcNz3V61jxWMYobVmHVRcXwnf4xMZdgq2KwesvSVa9pMuGHdgX3S8uJlk1rhw38MpJvjwPcE4paNOmYumo9xuZmfAbHCO9RS1RX6mh6tVSmf5ct5tfR-3hvQ7Se4IHTatXW1WbttKKvhHtD46_4IZJNJcFtaUDMvCA5-mLDadBTwx-oj9qfBpoV6Pxh1H18sPXRZXQ4wOqbIzDPoKJ-oqo4AK5Obwb7NB1kAHV1h-heM6BJ0lJjCFL1cgswQ1mHhyjRmB4MEvp88xJH5F2BoGNJHJzmiyxVYwCWEuY1Gzbw");
        var pagedResult = await httpClient.GetFromJsonAsync<PagedResult<MinimalDeckDto>>("http://localhost:5273/api/decks?pageNumber=1&pageSize=5");
        
        return pagedResult;
    }
}