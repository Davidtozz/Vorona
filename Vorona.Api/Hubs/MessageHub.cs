using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Vorona.Api.Hubs;

public sealed class MessageHub : Hub
{
    private Dictionary<string, string> Cache = new();
    private readonly Random random = new();
    private uint ConnectedClients = 0;

    /// <summary>
    /// Send a message to all connected clients
    /// </summary>
    /// <param name="user">The message author's name </param>
    /// <param name="message">The contents of the message sent</param>
    /// <returns></returns>
    public async Task SendMessage(string message)
    {
        Console.WriteLine($"Received message from {Context.ConnectionId}: {message}");
        await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);
    }

    public override async Task OnConnectedAsync()
    {
        //TODO extract JWT from headers to identify user
        string connectionId = Context.ConnectionId;
        ConnectedClients++;

        Cache.Add(connectionId, $"Guest{random.Next(0, 1000)}");


        Console.WriteLine($"Client ({Cache[connectionId]}) connected. Current clients: {ConnectedClients}");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;
        ConnectedClients--;

        Cache.Remove(connectionId);

        Console.WriteLine($"Client ({connectionId}) disconnected. Current clients: {ConnectedClients}");

        await base.OnDisconnectedAsync(exception);
    }


    public async Task ReceiveMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
