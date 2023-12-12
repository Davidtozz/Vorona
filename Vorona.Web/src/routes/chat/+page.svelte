<script lang="ts">
    import {HubConnectionBuilder, LogLevel, HttpTransportType, HubConnection} from "@microsoft/signalr";
    import {onMount} from "svelte";
    import MessageBubble from "$lib/MessageBubble.svelte";

    /** 
     * Contains all messages that have been sent and received.
     */
    let messages: string[] = [];

    // name a variable containing both the user and the message sent

    let currentMessage: string = "";

    /**
     * The SignalR connection.
     */
    let connection: HubConnection;
    onMount(() => {
        connection = new HubConnectionBuilder()
            .withUrl("http://localhost:5207/chat", {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Debug)
            .build();

        try {
            connection.start()
                .then(() => console.log("Connection started!"))
        } catch(err) {
            console.error(err)
        }
        
        /* Event listeners */
        connection.on("ReceiveMessage", (user, message) => {
            messages = [...messages, message];
        });
    });


    /**
     * Sends the current message to the server using SignalR.
     */
    function sendText(): void {
        connection.invoke("SendMessage", currentMessage);
        currentMessage = "";
    }
</script>


<div class="chatbox">

    <div class="chatbox-messages">
        {#if messages.length > 0}
            {#each messages as message}
                <MessageBubble content={message}></MessageBubble>
            {/each}
        {:else}
            <p>No messages yet.</p>
        {/if}
    </div>

    <div class="chatFooter">
        <input bind:value={currentMessage} type="text" placeholder="Type your message here..." on:keydown={(e) => e.key === 'Enter' && sendText()}/>
        <button on:click={sendText}>Send</button>
    </div>
</div>

<style>
    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap');

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