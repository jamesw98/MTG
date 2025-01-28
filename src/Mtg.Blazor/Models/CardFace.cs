using System.Text.Json.Serialization;

namespace Mtg.Blazor.Models;

public class CardFace
{
    [JsonPropertyName("image_uris")]
    public ImageUris ImageUris { get; set; }
}