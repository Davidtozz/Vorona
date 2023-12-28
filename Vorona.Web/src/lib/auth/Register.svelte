<script lang="ts">
    import { usernameStore } from "$lib/stores";
    import { goto } from "$app/navigation";
    import validator from "validator";

    export let username: string = '';
    export let password: string = '';


    export function validateForm(): boolean {
    let isValid = true;

    if (!username || username.trim().length === 0) {
      isValid = false;
    }

    if (!password || password.trim().length === 0) {
      isValid = false;
    }

    return isValid;
  }

    async function register(): Promise<boolean> {
        if(!validateForm()) {
            return false;
        }

        const response = await fetch("http://localhost:5000/api/v1/auth/jwt", {
            method: "POST",
            headers: {
                "Content-Type" : "application/json"
            },
            body: JSON.stringify({
                username: username,
                password: password,
            }),
            credentials: "include", //Allows token in 'Set-Cookie' header to be set
        });

        if(response.status === 200) {
            console.log("Successfully registered!");
            $usernameStore = username;
            goto("/chat");
            return true;
        } else {
            console.log("Something went wrong!");
            return false;
        }
    }
</script>

<form on:submit={() => register()}>
    <span>
        <p>Register</p>
    </span>
    
    <input minlength="1" type="text" id="username" placeholder="Username" bind:value={username} required>
    
    <input minlength="1" type="password" id="password" placeholder="Password" bind:value={password} required>
    <button type="submit">Submit</button>
</form>

<style lang="scss">

    @mixin centerOnScreen() {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
    }
 
    form {
        @include centerOnScreen();

        background-color: grey;
        padding: 1rem;
        border-radius: 0.5rem;
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 1rem;
    
        span{
            padding-bottom: 3rem;
            display: flex;
            justify-content: center;
            font-family: 'Roboto';
        }
        
        button {
            align-self: stretch;
            display: block;
            cursor: pointer;
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
    }
</style>