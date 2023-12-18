<script lang="ts">
    import { usernameStore } from "$lib/stores";
    import { goto } from "$app/navigation";
    import axios from "axios";

    let username: string;
    let password: string;


    export function validateForm(): boolean {

    if(username === undefined || password === undefined) return false;
        if(username === null || password === null) return false;
        if(username === "" || password === "") return false;
        


        return username.length > 0 && password.length > 0;
    }



    export async function register(): Promise<boolean> {
        const response = await axios.post("http://localhost:5207/api/v1/jwt", {
            username: username,
            password: password
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
            //! goto("/chat");
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
    <input type="text" id="username" placeholder="Username" bind:value={username}>
    <input type="text" id="password" placeholder="Password" bind:value={password}>
    <button type="submit">Submit</button>
    <a href="/chat" on:click>Login as guest</a>
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