import {messageHistoryStore, connectedUsersStore, userConversationsStore} from "$lib/stores";

export function onReceiveMessage(user: string, message: string) {
    const timestamp = new Date();

    messageHistoryStore.update(history => [...history, {
        sender: user, 
        content: message,
        timestamp: `${timestamp.getHours()}:${timestamp.getMinutes()}`
    }]);
}

//? REMINDER: To send a message, remember to first add a conversation, manually, to the userConversationsStore
export function onReceivePrivateMessage(author: string, message: string, recipient: string) {
    const timestamp = new Date();    

    console.log(`You (${recipient}) received a private message from ${author}: ${message}`)

    userConversationsStore.update((conversation: Conversation[]) => {
        /* const conversationExists = conversation.some(c => c.name === author); */
        //TODO: implement conversationExists, if it doen't exist, create it and add it to the store
        /* if(!conversationExists) {
            let conversationName = author === recipient ? author : recipient;
            conversation.push({
                name: conversation, 
                history: [{
                    sender: author,
                    content: message,
                    timestamp: `${timestamp.getHours()}:${timestamp.getMinutes()}`
                }],
                lastMessage: message
            })
        } else */ {
            conversation.forEach((c: Conversation) => {
            
            if (c.name === author) {
                c.history.push({
                    sender: author,
                    content: message,
                    timestamp: `${timestamp.getHours()}:${timestamp.getMinutes()}`
                })
                c.lastMessage = message;
            }
          })
        }

        
        return conversation;
    })
};

export function onConnectionEstablished(connectedGuests: string[])  {
    connectedUsersStore.set(connectedGuests);

}

export function onGetUsers(users: object) {
    console.table(users)
}

export function onUserConnected(user: string) {
    connectedUsersStore.update(users => [...users, user]);
}