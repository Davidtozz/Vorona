// place files you want to import through the `$lib` alias in this folder.

import {writable, type Writable} from "svelte/store";

export const usernameStore: Writable<string> = writable<string>("");