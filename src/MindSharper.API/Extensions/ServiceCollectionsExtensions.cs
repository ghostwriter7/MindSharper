using Microsoft.OpenApi.Models;
using MindSharper.API.Middlewares;

namespace MindSharper.API.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<ErrorHandlingMiddleware>();
        
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo { Title = "MindSharper API", Version = "v1" });
        });
    }
}