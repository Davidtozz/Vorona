using System.Text.Json.Serialization;

namespace Vorona.Api.Models;

public class User
{
    [JsonPropertyName("username")]
    public string Username { get; init; }

    [JsonPropertyName("password")]
    public string Password { get; init; }

    [JsonPropertyName("role")]
    public string Role { get; init; }

    public User(string username,string password, string role = "user")
    {
        Username = username;
        Password = password;
        Role = role;
    }
}