using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mtg.Functions.Models;

/// <summary>
/// Represents a single deck.
/// </summary>
public class Deck
{
    /// <summary>
    /// The ID from mongo.
    /// </summary>
    [BsonElement("_id")]
    public ObjectId Id { get; set; }
    
    /// <summary>
    /// My ID for this deck.
    /// </summary>
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
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
    /// Optional: Secondary commander.
    /// </summary>
    public string? SecondaryCommander { get; set; }
    
    /// <summary>
    /// The user that made this deck.
    /// </summary>
    public User User { get; set; } 
}