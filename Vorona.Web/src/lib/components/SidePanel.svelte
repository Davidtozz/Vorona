<script lang="ts">
	import { connectedUsersStore, userConversationsStore, usernameStore } from "$lib/stores";
	import Search from "./Search.svelte";
    import Conversation from "./Conversation.svelte";
	import Fa from "svelte-fa";
	import { faUser } from "@fortawesome/free-solid-svg-icons";
	import { derived, get, writable, type Readable } from "svelte/store";

    let searchQuery = writable('');

    /**
     * This derived store filters user conversations based on a search query.
     * @param {Readable<Array>} userConversationsStore - The store containing user conversations.
     * @param {Readable<string>} searchQuery - The store containing the search query.
     * @returns {Derived<Array>} - The derived store containing filtered conversations.
     */
    const searchResultsStore = derived(
        [userConversationsStore, searchQuery],
        ([$userConversationsStore, $searchQuery]) => 
            $userConversationsStore.filter(conversation => 
                conversation.name.toLowerCase().includes($searchQuery.toLowerCase())
            )
    );
    
</script>

<div class="sidePanel">
    <div class="search-wrapper">
        <Search bind:query={$searchQuery} />
    </div>
    
    <div>
        {#if $searchQuery !== ""}
            {#each $searchResultsStore  as conversation}
                <Conversation name={conversation.name} lastMessage={conversation.lastMessage}/>
            {/each}
        {:else}
            {#each $userConversationsStore as conversation}
                <Conversation name={conversation.name} lastMessage={conversation.lastMessage}/>
            {/each}
        {/if}
    </div>

    <section>
        <Fa icon={faUser} class="icon" size={"2x"}/>
        <p>Logged in as: {$usernameStore}</p>
    </section>
</div>

<style lang="scss">
    @import "../../global.scss";

    .sidePanel {
        flex: 3;
        display: flex;
        
        flex-direction: column;
        background-color: $blue3;
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

        section {
            display: flex;
            padding: 1rem;
            background-color: #003554;
            justify-content: space-between;
            align-items: center;
            border-radius: .5rem;
            p {
                margin: 0;
                font-size: 16px;
                font-family: 'Roboto';
                border-radius: .5rem;
                color: mix(white, transparent, 90%);
                padding: .5rem;
                text-align: center;
                width: fit-content;
                background-color: transparent;
            }
        }
    }
</style>