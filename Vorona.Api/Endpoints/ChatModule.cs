using System.Collections;
using System.Data.Common;
using Carter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        app.MapPost("/new", NewConversation);
        app.MapGet("/{conversation_id}", GetConversationContents);
        app.MapPost("/{conversation_id}/messages", SendMessage);
        app.MapDelete("/{conversation_id}/messages/{message_id}", DeleteMessage);
        /*app.MapPost("/{conversation_id}/typing", Typing);*/
    }
    
    #region NewConversationDocs
    [SwaggerOperation(Summary = "Creates a new conversation with the given name.",
        Description = "The conversation name must be UNIQUE. The participants must be valid users"
    )]
    [SwaggerResponse(201, "The conversation was created successfully")]
    #endregion
    private async Task<IResult> NewConversation([FromBody] NewConversationModel newConversation, HttpContext ctx, PostgresContext db)
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
            await db.Database.ExecuteSqlInterpolatedAsync($"SELECT public.create_conversation(ARRAY[{userIds}],{newConversation.ConversationName}, {newConversation.Type});");
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
    }

    #region GetConversationContentsDocs
    [SwaggerOperation(Summary = "Returns the contents of the conversation")]
    [SwaggerResponse(200, "The contents of the conversation")]
    [SwaggerResponse(204, "The conversation is empty")]
    [SwaggerResponse(401, "The user is not a member of the conversation")]
    [SwaggerResponse(404, "The conversation does not exist")]
    [SwaggerResponse(500, "Internal server error")]
    #endregion
    [Authorize("user")]
    private async Task<IResult> GetConversationContents([FromRoute] int conversation_id, PostgresContext db, HttpContext ctx)
    {
        string contextUsername = ctx.User.Claims.FirstOrDefault(c => c.Type == "username")!.Value;
                
        var conversationExists = db.ConversationExists(conversation_id);
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
    }
    
    #region SendMessageDocs
    [SwaggerOperation(Summary = "Sends a message to the conversation")]
    [SwaggerResponse(200, "The message was sent successfully")]
    [SwaggerResponse(401, "The user is not allowed to send messages to the conversation")]
    [SwaggerResponse(404, "Conversation doesn't exist")]
    [SwaggerResponse(500, "Internal server error")]
    #endregion
    private async Task<IResult> SendMessage([FromRoute] int conversation_id, [FromBody] NewMessageModel message, PostgresContext db, HttpContext ctx)
    {

        if (message.Content.IsNullOrEmpty())
        {
            return Results.BadRequest("Can't send an empty message");
        }

        bool conversationExists = db.ConversationExists(conversation_id);
        if (!conversationExists)
        {
            return Results.NotFound();
        }
            
        string contextUsername = ctx.User.Claims.FirstOrDefault(c => c.Type == "username")!.Value;

        bool userExists = db.UserExists(contextUsername, out var user);

        if(userExists)
        {
            bool isMember = await db.Conversations
                .AnyAsync(c => c.Id == conversation_id && c.Users.Contains(user));
            if (!isMember)
            {
                return Results.Unauthorized();
            }
        }
        else
        {
            return Results.Unauthorized();
        }
            
        var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Messages.AddAsync(
                new Message()
                {
                    Content = message.Content,
                    UserId = user.Id,
                    User = user,
                    ConversationId = conversation_id,
                }
            );
                
            await transaction.CommitAsync();
            await db.SaveChangesAsync();
            return Results.Created();
        }
        catch
        {
            await db.Database.RollbackTransactionAsync();
            return Results.Problem(
                statusCode: 500,
                title: "Internal server error",
                detail: "Something went wrong while sending the message");
        }

    }
    
    #region DeleteMessageDocs
    [SwaggerOperation(Summary = "Deletes a message from the conversation")]
    [SwaggerResponse(200, "The message was deleted successfully")]
    [SwaggerResponse(401, "The user is not allowed to delete (message_id) from the conversation")]
    [SwaggerResponse(404, "Conversation or message doesn't exist")]
    #endregion
    private async Task<IResult> DeleteMessage([FromRoute] int conversation_id, [FromRoute] int message_id, PostgresContext db, HttpContext ctx)
    {
        if(db.ConversationExists(conversation_id) && 
           db.MessageExists(message_id, out var messageFound))
        {
            var transaction = await db.Database.BeginTransactionAsync();
            try
            {
                db.Messages.Remove(messageFound);
                await transaction.CommitAsync();
                await db.SaveChangesAsync();
                return Results.Ok($"Message {message_id} was deleted successfully!");
            }
            catch(Exception e)
            {
                await transaction.RollbackAsync();
                return Results.Problem(
                    statusCode: 500,
                    title: "Internal server error",
                    detail: e.Message);
            }
        }
        return Results.NotFound();
    }
    
}



