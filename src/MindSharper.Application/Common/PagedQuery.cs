using MediatR;

namespace MindSharper.Application.Common;

public class PagedQuery<T> : IRequest<T>
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}