using MindSharper.Domain.Constants;
using MindSharper.Domain.Entities;
using MindSharper.Domain.Interfaces;
using Moq;

namespace MindSharper.Tests.Common.Helpers;

public static class SetupHelper
{
    public static void SetUpAuthorizationService(Mock<IResourceAuthorizationService<Deck>> authorizationServiceMock,
        Deck deck, ResourceOperation operation, bool result) => authorizationServiceMock
        .Setup(authService => authService.IsAuthorized(deck, operation)).Returns(result);
}