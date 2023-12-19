<script lang="ts">
    import { usernameStore } from "$lib/stores";
    import { goto } from "$app/navigation";
    import axios from "axios";

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

    function loginAsGuest(): void {
        $usernameStore = "Guest" + Math.floor(Math.random() * 10000);
        goto("/chat?username=" + $usernameStore);
    }

    export async function register(): Promise<boolean> {
        if(!validateForm()) {
            return false;
        }
        const response = await axios.post("http://localhost:5207/api/v1/jwt", {
            username: username,
            password: password,
            role: "test" //! DEBUG
        }, {
            headers: {
                "Content-Type" : "application/json",
                "Access-Control-Allow-Origin" :"*",
            },
        }
        );
        if(response.status === 200) {
            console.log("Successfully registered!");
            $usernameStore = username;
            goto("/chat?username=" + $usernameStore);
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
    
    <input type="text" id="username" placeholder="Username" bind:value={username} required>
    
    <input type="text" id="password" placeholder="Password" bind:value={password} required>
    <button type="submit">Submit</button>
    <a href="#top" on:click|preventDefault={loginAsGuest}>Login as guest</a>
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