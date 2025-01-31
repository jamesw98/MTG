using MongoDB.Bson.Serialization.Attributes;

namespace Mtg.Functions.Models;

[BsonIgnoreExtraElements]
public class Match
{
    public DateTime MatchDate { get; set; }
    public List<Deck> Decks { get; set; } = new();
    public required Deck Winner { get; set; }
    public string? Note { get; set; }
}