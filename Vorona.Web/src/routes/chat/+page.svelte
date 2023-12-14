<script lang="ts">
    import {HubConnectionBuilder, LogLevel, HttpTransportType, HubConnection} from "@microsoft/signalr";
    import {onMount} from "svelte";
    import { usernameStore, messageHistoryStore } from "$lib/stores";
    import * as Handlers from "$lib/eventHandlers";
    import UsernameModal from "$lib/components/UsernameModal.svelte";
    import MessageBubble from "$lib/components/MessageBubble.svelte";
    import { page } from "$app/stores";
	import { writable, type Writable } from "svelte/store";

    /**
     * The current message that is being typed.
     */
    let currentMessage: string;
    /**
     * The SignalR connection.
     */
    let connection: HubConnection;
    
    $: if($usernameStore !== "") {
        connection = new HubConnectionBuilder()
        .withUrl(`http://localhost:5207/chat?username=${$usernameStore}`, {
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Debug)
        .build();

        try {
            connection.start()
        } catch(err) {
            console.error(err)
        }
        
        /* Event listeners */
        connection.on("ReceiveMessage", Handlers.onReceiveMessage);
        connection.on("Connected", Handlers.onConnectionEstablished);
        connection.on("GetUsers", Handlers.onGetUsers);
    }   
    /**
     * Sends the current message to the server using SignalR.
     */
    function sendText(): void {
        if(currentMessage === "") return;
        connection.invoke("SendMessage", $usernameStore, currentMessage);
        currentMessage = "";
    }

    function showHistory(): void {
        console.table("History of messages: " + JSON.stringify(messageHistoryStore));
    }
</script>

<div class="chatbox">
    {#if $usernameStore === ""}
    <UsernameModal></UsernameModal>
    {/if}
    {#if $messageHistoryStore.length > 0}
        <div class="chatbox-messages">

            {#each $messageHistoryStore as message}
                <MessageBubble content={message.content} sender={message.sender}></MessageBubble>
            {/each}
        </div>
    {:else}
        <p>No messages yet.</p>
        
    
    {/if}
    <div class="chatFooter">
        <input bind:value={currentMessage} type="text" placeholder="Type your message here..." on:keydown={(e) => e.key === 'Enter' && sendText()}/>
        <button on:click={sendText}>Send</button>
        
        <!-- DEBUG -->
        <!-- <button on:click={showHistory}>Log history</button>
        <button on:click={() => connection.invoke("FetchUsers")}>Log cache</button>
        <button on:click={() => {$page.url.searchParams.set('username', $usernameStore); console.log($page.url.searchParams.get('username'))}}>Set query url</button> -->
    </div>
</div>

<style>
    .chatbox {
        display: flex;
        justify-content: flex-end;
        align-items: center;
        flex-direction: column;
        gap: 1rem;
        height: 100dvh;
        background-color: grey;
    }

    .chatFooter {
      display: flex;
      justify-content: center;
      align-items: center;
      flex-direction: row;
      align-items: center;
      background-color: #fdfdfd;
    }

    .chatbox-messages {
        display: flex;
        flex-direction: column;
        scroll-behavior: smooth;
        padding: 1rem;
        gap: .5rem;
        
        overflow-y: scroll;
    }

    input {
        flex: 5;
        padding: 0.5rem;
        border: none;
        border-radius: 0.5rem;
        font-size: 24px;
        font-family: 'Roboto';
    }

    button {
        flex: 1;
        background-color: brown;
        font-size: 24px;
        padding: 0.5rem;
        border: none;
        border-radius: 0.5rem;
        font-family: 'Roboto';
    }

    button:hover {
        border: 5px solid black;
        border-color: black;
    }

    p {
        display: inline-block;
        width: min-content;
        white-space: nowrap;
        overflow: hidden;
        background-color: black;
        padding: 1rem;
        color: white;
        font-size: 16px;
        font-family: 'Roboto';
        text-align: center;
    }
</style>