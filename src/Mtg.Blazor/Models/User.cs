using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Mtg.Blazor.Models;

public class User
{
    public string UserId { get; set; }
    public string Nickname { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}