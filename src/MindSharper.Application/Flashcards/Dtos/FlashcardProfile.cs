using AutoMapper;
using MindSharper.Application.Flashcards.Commands.CreateFlashcard;
using MindSharper.Application.Flashcards.Commands.UpdateFlashcard;
using MindSharper.Domain.Entities;

namespace MindSharper.Application.Flashcards.Dtos;

public class FlashcardProfile : Profile
{
    public FlashcardProfile()
    {
        CreateMap<Flashcard, FlashcardDto>();
        CreateMap<CreateFlashcardCommand, Flashcard>();
    }
}