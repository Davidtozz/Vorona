using System.Collections;
using System.Data.Common;
using Carter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using Vorona.Api.Data;
using Vorona.Api.Entities;
using Vorona.Api.Models;

namespace Vorona.Api.Endpoints;

public class ChatModule : CarterModule
{


    public ChatModule() : base("/api/v1/chat")
    {
        this.WithTags("/chat");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {   
        app.MapPost("/new", 
            [SwaggerOperation(Summary = "Creates a new conversation with the given name.")]
            [SwaggerResponse(201, "The conversation was created successfully")]
            /*[Authorize("user")]*/ async (
            [FromBody] NewConversation newConversation, 
            HttpContext ctx,
            PostgresContext db) =>
        {
            List<User> matchingUsers = db.Users.Where(x => newConversation.Participants.Contains(x.Username)).ToList();

            int[] userIds = matchingUsers.Select(x => x.Id).ToArray();

            if (userIds.Length != newConversation.Participants.Length)
            {
                return Results.Problem(
                    statusCode: 400,
                    title: "Bad request",
                    detail: "One or more of the participants do not exist"
                    );
            }
            
            var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                await db.Database.ExecuteSqlInterpolatedAsync($"SELECT public.create_conversation(ARRAY[{userIds}],{newConversation.ConversationName});");
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                return Results.Created();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return Results.Problem(
                    statusCode: 500,
                    title: "Internal server error",
                    detail: e.Message);
            }
        });
        
        app.MapGet("/{conversation_id}", 
            #region SwaggerDocs
            [SwaggerOperation(Summary = "Returns the contents of the conversation")]
            [SwaggerResponse(200, "The contents of the conversation")]
            [SwaggerResponse(204, "The conversation is empty")]
            [SwaggerResponse(401, "The user is not a member of the conversation")]
            [SwaggerResponse(404, "The conversation does not exist")]
            [SwaggerResponse(500, "Internal server error")]
            #endregion
            [Authorize("user")]
            async ([FromRoute] int conversation_id,
                PostgresContext db,
                HttpContext ctx
            ) =>
            {

                string contextUsername = ctx.User.Claims.FirstOrDefault(c => c.Type == "username")!.Value;
                
                var conversationExists = await db.Conversations.AnyAsync(x => x.Id == conversation_id);
                if (!conversationExists)
                {
                    return Results.NotFound();
                }
                
                var query = from conversation in db.Conversations
                            where conversation.Id == conversation_id
                            from user in conversation.Users
                            from message in user.Messages
                            select new
                            {
                                ConversationId = conversation_id,
                                Username = user.Username,
                                MessageContent = message.Content,
                                MessageId = message.Id
                            };
                
                if (query.Any())
                {
                    var result = await query.ToListAsync();

                    bool isMember = result.Any(x => x.Username == contextUsername);
                    return isMember ? Results.Ok(result) : Results.Unauthorized();
                }
                
                return Results.NoContent();
            });

        app.MapPost("/{conversation_id}/messages", 
        #region SwaggerDocs

        [SwaggerOperation(Summary = "Sends a message to the conversation")]
        [SwaggerResponse(200, "The message was sent successfully")]
        [SwaggerResponse(401, "The user is not allowed to send messages to the conversation")]
        [SwaggerResponse(404, "Conversation doesn't exist")]
        [SwaggerResponse(500, "Internal server error")]

        #endregion

        ([FromRoute] Guid conversation_id, [FromBody] Message messageContent) =>
        {
            throw new NotImplementedException();
        });

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
        
        app.MapGet("/{conversation_id}/messages/{message_id}", 
        #region SwaggerDocs
        [SwaggerOperation(Summary = "Returns the contents of a given message, in a given conversation")]
        [SwaggerResponse(200, "The contents of the message")]
        [SwaggerResponse(401, "The user is not in the conversation")]
        [SwaggerResponse(404, "Conversation or message doesn't exist")]
        #endregion

        ([FromRoute] Guid conversation_id, [FromRoute] Guid message_id) => {throw new NotImplementedException();});
    }
}