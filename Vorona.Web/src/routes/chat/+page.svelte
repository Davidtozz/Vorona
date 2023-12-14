<script lang="ts">
    import {HubConnectionBuilder, LogLevel, HttpTransportType, HubConnection} from "@microsoft/signalr";
    import {onMount} from "svelte";
    import { usernameStore } from "$lib/index";
    import UsernameModal from "$lib/UsernameModal.svelte";
    import MessageBubble from "$lib/MessageBubble.svelte";
    import { page } from "$app/stores";

    /** 
     * Contains all messages that have been sent and received.
     */
    let history: Message[] = [];

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
        connection.on("ReceiveMessage", (user, message) => {
            history = [...history, {sender: user, content: message}];
        });

        connection.on("Connected", (generatedGuestName, connectedGuests) => {
            
            console.log("Connection started! Connected as: " + generatedGuestName);
            console.table(connectedGuests)
            
        /*  currentUser = username; */
        });

        connection.on("Users", (users) => {
            console.table(users)
        });
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
        console.table("History of messages: " + JSON.stringify(history));
    }
</script>



<div class="chatbox">

    {#if $usernameStore === ""}
    <UsernameModal></UsernameModal>
    {/if}
    <div class="chatbox-messages">
        {#if history.length > 0}
            {#each history as message}
                <MessageBubble content={message.content} sender={message.sender}></MessageBubble>
            {/each}
        {:else}
            <p>No messages yet.</p>
        {/if}
    </div>

    <div class="chatFooter">
        <input bind:value={currentMessage} type="text" placeholder="Type your message here..." on:keydown={(e) => e.key === 'Enter' && sendText()}/>
        <button on:click={sendText}>Send</button>
        
        <!-- DEBUG -->
        <button on:click={showHistory}>Log history</button>
        <button on:click={() => connection.invoke("FetchUsers")}>Log cache</button>
        <button on:click={() => {$page.url.searchParams.set('username', $usernameStore); console.log($page.url.searchParams.get('username'))}}>Set query url</button>
    </div>
    

    
</div>

<style>


    .chatbox {
        display: flex;
        justify-content: flex-end;
        flex-direction: column;
        align-items: center;
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

    button:focus {
        outline: none;
    }
</style>