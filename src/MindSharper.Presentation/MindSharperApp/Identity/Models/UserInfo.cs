namespace MindSharperApp.Identity.Models;

public class UserInfo
{
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public Dictionary<string, string> Claims { get; set; } = [];
}