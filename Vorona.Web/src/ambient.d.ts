//! Experimental types for Vorona.Web

type Message = {
    sender: string;
    content: string;
    timestamp: string;
    isRead?: false; // TODO: implement read receipts
    isSent?: false; // TODO: implement message status
} 

type Conversation = {
    id?: string; // GUID
    name: string;
    isSelected?: boolean;
    history: Message[];
    lastMessage?: string;
    isOnline?: boolean;
    avatarUrl?: string; 
}