// place files you want to import through the `$lib` alias in this folder.

import {writable, type Writable} from "svelte/store";

export const username: Writable<string> = writable<string>("");