<script lang="ts">
    import { usernameStore } from "$lib/stores";
    import { goto } from "$app/navigation";
    import axios from "axios";

    let username: string;
    let password: string;

    async function register() {
        
        const response = await axios.post("http://localhost:5207/api/v1/jwt", {
            username: username,
            password: password
        }, {
            headers: {
                "Content-Type" : "application/json",
                "Access-Control-Allow-Origin" :"*"
            }
        }
        );
        
        
        /* const response = await fetch("http://localhost:5207/api/v1/jwt", {
            method: "POST",
            mode: "no-cors",
            headers: {"Content-Type" : "application/json"},
            body: JSON.stringify({
                username: username,
                password: password
            })
        }); */

        if(response.status === 200 && document.cookie === "") {
            console.log("Successfully registered!");
            $usernameStore = username;
            goto("/chat");
        } else {
            console.log("Something went wrong!");
        }
    }
</script>


<form on:submit={register}>
    <span>
        <p>Register</p>
    </span>
    <input type="text" id="username" placeholder="Username" bind:value={username}>
    <input type="text" id="password" placeholder="Password" bind:value={password}>
    <button type="submit">Submit</button>
</form>

<style>
 
    form {
        position: absolute;
        background-color: grey;
        padding: 1rem;
        border-radius: 0.5rem;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        display: flex;
        flex-direction: column;
        gap: 1rem;
    }

    span{
        padding-bottom: 3rem;
        display: flex;
        justify-content: center;
        font-family: 'Roboto';
    }

    button {
        background-color: black;
        font-size: 16px;
        color: white;
        padding: 0.5rem;
        border: none;
        border-radius: 0.5rem;
        font-family: 'Roboto';
    }

    input {
        padding: 0.5rem;
        border: none;
        border-radius: 0.5rem;
        font-size: 14px;
        font-family: 'Roboto';
    }

</style>