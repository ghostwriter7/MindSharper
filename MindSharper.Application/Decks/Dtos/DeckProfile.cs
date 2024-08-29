using AutoMapper;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Decks.Dtos;

public class DeckProfile : Profile
{
    public DeckProfile()
    {
        CreateMap<Deck, DeckDto>();
        CreateMap<Deck, MinimalDeckDto>();
        CreateMap<CreateDeckDto, Deck>();
    }
}