using System;
using System.Collections.Generic;

namespace Vorona.Api.Entities;

public partial class Message
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ConversationId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual User User { get; set; } = null!;
}
