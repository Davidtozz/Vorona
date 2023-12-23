<script lang="ts">
    import Fa from 'svelte-fa';
    import {faCaretDown, faSearch} from '@fortawesome/free-solid-svg-icons';

    let query: string;

</script>

<div>
    <input placeholder="Search or start a new conversation..." bind:value={query}>
    <Fa icon={faSearch} class="icon"/>
</div>


<style lang="scss">
    div {
        display: flex;
        align-items: center;
        padding: 0.8rem;
        border-radius: 0.5rem;
        background-color: #f5f5f5;
        /* color: grey; */
        /* font-size: 1rem;
        font-weight: 500; */
        width: 100%;
        box-sizing: border-box;
        border: none;
        outline: none;
        transition: all 0.2s ease-in-out;

        &:hover {
            background-color: #e5e5e5;
        }

        input {
            flex: 1;
            border: none;
            outline: none;
            background-color: transparent;
            font-size: .9rem;
            font-weight: 500;
            /* color: #a0a0a0; */
            margin-left: 0.5rem;
        }
    } 
</style>