<script lang="ts">
    export let content: string;
</script>

<div>
    {content}
</div>

<style>
    /* TODO define global font styling */
    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap');

    div {
        display: block;
        text-align: center;
        font-family: 'Roboto', sans-serif;
    }
</style>