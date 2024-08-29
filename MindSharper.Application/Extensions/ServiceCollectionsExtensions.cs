﻿using Microsoft.Extensions.DependencyInjection;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Services;

namespace MindSharper.Application.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DeckProfile));

        services.AddScoped<IDeckService, DeckService>();
    }
}