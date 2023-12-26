import {writable, type Writable} from "svelte/store";

export const usernameStore: Writable<string> = writable<string>("");
/**  
 * @deprecated You should use userConversationsStore instead
 * */ 
export const messageHistoryStore: Writable<Message[]> = writable<Message[]>([]);
export const userConversationsStore: Writable<Conversation[]> = writable<Conversation[]>([
    {
        name: "Lobby",
        history: [],
        lastMessage: ""
    }
]);
export const connectedUsersStore: Writable<string[]> = writable<string[]>([]);

