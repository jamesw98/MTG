using System.Text.Json.Serialization;

namespace Mtg.Blazor.Models;

public class ScryFallList
{
    [JsonPropertyName("data")]
    public List<ScryFallCard> Data { get; set; }
}