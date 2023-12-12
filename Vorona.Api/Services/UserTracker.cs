using System.Collections.Concurrent;
namespace Vorona.Api.Services;


/// <summary>
/// Represents a user tracker that keeps track of users using a concurrent dictionary.
/// </summary>
public sealed class UserTracker
{
    public ConcurrentDictionary<string, string> Users { get; } = new();
}