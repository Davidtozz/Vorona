import {writable, type Writable} from "svelte/store";

export const usernameStore: Writable<string> = writable<string>("");
export const messageHistoryStore: Writable<Message[]> = writable<Message[]>([]);