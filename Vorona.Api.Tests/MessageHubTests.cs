using Moq;
using Microsoft.AspNetCore.SignalR;
using Vorona.Api;
using Vorona.Api.Hubs;
using Vorona.Api.Services;

namespace Vorona.Api.Tests;

public class MessageHubTests 
{

    [Fact]
    public async void OnDisconnectedAsync_Should_Remove_User_From_UserTracker()
    {
        //? Arrange
        var mockClients = new Mock<IHubCallerClients>();
        var mockGroups = new Mock<IGroupManager>();
        var mockContext = new Mock<HubCallerContext>();

        mockContext.SetupGet(m => m.ConnectionId).Returns("testConnectionId");

        var userTracker = new UserTracker();
        userTracker.Users.TryAdd("testConnectionId", "testUser");

        var messageHub = new MessageHub(userTracker) 
        {
            Clients = mockClients.Object,
            Groups = mockGroups.Object,
            Context = mockContext.Object
        };
        
        //? Act
        await messageHub.OnDisconnectedAsync(null);

        //? Assert
        Assert.False(userTracker.Users.ContainsKey("testConnectionId"));

    }

}