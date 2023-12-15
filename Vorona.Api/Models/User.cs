using System.Text.Json.Serialization;

namespace Vorona.Api.Models;

public class User
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    public User(string username,string password)
    {
        Username = username;
        Password = password;
    }
}