<script lang="ts">
    export let content: string;
    //TODO: add sender to message bubble
    export let sender: string;
    export let isSentFromMe: boolean;
</script>

{#if isSentFromMe}
    <div class="messageBubbleWrapper reversed">
        <div class="triangle reversed" />
        <div class="messageBubble reversed">
            <p class="senderName"> {sender} </p>
            <p> {content} </p>
        </div>
    </div> 
{:else}
    <div class="messageBubbleWrapper">
        <div class="triangle"></div>
        <div class="messageBubble">
            <p class="senderName"> {sender} </p>
            <p> {content} </p>
        </div>
</div>
{/if}

<style lang="scss">
    /* TODO define global font styling */
    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap');

    .messageBubbleWrapper {
        display: flex;
        align-items: flex-start;
        justify-content: flex-start;
        height: fit-content;

        &.reversed {
            flex-direction: row-reverse;
        }

        .triangle {
            width: 0;
            height: 0;
            border-top: 15px solid transparent;
            border-bottom: 15px solid transparent;
            position: relative;
            top: 0.6rem;
            border-left: 20px solid white;
            rotate: 180deg;

            &.reversed {
            width: 0;
            height: 0;
            border-top: 15px solid transparent;
            border-bottom: 15px solid transparent;
            position: relative;
            top: 0.6rem;
            border-left: 20px solid #BB98FF;
            rotate: 0deg;
            
            }
        }

        .messageBubble {
            width: fit-content;
            display: flex;
            flex-direction: column;
            justify-content: space-evenly;
            max-width: 15rem;
            padding: 0 0.5rem;
            min-height: 50px;
            padding: .5rem;
            gap: 0.5rem;
            height: fit-content;
            background-color: white;
            border-radius: 10px;
            /* outline: 1px solid red; */ /* TODO add a slight outline to bubble and triangle */
            font-family: "Roboto", sans-serif;

            .senderName {
                font-weight: 500;
                font-size: 0.8rem;
                margin: 0;
            }

            p {
                margin: 0;
            }

            &.reversed {
                background-color: #BB98FF;
                color: white;
            }
        }
    }
</style>