using MindSharper.Domain.Constants;

namespace MindSharper.Domain.Interfaces;

public interface IResourceAuthorizationService<in T>
{
    bool IsAuthorized(T resource, ResourceOperation operation);
}