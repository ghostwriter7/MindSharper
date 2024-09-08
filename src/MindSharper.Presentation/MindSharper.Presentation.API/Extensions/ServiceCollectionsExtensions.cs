using Microsoft.OpenApi.Models;
using MindSharper.Presentation.API.Middlewares;
using Serilog;

namespace MindSharper.Presentation.API.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options => options.AddPolicy(name: "corsPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:5289")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }));
        
        builder.Services.AddControllers();
        builder.Services.AddScoped<ErrorHandlingMiddleware>();
        
        builder.Services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo { Title = "MindSharper API", Version = "v1" });

            var securityDefinitionName = "bearerAuth";
            config.AddSecurityDefinition(securityDefinitionName, new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = securityDefinitionName
                        }
                    },
                    []
                }
            });
        });

        builder.Services.AddAuthentication();
        builder.Services.AddEndpointsApiExplorer();

        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }
}