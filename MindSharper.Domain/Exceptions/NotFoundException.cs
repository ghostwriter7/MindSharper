namespace MindSharper.Domain.Exceptions;

public class NotFoundException(string resource, string identifier)
    : Exception($"Resource {resource} identified by {identifier} does not exist.")
{
}