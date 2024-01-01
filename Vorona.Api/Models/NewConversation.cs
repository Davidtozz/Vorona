using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Vorona.Api.Entities;

namespace Vorona.Api.Models;

public class NewConversationModel
{
    [JsonPropertyName("name")]
    public required string ConversationName { get; init; }
    [JsonPropertyName("type")]
    public required string Type { get; init; }
    [JsonPropertyName("participants")]
    public required string[] Participants { get; init; }
    
}