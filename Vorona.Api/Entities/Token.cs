using System;
using System.Collections.Generic;

namespace Vorona.Api.Entities;

public partial class Token
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
}
