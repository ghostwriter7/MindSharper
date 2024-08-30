using MindSharper.Infrastructure.Extensions;
using MindSharper.Infrastructure.Seeders;
using MindSharper.Application.Extensions;
using MindSharper.API.Extensions;
using MindSharper.API.Middlewares;

namespace MindSharper.API;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddPresentation();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication();
        

        var app = builder.Build();

        var scope = app.Services.CreateScope();
        var databaseSeeder = scope.ServiceProvider.GetService<IDatabaseSeeder>()!;
        await databaseSeeder.Seed();

        app.UseMiddleware<ErrorHandlingMiddleware>();
        
        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}
