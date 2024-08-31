using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using MindSharper.Application.Flashcards.Dtos;
using MindSharper.Application.Flashcards.Queries.GetFlashcardById;
using MindSharper.Application.Tests.Fixtures;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Exceptions;
using MindSharper.Domain.Repositories;
using Moq;
using Xunit;

namespace MindSharper.Application.Tests.Flashcards.Queries.GetFlashcardById;

[TestSubject(typeof(GetFlashcardByIdQueryHandler))]
public class GetFlashcardByIdQueryHandlerTest
{
    private readonly Mock<ILogger<GetFlashcardByIdQueryHandler>> _loggerMock = new();
    private readonly IMapper _mapper;
    private readonly Mock<IFlashcardRepository> _repositoryMock = new();
    private readonly GetFlashcardByIdQuery _query = new(1, 2);
    private readonly GetFlashcardByIdQueryHandler _handler;

    public GetFlashcardByIdQueryHandlerTest()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<FlashcardProfile>());
        _mapper = mapperConfig.CreateMapper();
        _handler = new GetFlashcardByIdQueryHandler(_loggerMock.Object, _mapper, _repositoryMock.Object);
    }
    
    
    [Fact]
    public async Task Handle_ForExistingFlashcard_ShouldReturnFlashcardDto()
    {
        var flashcard = FlashcardFixtures.GetAnyFlashcard();
        _repositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_query.DeckId, _query.FlashcardId))
            .ReturnsAsync(flashcard);

        var result = await _handler.Handle(_query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_mapper.Map<FlashcardDto>(flashcard));
    }

    [Fact]
    public void Handle_ForNonExistingFlashcardId_ShouldThrowNotFoundException()
    {
        _repositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(_query.DeckId, _query.FlashcardId))
            .ReturnsAsync((Flashcard)null);

        Func<Task> action = async () => await _handler.Handle(_query, CancellationToken.None);

        action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Resource Flashcard identified by {_query.FlashcardId} does not exist.");
    }
}