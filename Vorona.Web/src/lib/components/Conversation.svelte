<script lang="ts">
	import { faCircleUser } from "@fortawesome/free-solid-svg-icons";
	import Fa from "svelte-fa";

    export let name: string;
    export let lastMessage: string = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod.";

</script>


<div class="contactCard">
            
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
        
        /* border-radius: 5px; */
        background-color: #fff;
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
</style>