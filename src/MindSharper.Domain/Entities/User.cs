using Microsoft.AspNetCore.Identity;

namespace MindSharper.Domain.Entities;

public class User : IdentityUser
{
    public List<Deck> OwnedDecks { get; set; } = [];
}