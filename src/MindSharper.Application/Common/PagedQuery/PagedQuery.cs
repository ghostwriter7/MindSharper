using MediatR;
using Microsoft.AspNetCore.Http;

namespace MindSharper.Application.Common.PagedQuery;

public class PagedQuery<T> : IRequest<T>, IPagedQuery
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}
