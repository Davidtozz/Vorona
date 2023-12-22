using System.Collections.Concurrent;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Vorona.Api.Services;

namespace Vorona.Api.Hubs;


public sealed class MessageHub : Hub
{
    /// <summary>
    /// The <see cref="UserTracker"/> instance for this hub.<br>
    /// This allows us to track connected users and their names.
    /// </summary>
    private readonly UserTracker _userTracker;
    private readonly Random random = new();
    private uint ConnectedClients = 0;
    private const string DEBUG_PREFIX = "\x1b[31mdbug:\x1b[0m";


    public MessageHub(UserTracker userTracker)
    {
        _userTracker = userTracker;
    }

    /// <summary>
    /// Send a message to all connected clients
    /// </summary>
    /// <param name="user">The message author's name </param>
    /// <param name="message">The contents of the message sent</param>
    /// <returns></returns>
    public async Task SendMessage(string sender, string message)
    {

        Console.WriteLine($"{DEBUG_PREFIX} Received message from {sender}: {message}");
        await Clients.All.SendAsync("ReceiveMessage", sender, message);
    }

    public async Task SendPrivateMessage(string fromUser, string message, string toUser)
    {
        string receiver = _userTracker.Users.FirstOrDefault(x => x.Value == toUser).Key;

        Console.WriteLine($"{DEBUG_PREFIX} Received private message from {fromUser} to {toUser}: {message}");
        await Clients.Clients(Context.ConnectionId, receiver).SendAsync("ReceivePrivateMessage", fromUser, message);
    }

    public override async Task OnConnectedAsync()
    {
        //TODO extract JWT from headers to identify user
        string connectionId = Context.ConnectionId;
        ConnectedClients++;

        string connectedUser = Context.User!.Claims.FirstOrDefault(c => c.Type == "username")!.Value;

        Console.WriteLine($"{DEBUG_PREFIX} Client connected: {connectedUser}");

        _userTracker.Users.TryAdd(connectionId, connectedUser);
        Console.WriteLine($"{DEBUG_PREFIX} User {connectedUser} connected. Current clients: {ConnectedClients}");
        Console.WriteLine($"{DEBUG_PREFIX} Users({_userTracker.Users.Count}): {string.Join(", ", _userTracker.Users.Select(x => $"{x.Key} => {x.Value}"))}");

        await Clients.Caller.SendAsync("Connected", _userTracker.Users[connectionId], _userTracker.Users);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Overrides the <see cref="Hub.OnDisconnectedAsync(Exception)"/> method to handle client disconnection.
    /// </summary>
    /// <param name="exception">The exception that caused the disconnection, if any.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectedClients--;
        _userTracker.Users.Remove(Context.ConnectionId, out string? _);
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Receives a message from a user and sends it to all clients.
    /// </summary>
    /// <param name="user">The name of the user sending the message.</param>
    /// <param name="message">The content of the message.</param>
    public async Task ReceiveMessage(string user, string message) =>
        await Clients.All.SendAsync("ReceiveMessage", _userTracker.Users[user], message);
    
}
