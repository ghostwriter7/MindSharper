using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MindSharper.Domain.Repositories;
using MindSharper.Infrastructure.Persistance;
using MindSharper.Infrastructure.Repositories;
using MindSharper.Infrastructure.Seeders;

namespace MindSharper.Infrastructure.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MindSharperDatabaseContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("MindSharperDb"));
        });

        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();
        services.AddScoped<IDeckRepository, DeckRepository>();
        services.AddScoped<IFlashcardRepository, FlashcardRepository>();
    }
}