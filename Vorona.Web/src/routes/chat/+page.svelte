<script lang="ts">
    import {HubConnectionBuilder, LogLevel, HttpTransportType, type HubConnection} from "@microsoft/signalr";
    import {onDestroy, onMount} from "svelte";
    import { usernameStore, messageHistoryStore, connectedUsersStore } from "$lib/stores";
    import * as Handlers from "$lib/eventHandlers";
    import UsernameModal from "$lib/components/UsernameModal.svelte";
    import MessageBubble from "$lib/components/MessageBubble.svelte";
	import Search from "$components/Search.svelte";
	import Conversation from "$components/Conversation.svelte";

    let currentMessage: string;
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
        connection.on("ReceivePrivateMessage", Handlers.onReceivePrivateMessage);
        connection.on("ConnectionEstablished", Handlers.onConnectionEstablished);
        connection.on("UserConnected", (user: string) => {
            connectedUsersStore.update(users => [...users, user])
        });
        connection.on("GetUsers", Handlers.onGetUsers);
    }   
    /**
     * Sends the current message to the server using SignalR.
     */
    function sendText(): void {
        if(currentMessage === "") return;

        if(currentMessage.startsWith("/whisper ")) {
            let whisperMessage = currentMessage.split(" ");
            console.log("Whisper message:" + whisperMessage)
            let receiver = whisperMessage[1];
            let message = whisperMessage.slice(2).join(" "); 
            connection.invoke("SendPrivateMessage", $usernameStore, message, receiver);

            currentMessage = "";
            return;
        }

        connection.invoke("SendMessage", $usernameStore, currentMessage);
        currentMessage = "";
    }

    onDestroy(() => {
        if(connection) {
            connection.invoke("Disconnect", $usernameStore).then(() => connection.stop());    
        };
        messageHistoryStore.set([]);
        //TODO: implement one on one chat (on frontend)
        //userConversationsStore.set([]);
    })

</script>

<div class="chatbox-wrapper">
    <div class="sidePanel">
        <div class="search-wrapper"><Search /></div>
        <div>
            {#each $connectedUsersStore as user}
            {#if user !== $usernameStore}
                <Conversation name={user}  lastMessage={"Lorem ipsum dolor sit amet"}/>
            {/if}
            {/each}
        </div>
    </div>

    <div class="chatbox">
        <div class="chatbox-header">
            <p>Logged in as: {$usernameStore}</p>
        </div>
        {#if $messageHistoryStore.length > 0}
            <div class="chatbox-messages">
                {#each $messageHistoryStore as message}
                    <!-- avoid rendering own message twice -->
                    {#if message.sender !== $usernameStore}
                        <MessageBubble content={message.content} sender={message.sender} isSentFromMe={false}></MessageBubble>
                    {:else}
                    <MessageBubble content={message.content} sender={$usernameStore} isSentFromMe={true}></MessageBubble>
                    {/if}
                {/each}
            </div>
        {:else}
        <div class="no-messages-wrapper">
            <p>No messages yet.</p>
        </div>
        {/if}
        <div class="chatFooter">
            <input bind:value={currentMessage} type="text" placeholder="Type your message here..." on:keydown={(e) => e.key === 'Enter' && sendText()}/>
            <button on:click={sendText}>Send</button> 
        </div>
    </div>
</div>

<style lang="scss">

    $sidePanelBackgroundColor: #605244;
    $chatboxBackgroundColor: #d5d2d2;

    @mixin center {
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .chatbox-wrapper {
        display: flex;
        flex-direction: row;
        height: 100dvh;
        background-color: grey;
        .sidePanel {
            flex: 3;
            display: flex;
            
            flex-direction: column;
            background-color: $sidePanelBackgroundColor;
            .search-wrapper {
                flex: 0;
                padding: 1rem;
                display: flex;
                justify-content: center;
                align-items: center;
                border-radius: .5rem;
            }
            div {
                align-self: stretch;
                flex: 1;
                display: flex;
                flex-direction: column;                
            }
            
        }
        .chatbox {
            padding: 1rem;
            flex: 7;
            display: flex;
            flex-direction: column;
            align-items: center;
            background-color: $chatboxBackgroundColor;
            /* border-radius: 1rem; */
            /* width: 50%;
            height: 80%; */
/*             box-shadow: 0 0 10px rgba(0, 0, 0, 0.5); */
            .no-messages-wrapper {
                @include center;
                flex-direction: column;
                gap: 1rem;
                height: 100%;
                width: 100%;
                p {
                    font-size: 24px;
                    font-family: 'Roboto';
                    border-radius: 1rem;
                }
            }
            .chatbox-messages {
                flex: 1;
                flex-direction: column;
                scroll-behavior: smooth;
                padding: 1rem;
                gap: .5rem;
                align-self: stretch;
                overflow-y: scroll;
            }
            .chatFooter {
                @include center;
                border-radius: .5rem;
                align-self: stretch;
                flex-direction: row;
                align-items: center;
                background-color: #fdfdfd;
            }
        }
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
        
        &:hover {
            outline: 2px solid black;
            border-color: black;
        }
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