import {writable, type Writable} from "svelte/store";

export const usernameStore: Writable<string> = writable<string>("");
export const messageHistoryStore: Writable<Message[]> = writable<Message[]>([]);
//TODO: implement one on one chat (on frontend)
//export const userConversationsStore: Writable<Conversation[]> = writable<Conversation[]>([]);
export const connectedUsersStore: Writable<string[]> = writable<string[]>([]);
