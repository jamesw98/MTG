using System.Text.Json.Serialization;
using MudBlazor;

namespace Mtg.Blazor.Models;

/// <summary>
/// Currently, I don't need the whole card, just the images. 
/// </summary>
public class ScryFallCard
{
    [JsonPropertyName("image_uris")]
    public ImageUris ImageUris { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("color_identity")]
    public string[] ColorIdentity { get; set; }

    public override string ToString()
    {
        return Name; 
    }
}