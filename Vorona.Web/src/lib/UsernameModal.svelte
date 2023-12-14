<script lang="ts">
import {usernameStore} from '$lib/index'
import {page} from '$app/stores'
import { goto } from '$app/navigation';

let inputFieldValue = ""

function setUsername() {
    usernameStore.set(inputFieldValue)
    const url = new URL(window.location.href)
    url.searchParams.set('username', inputFieldValue)
    goto(url.toString())
}

</script>



<dialog open>

    <form method="dialog" on:submit|preventDefault={setUsername}>
        <label for="nickname">Nickname</label>
        <input type="text" name="nickname" id="nickname" placeholder="Nickname" bind:value={inputFieldValue} on:keydown={(e) => {e.key === 'Enter' && setUsername()}}>
        <button type="submit">Submit</button>
    </form>

</dialog>


<style>

    dialog {
        position: fixed;
        top: 0;
        left: 0;
        width: 100vw;
        height: 100vh;
        background-color: rgba(0, 0, 0, 0.277);
        display: flex;
        justify-content: center;
        align-items: center;
    }

    form {
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        background-color: #fdfdfd;
        padding: 2rem;
        gap: 1rem;
        border-radius: 1rem;

        & button {
            padding: 0.5rem;
            border-radius: 0.5rem;
            border: none;
            background-color: #f0f0f0;
            font-size: 14px;
            font-family: 'Roboto';
        }
    }

</style>