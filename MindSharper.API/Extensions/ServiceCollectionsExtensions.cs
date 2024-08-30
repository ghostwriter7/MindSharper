using MindSharper.API.Middlewares;

namespace MindSharper.API.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<ErrorHandlingMiddleware>();
        
    }
}