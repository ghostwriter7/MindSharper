namespace MindSharper.Domain.Exceptions;

public class UnauthorizedException(string resource, int resourceId, string userId)
    : Exception($"Access denied to {resource} ({resourceId}) for User ({userId}) to ")
{
}