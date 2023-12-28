

using System.Net;
using Carter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        app.MapGet("/@me", 
        #region SwaggerDocs
        [SwaggerOperation(
            Summary = "Returns some info about the current user",
            Description = "Some random ass description about the /@me endpoint"
        )]
        [Authorize(Roles = "user")]
        #endregion
        () => "(Some info about the current user!)");
        app.MapGet("/{user_id}", ([FromRoute] Guid user_id) => $"(Some info about the user with id {user_id})");
        
        app.MapGet("/{user_id}/conversations", 
        #region
        [SwaggerOperation(
            Summary = "Gets the saved conversations of the user"
        )]
        #endregion
        ([FromRoute] Guid user_id) => $"(Some info about the channels of the user with id {user_id})");
        
        app.MapPost("/{user_id}/conversations", 
        [SwaggerOperation(
            Summary = ""
        )]
        ([FromRoute] Guid user_id) => {});

        app.MapGet("/{user_id}/profile", ([FromRoute] Guid user_id) => {});
    }
}