using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MindSharper.Infrastructure.Persistance;

namespace MindSharper.Infrastructure.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MindSharperDatabaseContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("MindSharperDb"));
        });
    }
}