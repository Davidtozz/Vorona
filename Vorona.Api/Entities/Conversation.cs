using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


namespace Vorona.Api.Entities;

public partial class Conversation
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public DateTime UpdatedAt { get; set; }
    [JsonIgnore]
    public int? MessageId { get; set; }
    [JsonIgnore]
    public int? LastMessageId { get; set; }
    [JsonIgnore]
    public virtual Message? LastMessage { get; set; }
    [JsonIgnore]
    public virtual Message? Message { get; set; }
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
