using AutoMapper;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Flashcards.Dtos;

public class FlashcardProfile : Profile
{
    public FlashcardProfile()
    {
        CreateMap<Flashcard, FlashcardDto>();
    }
}