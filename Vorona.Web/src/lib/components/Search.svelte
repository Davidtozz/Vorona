<script lang="ts">
    import Fa from 'svelte-fa';
    import {faSearch, faUserPlus} from '@fortawesome/free-solid-svg-icons';
    import { userConversationsStore } from '$lib/stores';
	import { derived } from 'svelte/store';

    let query: string;



    function addNewConversation() {
        userConversationsStore.update(conversations => [...conversations, {
            name: query,
            history: []
        }]);
     
        console.log("Add conversation");
    }

    //TODO: implement search
    /**
     * ! WIP
     */
    function search() {
        console.log("Search");
    }

    /**
     * ! WIP
     */
    const filteredConversationsStore = derived(
        userConversationsStore,
        ($userConversationsStore) => $userConversationsStore.filter(conversation => conversation.name.includes(query))
    );

</script>

<div>
    <input placeholder="Search or start a new conversation..." bind:value={query}>
    <span>
        <!-- TODO: make it prettier  -->
        <button on:click={search}><Fa icon={faSearch} class="icon"/></button>
        <button on:click={addNewConversation}><Fa icon={faUserPlus} class="icon" /></button>
    </span>
</div>

<style lang="scss">
    div {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0.8rem;
        border-radius: 0.5rem;
        background-color: #f5f5f5;
        /* color: grey; */
        /* font-size: 1rem;
        font-weight: 500; */
        align-self: stretch;

        border: none;
        outline: none;
        transition: all 0.2s ease-in-out;

        &:hover {
            background-color: #e5e5e5;
        }

        input {
            flex-grow: 9;
            border: none;
            outline: none;
            background-color: transparent;
            font-size: .9rem;
            font-weight: 500;
            /* color: #a0a0a0; */
            
        }
        span {
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-grow: 1;
           &.icon {
               z-index: 1;
           }

           button {
                background-color: transparent;
                border: none;
                outline: none;
                cursor: pointer;
                border-radius: 50%;
                transition: all 0.2s ease-in-out;
    
                &:hover {
                     background-color: #e5e5e5;
                }
           }
        }

       
    } 
</style>