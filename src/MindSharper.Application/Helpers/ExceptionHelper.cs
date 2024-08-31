using System.Data.Common;

namespace MindSharper.Application.Helpers;

public static class ExceptionHelper
{
    public static bool IsUniqueConstraintViolationException(Exception exception)
    {
        return exception.InnerException is DbException innerException &&
               innerException.Message.Contains("duplicate key");
    }
}