﻿using MindSharper.Application.Common;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Decks.Queries.GetDecks;

namespace MindSharper.Application.Helpers;

public static class PagingHelper
{
    public static PagedResult<T> GetPagedResult<T>(IEnumerable<T> results, int total,  PagedQuery<PagedResult<T>> query)
    {
        return new PagedResult<T>(
            results,
            total,
            query.PageNumber,
            query.PageSize
        );
    }
}