using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Vorona.Api.Entities;

public partial class Message
{   [JsonIgnore]
    public int Id { get; set; }
    [JsonIgnore]
    public int UserId { get; set; }
    [JsonIgnore]
    public string Content { get; set; } = null!;
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public int? ReplyToMessageId { get; set; }
    [JsonIgnore]
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    [JsonIgnore]
    public virtual ICollection<Conversation> ConversationLastMessages { get; set; } = new List<Conversation>();
    [JsonIgnore]
    public virtual ICollection<Conversation> ConversationMessages { get; set; } = new List<Conversation>();
    [JsonIgnore]
    public virtual ICollection<Message> InverseReplyToMessage { get; set; } = new List<Message>();
    [JsonIgnore]
    public virtual Message? ReplyToMessage { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
