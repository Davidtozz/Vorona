using Carter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Vorona.Api.Endpoints;

public class UsersModule : CarterModule
{

    public UsersModule() : base("/api/v1/users")
    {
        this.WithTags("/users");
    }
    
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/@me", GetCurrentUser);
        app.MapGet("/{user_id}", GetUser);
        app.MapGet("/{user_id}/conversations", SavedConversations);
        app.MapGet("/{user_id}/profile", UserProfile);
    }
    
    #region GetCurrentUserDocs
    [SwaggerOperation(Summary = "Returns some info about the current user",Description = "Some random ass description about the /@me endpoint")]
    [Authorize("user")]
    #endregion
    private async Task<IResult> GetCurrentUser(HttpContext context)
    {
        throw new NotImplementedException("The /@me route was not implemented yet.");
    }

    private async Task<IResult> GetUser([FromRoute] int user_id)
    {
        throw new NotImplementedException();
    }

    #region SavedConversationDocs
    [SwaggerOperation(Summary = "Gets the saved conversations of the user")]
    #endregion
    private async Task<IResult> SavedConversations(HttpContext context)
    {
        throw new NotImplementedException("The /{user_id}/conversations route was not implemented yet.");
    }

    private Task<IResult> UserProfile(HttpContext context)
    {
        throw new NotImplementedException();
    }
}