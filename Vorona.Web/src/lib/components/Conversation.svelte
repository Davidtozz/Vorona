<script lang="ts">
	import { faCircleUser } from "@fortawesome/free-solid-svg-icons";
	import Fa from "svelte-fa";
    import { userConversationsStore } from "$lib/stores";

    export let name: string;
    export let lastMessage: string = "";
    export let isSelected: boolean = false;

    function removeSelf(): void {
        console.log(`Removing ${name} from conversations.`);
        userConversationsStore.update(conversations => conversations.filter(c => c.name !== name));
        
        for(const c of $userConversationsStore) {
            console.table(c);
        }
    }

    /**
     * ! Debugging function
     */
    function handleClick(): void {
        isSelected = !isSelected;
        console.log(`Selected ${name}:\n Value is now:${isSelected}`)        
        /* selectConversation(name); */
    }

</script>


<div class="contactCard" 
    class:selected={isSelected}
    on:click={handleClick} 
    on:contextmenu|preventDefault={removeSelf} role={"button"}>
    <div class="propicWrapper">
    <!-- ? DEFAULT ICON -->
        <Fa icon={faCircleUser} class="propic" style="height: unset;"/>

    </div>
    
    <div class="contactDetails">
        <h2>{name}</h2>                  
        <h4>{lastMessage}</h4>    
    </div>
    <div class="messageInfo">
        <p>6:50</p> 
    </div>

</div>


<style lang="scss">
    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap');

    $purple: #8C52FF;
    $lightPurple: #BB98FF;

    
    .contactCard {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 1rem;
        gap:.3rem;
        height: fit-content;
        
        background-color: #fff;
        /* border-radius: 5px; */
        
        &:hover {
            /* background-color: #d8d8d8; */
            cursor: pointer;
        }
        

        text-indent: 15px;  
        .contactDetails {
        height: 100%;
        justify-content: space-evenly;
        display: flex;
        flex: 6;
        flex-direction: column;
    
        h2 {
            margin: 0;
            font-family: 'Roboto', sans-serif;
            font-size: 16px;
            font-weight: 300;
        }
    
        h4 {
            margin: 0;
            font-family: 'Roboto', sans-serif;
            font-size: 14px;
            color: rgb(30, 28, 28);
            font-weight: 400;
            font-style: italic;
        }
        }
        .propicWrapper {
        display: inline-flex;
        justify-content: center;
        height: 50px;
        width: 50px;
        outline: 1px solid $purple;
        border-radius: 50%;
        .propic {
            border-radius: 50%;
            object-fit: cover; 
            cursor: pointer;
            font-size: 5rem;
            height: 50px;
            width: 50px;
            path {
                fill: grey;
            }
        }     
    }
        .messageInfo {
            font-family: 'Roboto', sans-serif;
        }
    }

    .selected {
        background-color: #b62020;
    }
</style>