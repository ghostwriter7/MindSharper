﻿using Microsoft.Extensions.DependencyInjection;
using MindSharper.Application.Decks.Dtos;
using MindSharper.Application.Flashcards.Dtos;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace MindSharper.Application.Extensions;

public static class ServiceCollectionsExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionsExtensions).Assembly;
        services.AddAutoMapper(typeof(DeckProfile), typeof(FlashcardProfile));
        
        services.AddValidatorsFromAssembly(assembly)
            .AddFluentValidationAutoValidation();

        services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));
    }
}