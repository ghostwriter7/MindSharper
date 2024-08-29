using MindSharper.Infrastructure.Extensions;
using MindSharper.Infrastructure.Seeders;

namespace MindSharper.API;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddInfrastructure(builder.Configuration);
        
        builder.Services.AddControllers();

        var app = builder.Build();

        var scope = app.Services.CreateScope();
        var databaseSeeder = scope.ServiceProvider.GetService<IDatabaseSeeder>();
        await databaseSeeder.Seed();
        
        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}
