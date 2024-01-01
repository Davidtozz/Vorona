using System.Text.Json.Serialization;
using Vorona.Api.Entities;

namespace Vorona.Api.Models;

public class NewMessageModel
{
    public int FromUserId { get; set; }
    public string Content { get; set; } = null!;
    [JsonIgnore]
    public int? ReplyToMessageId { get; set; } = null;
    [JsonIgnore]
    public ICollection<Attachment>? Attachments { get; set; } = null;
}