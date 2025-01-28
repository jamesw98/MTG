using MongoDB.Bson;

namespace Mtg.Blazor.Models;

/// <summary>
/// Represents a single deck.
/// </summary>
public class Deck
{
    /// <summary>
    /// The ID from mongo.
    /// </summary>
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// My ID for this deck.
    /// </summary>
    public Guid DeckGuid { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The name of the deck.
    /// </summary>
    public required string DeckName { get; set; }
    
    /// <summary>
    /// URL to the deck. Moxfield, Archidekt, etc.
    /// </summary>
    public required string DeckUrl { get; set; }
    
    /// <summary>
    /// The color to associate with this deck.
    /// </summary>
    public required int Color { get; set; }
    
    /// <summary>
    /// The commander of this deck.
    /// </summary>
    public required string Commander { get; set; }
    
    /// <summary>
    /// The mana colors for this deck.
    /// </summary>
    public IEnumerable<string> ColorIdentity { get; set; }
    
    /// <summary>
    /// Optional: Secondary commander.
    /// </summary>
    public string? SecondaryCommander { get; set; }

    /// <summary>
    /// ID of the user that made this deck.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Name of the user that made this deck.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Scryfall card image url for the commander.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsValid()
    {
        return DeckName.Trim() != string.Empty && DeckUrl != string.Empty && Commander != string.Empty;
    }

    public override string ToString()
    {
        return DeckName;
    }
}