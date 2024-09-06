using System.Reflection;
using MindSharper.Application.Users;
using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Interfaces;
using MindSharper.Domain.Repositories;
using Moq;

namespace MindSharper.Tests.Common.Helpers;

public static class SetupHelper
{
    public static void SetUpGetFlashcardByIdAsync(Mock<IFlashcardRepository> flashcardRepositoryMock, int deckId,
        int flashcardId,
        Flashcard? flashcard) => flashcardRepositoryMock.Setup(repo => repo.GetFlashcardByIdAsync(deckId, flashcardId))
        .ReturnsAsync(flashcard);

    public static void SetUpUserContext(Mock<IUserContext> userContextMock, CurrentUser? currentUser) =>
        userContextMock.Setup(userContext => userContext.GetCurrentUser()).Returns(currentUser);

    public static void SetUpAuthorizationService(Mock<IResourceAuthorizationService<Deck>> authorizationServiceMock,
        Deck deck, ResourceOperation operation, bool result) => authorizationServiceMock
        .Setup(authService => authService.IsAuthorized(deck, operation)).Returns(result);

    public static void SetUpGetDeckByIdAsync(Mock<IDeckRepository> deckRepositoryMock, int deckId, Deck? deck) =>
        deckRepositoryMock.Setup(repo => repo.GetDeckByIdAsync(deckId)).ReturnsAsync(deck);
}