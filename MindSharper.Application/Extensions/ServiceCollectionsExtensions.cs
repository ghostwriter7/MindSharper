using Microsoft.Extensions.DependencyInjection;

namespace MindSharper.Application.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var appAssembly = typeof(ServiceCollectionExtensions).Assembly;
        
        services.AddAutoMapper(appAssembly);
    }
}