using Moq;
using Microsoft.AspNetCore.SignalR;
using Vorona.Api.Hubs;
using Vorona.Api.Services;

namespace Vorona.Api.Tests.Unit;

public class MessageHubTests
{
    [Fact]
    public async void OnDisconnectedAsync_Should_RemoveUserFromUserTracker()
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

    //TODO: Fix this test
    /*[Fact]
    public async void OnConnectedAsync_Should_AddUserToUserTracker()
    {
        //Arrange
        var mockClients = new Mock<IHubCallerClients>();
        var mockGroups = new Mock<IGroupManager>();
        var mockCaller = new Mock<ISingleClientProxy>();
        var mockContext = new Mock<HubCallerContext>();

        mockClients.Setup(clients => clients.Caller).Returns(mockCaller.Object);

        mockCaller
            .Setup(caller => caller.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object[]>(),
                default(CancellationToken)))
            .Returns(Task.CompletedTask)
            .Verifiable();
            
        var userTracker = new UserTracker();
        userTracker.Users.TryAdd("testConnectionId", "testUser");

        var messageHub = new MessageHub(userTracker)
        {
            Clients = mockClients.Object,
            Groups = mockGroups.Object,
            Context = mockContext.Object
        };

        //Act
        await messageHub.OnConnectedAsync();
        
        //Assert 
        Assert.True(userTracker.Users.ContainsKey("testConnectionId"));
    }*/

}