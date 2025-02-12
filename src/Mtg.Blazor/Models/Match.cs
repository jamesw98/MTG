namespace Mtg.Blazor.Models;

public class Match
{
    public DateTime MatchDate { get; set; }
    public DateOnly MatchDateOnly => DateOnly.FromDateTime(MatchDate);
    public List<Deck> Decks { get; set; } = new();
    public required Deck Winner { get; set; }
    public string? Note { get; set; }

    public bool IsValid()
    {
        return Decks.Count > 1;
    }
}