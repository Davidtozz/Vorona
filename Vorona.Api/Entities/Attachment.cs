using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vorona.Api.Entities;

public partial class Attachment
{
    [JsonIgnore]
    public int Id { get; set; }
    [JsonIgnore]
    public int MessageId { get; set; }
    [JsonIgnore]
    public byte[]? File { get; set; }
    [JsonIgnore]
    public virtual Message Message { get; set; } = null!;
}
