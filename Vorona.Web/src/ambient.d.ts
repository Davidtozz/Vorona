//! Experimental types for Vorona.Web

declare type Message = {
    sender: string;
    content: string;
    timestamp: string;
    isRead?: false; // TODO: implement read receipts
    isSent?: false; // TODO: implement message status
} 

declare type Conversation = {
    name: string;
    lastMessage: string;
    isOnline?: boolean;
    avatarUrl?: string; 
}