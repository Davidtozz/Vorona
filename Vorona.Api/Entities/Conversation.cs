using System;
using System.Collections.Generic;

namespace Vorona.Api.Entities;

public partial class Conversation
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
