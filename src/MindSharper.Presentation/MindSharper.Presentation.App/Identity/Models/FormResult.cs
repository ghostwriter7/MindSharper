namespace MindSharper.Presentation.App.Identity.Models;

public class FormResult
{
    public bool Succeeded { get; set; }
    public string[] ErrorList { get; set; } = default!;
}