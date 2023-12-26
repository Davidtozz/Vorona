import {connectedUsersStore, userConversationsStore} from "$lib/stores";

function onReceiveMessage(user: string, message: string) {
    
    userConversationsStore.update(conversations => {
        conversations.forEach((c: Conversation) => {
            if (c.name === "Lobby") {
                c.history.push({
                    sender: user,
                    content: message,
                    timestamp: new Date().toLocaleTimeString().slice(0, -3)
                })
                c.lastMessage = `${user}: ${message}`;
            }
        })
        return conversations;
    });
}

function onReceivePrivateMessage(author: string, message: string, recipient: string) {
    
    const newMessage: Message = {
        sender: author,
        content: message,
        timestamp: new Date().toLocaleTimeString().slice(0, -3)
    }

    console.log(`You (${recipient}) received a private message from ${author}: ${message}`)

    userConversationsStore.update((conversations: Conversation[]) => {
        let conversationExists: boolean = false;

        conversations.forEach((conversation: Conversation) => {
            if (conversation.name === author) {
                conversationExists = true;
                conversation.history.push(newMessage);
                conversation.lastMessage = message;
            }
        });

        if (!conversationExists) {
            conversations.push({
                name: author,
                history: [newMessage],
                lastMessage: message
            });
        }

        return conversations;
    });
}

function onConnectionEstablished(connectedGuests: string[])  {
    connectedUsersStore.set(connectedGuests);
}

/* function onGetUsers(users: object) {
    console.table(users)
} */

function onUserConnected(user: string) {
    connectedUsersStore.update(users => [...users, user]);
}

function onUserOffline(user: string) {
    connectedUsersStore.update(users => users.filter(u => u !== user));
}

export {
    onReceiveMessage,
    onReceivePrivateMessage,
    onConnectionEstablished,
   /*  onGetUsers, */
    onUserConnected,
    onUserOffline
}