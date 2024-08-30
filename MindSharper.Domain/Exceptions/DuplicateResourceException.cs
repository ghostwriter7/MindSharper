namespace MindSharper.Domain.Exceptions;

public class DuplicateResourceException(string resource, string propertyName, string propertyValue)
    : Exception($"{resource} with {propertyName}: {propertyValue} already exists")
{
}