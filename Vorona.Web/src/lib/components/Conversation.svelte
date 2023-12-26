<script lang="ts">
	import { faGlobe } from "@fortawesome/free-solid-svg-icons";
	import Fa from "svelte-fa";
    import { userConversationsStore } from "$lib/stores";

    export let name: string;
    export let lastMessage: string = "";

    function removeSelf(): void {
        if(name === "Lobby") {
            alert("You can't remove the lobby!")
            return;
        }
        console.log(`Removing ${name} from conversations.`);
        userConversationsStore.update(conversations => conversations.filter(c => c.name !== name));
        
        for(const c of $userConversationsStore) {
            console.table(c);
        }
    }
    
    function selectConversation(): void {

        userConversationsStore.set($userConversationsStore.map(c => {
            if(c.name !== name) {
                c.isSelected = false;
            } else {
                c.isSelected = !c.isSelected;
                console.log(`Selected ${name}:\n Value is now:${c.isSelected}`)
            }
            return c;
        }));
    }

</script>


<div class="contactCard" 
    class:selected={$userConversationsStore.find(c => c.name === name)?.isSelected}
    on:click={selectConversation} 
    on:contextmenu|preventDefault={removeSelf} role={"button"}>
    <div class="propicWrapper">
    <!-- ? DEFAULT ICON -->
        {#if name === "Lobby"}
        <Fa icon={faGlobe} class="propic" style="height: unset;"/>
        {:else}
        <img src={`https://source.unsplash.com/random/500x500?sig=${Math.random()}`} class="propic" alt="" srcset="">
        <!-- <Fa icon={faCircleUser} class="propic" style="height: unset;"/> -->
        {/if}
    </div>
    
    <div class="contactDetails">
        <h2>{name}</h2>                  
        <h4>{lastMessage}</h4>    
    </div>
    <div class="messageInfo">
        <p>6:50</p> 
    </div>

</div>
<!-- TODO create dialog at cursor when right clicking Conversation cards
<svelte:component this={} x={X} y={Y}/> -->


<style lang="scss">
    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap');
    @import "../../global.scss";

    $purple: #8C52FF;
    $lightPurple: #BB98FF;

    
    .contactCard {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 1rem;
        gap:.3rem;
        height: fit-content;
        
        background-color: transparent;
        /* border-radius: 5px; */
        
        &:hover {
            /* background-color: #d8d8d8; */
            cursor: pointer;
        }

        &:not(.selected):hover {
            background-color: mix(transparent, $blue4, 50%);
        }

        &.selected {
        background-color: mix(transparent, $blue4, 40%);
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
            border-radius: 50%;
            .propic {
                border-radius: 50%;
                object-fit: cover; 
                cursor: pointer;
                path {
                    fill: grey;
                }
            }     
        }
       
    }
    
    .messageInfo {
        font-family: 'Roboto', sans-serif;
    }
    
</style>