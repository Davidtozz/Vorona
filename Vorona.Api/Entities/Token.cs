using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Vorona.Api.Entities;

public partial class Token
{   
    [JsonIgnore]
    public Guid Id { get; set; }
    [JsonIgnore]    
    public int UserId { get; set; }
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public DateTime UpdatedAt { get; set; }
    [JsonIgnore]
    public DateTime ExpiresAt { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
