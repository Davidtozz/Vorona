using System;
using System.Collections.Generic;

namespace Vorona.Api.Entities;

public partial class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int? ReplyToMessageId { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual ICollection<Message> InverseReplyToMessage { get; set; } = new List<Message>();

    public virtual Message? ReplyToMessage { get; set; }

    public virtual User User { get; set; } = null!;
}
