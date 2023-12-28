using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Vorona.Api.Entities;


public partial class User
{
    [JsonIgnore]
    public int Id { get; set; }
    
    public string Username { get; set; } = null!;
    
    public string Email { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    [JsonIgnore]
    public string Role { get; set; } = null!;
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public string Settings { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    [JsonIgnore]
    public virtual Token? Token { get; set; }
    [JsonIgnore]
    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
}
