using System;
using System.Collections.Generic;

namespace Vorona.Api.Entities;

public partial class Attachment
{
    public Guid Id { get; set; }

    public int MessageId { get; set; }

    public byte[]? File { get; set; }

    public virtual Message Message { get; set; } = null!;
}
