using Microsoft.Extensions.DependencyInjection;
using MindSharper.Infrastructure.Persistance;

namespace MindSharper.Infrastructure.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<MindSharperDatabaseContext>();
    }
}