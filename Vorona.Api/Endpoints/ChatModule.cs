using System.Diagnostics;
using System.Security.Claims;
using Carter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Vorona.Api.Data;
using Vorona.Api.Entities;

namespace Vorona.Api.Endpoints;

public class ChatModule : CarterModule
{


    public ChatModule() : base("/api/v1/chat")
    {
        this.WithTags("/chat");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {   //should return the contents of the conversation, if the user is a member of the conversation
        app.MapGet("/{conversation_id}", 
            #region SwaggerDocs
            [SwaggerOperation(Summary = "Returns the contents of the conversation, if the user is a member of the conversation")]
            [SwaggerResponse(200, "The contents of the conversation")]
            [SwaggerResponse(401, "The user is not a member of the conversation")]
            [SwaggerResponse(404, "The conversation does not exist")]
            [SwaggerResponse(500, "Internal server error")]
            #endregion
            [Authorize("user")]
            ([FromRoute] int conversation_id,
                PostgresContext db,
                HttpContext ctx
            ) =>
            {
                //First, check if the conversation exists
                throw new NotImplementedException();
                    
                Conversation? conversation = db.Conversations.FirstOrDefault(c => c.Id == conversation_id);
                if (conversation is null)
                {
                    return Results.NotFound();
                }
                
                //Check if the user is a member of the conversation
                string? username = ctx.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? default;
                
                return Results.Ok(new {NoContents = "Endpoint "});

            });

        app.MapPost("/{conversation_id}/messages", 
        #region SwaggerDocs

        [SwaggerOperation(Summary = "Sends a message to the conversation")]
        [SwaggerResponse(200, "The message was sent successfully")]
        [SwaggerResponse(401, "The user is not allowed to send messages to the conversation")]
        [SwaggerResponse(404, "Conversation doesn't exist")]
        [SwaggerResponse(500, "Internal server error")]

        #endregion

        ([FromRoute] Guid conversation_id) => { throw new NotImplementedException(); });

        //TODO add pagination
        app.MapGet("/{conversation_id}/messages",
        #region SwaggerDocs
        [SwaggerOperation(Summary = "Returns the messages of a given conversation")]
        [SwaggerResponse(200, "The messages of the conversation")]
        [SwaggerResponse(204, "The conversation is empty")]
        [SwaggerResponse(401, "The user is not in the conversation")]
        #endregion
        () => {throw new NotImplementedException();});

        app.MapDelete("/{conversation_id}/messages/{message_id}", 
        #region SwaggerDocs
        [SwaggerOperation(Summary = "Deletes a message from the conversation")]
        [SwaggerResponse(200, "The message was deleted successfully")]
        [SwaggerResponse(401, "The user is not allowed to delete (message_id) from the conversation")]
        [SwaggerResponse(404, "Conversation or message doesn't exist")]
        #endregion
        ([FromRoute] Guid conversation_id, [FromRoute] Guid message_id) => {throw new NotImplementedException();});
        //? Inform users in the conversation that the user is typing
        app.MapPost("/{conversation_id}/typing", 
        #region SwaggerDocs
        [SwaggerOperation(Summary = "Informs users in the conversation that the user is typing")]
        [SwaggerResponse(204, "User is typing")]
        #endregion
        ([FromRoute] Guid conversation_id) => { throw new NotImplementedException();});
        
        app.MapGet("/{conversation_id}/{message_id}", 
        #region SwaggerDocs
        [SwaggerOperation(Summary = "Returns the contents of a given message, in a given conversation")]
        [SwaggerResponse(200, "The contents of the message")]
        [SwaggerResponse(401, "The user is not in the conversation")]
        [SwaggerResponse(404, "Conversation or message doesn't exist")]
        #endregion

        ([FromRoute] Guid conversation_id, [FromRoute] Guid message_id) => {throw new NotImplementedException();});
    }
}