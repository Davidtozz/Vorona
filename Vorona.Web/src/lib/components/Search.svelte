<script lang="ts">
    import Fa from 'svelte-fa';
    import {faSearch, faUserPlus} from '@fortawesome/free-solid-svg-icons';
    import { userConversationsStore } from '$lib/stores';
	import { derived } from 'svelte/store';

    export let query: string;
    let inputElement: HTMLInputElement;

    function addNewConversation() {
        userConversationsStore.update(conversations => [...conversations, {
            name: query,
            history: []
        }]);
     
        console.log("Add conversation");
    }

    //TODO: implement search
    function search() {
        console.log("Search");
    }


</script>

<div>
    <input bind:this={inputElement} placeholder="Search or start a new conversation..." bind:value={query}>
    <span>
        <!-- TODO: make it prettier  -->
        <button title="Search for a conversation" on:click={() => inputElement.focus()}>
            <Fa icon={faSearch} color="white" class="icon"/>
        </button>
        
        <button title="Add a new conversation" on:click={addNewConversation}>
            <Fa icon={faUserPlus} color="white" class="icon" />
        </button>
    </span>
</div>

<style lang="scss">
    @import "../../global.scss";

    div {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 0.8rem;
        border-radius: 0.5rem;
        background-color: $blue5;
        align-self: stretch;
        border: none;
        outline: none;
        transition: all 0.2s ease-in-out;

        &:hover {
            background-color: mix($blue4, transparent, 90%);
            
        }

        input {
            flex-grow: 9;
            border: none;
            outline: none;
            background-color: transparent;
            font-size: .9rem;
            font-weight: 500;
            color: whitesmoke;
            &:focus-visible {
                border-bottom: 2px solid $blue1;
                margin-bottom: -2px;
            }
            &::placeholder {
                color: $blue1;
            }
        }
        span {
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-grow: 1;
           .icon {
               z-index: 1;
            }

           button {
                background-color: transparent;
                border: none;
                outline: none;
                cursor: pointer;
                border-radius: 50%;
                transition: all 0.2s ease-in-out;
           }
        }

       
    } 
</style>