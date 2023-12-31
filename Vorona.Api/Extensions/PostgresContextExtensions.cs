using Microsoft.EntityFrameworkCore;
using Vorona.Api.Data;
using Vorona.Api.Entities;

public static class PostgresContextExtensions
{
    public static bool ConversationExists(this PostgresContext db, int conversationId) =>
        db.Conversations.AnyAsync(c => c.Id == conversationId).Result;
    
    public static bool UserExists(this PostgresContext db, int userId) =>
        db.Users.AnyAsync(u => u.Id == userId).Result;
    
    public static bool UserExists(this PostgresContext db, string username) =>
        db.Users.AnyAsync(u => u.Username == username).Result;
    
    public static bool UserIsMemberOfConversation(this PostgresContext db, int userId, int conversationId) =>
        db.Conversations.AnyAsync(c => c.Id == conversationId && c.Users.Any(u => u.Id == userId)).Result;
    
    public static bool UserIsMemberOfConversation(this PostgresContext db, string username, int conversationId) =>
        db.Conversations.AnyAsync(c => c.Id == conversationId && c.Users.Any(u => u.Username == username)).Result;

    public static bool MessageExists(this PostgresContext db, int messageId, out Message? messageFound)
    {
        //db.Messages.AnyAsync(m => m.Id == messageId).Result;
        messageFound = db.Messages.FirstOrDefaultAsync(m => m.Id == messageId).Result;        
        
        return messageFound != null;
    }
    
    public static bool MessageExists(this PostgresContext db, int messageId, int conversationId) =>
        db.Messages.AnyAsync(m => m.Id == messageId && m.ConversationId == conversationId).Result;
}