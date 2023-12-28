<script lang="ts">
    import {HubConnectionBuilder, LogLevel, HttpTransportType, type HubConnection} from "@microsoft/signalr";
    import {onDestroy} from "svelte";
    import { usernameStore,  connectedUsersStore, userConversationsStore } from "$lib/stores";
    import {onReceiveMessage, onReceivePrivateMessage, onConnectionEstablished, /* onGetUsers ,*/ onUserConnected, onUserOffline} from "$lib/eventHandlers";
    import Message from "$components/Message.svelte";
	import SidePanel from "$components/SidePanel.svelte";
	

    let currentMessage: string;
    let connection: HubConnection;

    $: if($usernameStore !== "") {
        connection = new HubConnectionBuilder()
        .withUrl(`http://localhost:5000/chat`, {
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
        connection.on("ReceiveMessage", onReceiveMessage);
        connection.on("ReceivePrivateMessage", onReceivePrivateMessage);
        connection.on("ConnectionEstablished", onConnectionEstablished);
        connection.on("UserConnected", onUserConnected);
        connection.on("UserOffline", onUserOffline)
    }   


    function isGlobalChat(): boolean | undefined {
        return $userConversationsStore.find(c => c.name === "Lobby")?.isSelected;
    }

    /**
     * Sends the current message to the server using SignalR.
     */
    function sendText(): void {
        if(currentMessage === "") return;

        if(isGlobalChat()) {
            connection.invoke("SendMessage", $usernameStore, currentMessage);
            currentMessage = "";
            return;
        }
        //check which chat is currently selected, and send the message to the appropriate user
        
        userConversationsStore.update(conversations => {
            conversations.forEach(c => {
                if (c.isSelected) {
                    c.history.push({
                        sender: $usernameStore,
                        content: currentMessage,
                        timestamp: new Date().toLocaleTimeString().slice(0, -3)
                    });
                    c.lastMessage = currentMessage;
                }
            });
            connection.invoke("SendPrivateMessage", $usernameStore, currentMessage, conversations.find(c => c.isSelected)?.name);
            return conversations;
        })
        currentMessage = "";
    }

    $: console.table($userConversationsStore);

</script>

<div class="chatbox-wrapper">
    <SidePanel />
    <div class="chatbox">
        <div class="chatbox-messages">
            {#each $userConversationsStore as conversation}
                
                {#if conversation?.isSelected}
                    <center><p>You're chatting with: {conversation.name}</p></center>
                    {#each conversation.history as message}
                        {#if message.sender === $usernameStore}
                            <Message content={message.content} sender={$usernameStore} isSentFromMe={$usernameStore === message.sender} />
                        {:else}
                            <Message content={message.content} sender={message.sender} isSentFromMe={$usernameStore === message.sender} />
                        {/if}
                    {/each}
                {/if}
            {/each}
        </div>
        <footer class="chatFooter">
            <input bind:value={currentMessage} type="text" placeholder="Send a message in here..." on:keydown={(e) => e.key === 'Enter' && sendText()}/>
        </footer>
    </div>
</div>

<style lang="scss">

    @import "../../global.scss";
    $chatboxBackgroundColor: #0582ca;

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
        
        .chatbox {
            padding: 1rem;
            flex: 7;
            display: flex;
            flex-direction: column;
            align-items: center;
            background-color: $chatboxBackgroundColor;
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

            .chatbox-header {
                display: flex;
                flex-direction: row;
                justify-content: space-between;
                align-items: center;
                align-self: stretch;
                padding: 1rem;
                gap: 1rem;
                background-color: #fdfdfd;
                border-radius: .5rem;
                ul {
                    display: flex;
                    flex-direction: row;
                    list-style-type: none;
                    padding: 0;
                    margin: 0;
                    li {
                        padding: 0.5rem;
                        border: 1px solid black;
                        border-radius: 2rem;
                        text-align: center;
                        &:hover {
                            background-color: #d8d8d8;
                            cursor: pointer;
                        }
                    }
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
            footer {
                @include center;
                border-radius: .5rem;
                align-self: stretch;
                flex-direction: row;
                align-items: center;
                background-color: #fdfdfd;
                input {
                    flex: 5;
                    padding: 0.5rem;
                    border: none;
                    border-radius: 0.5rem;
                    font-size: 24px;
                    font-family: 'Roboto';
                    background-color: #00a6fb;
                    &::placeholder {
                        color: #003554;
                    }
                }
            }
        }
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