namespace Mtg.Functions.Models;

public class Match
{
    public DateOnly MatchDate { get; set; }
    public List<Deck> Decks { get; set; } = new();
    public required Deck Winner { get; set; }
    public string? Note { get; set; }
}