<script lang="ts">
    export let content: string;
    //TODO: add sender to message bubble
    export let sender: string;
</script>

<div>
   {sender}: {content}
</div>

<style>
    /* TODO define global font styling */
    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap');

    div {
        display: block;
        flex: 1;
        padding: .2rem;
        border: 1px solid black;
        text-align: left;
        font-family: 'Roboto', sans-serif;
    }
</style>