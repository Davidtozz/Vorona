<script lang="ts">
    import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";


    const connection = new HubConnectionBuilder()
        .withUrl("https://localhost:5207/chat")
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Debug)
        .build();
</script>

<div class="chatbox">
    
    <input type="text" placeholder="Type your message here..." />
    <button>Send</button>

</div>

<style>
    @import url('https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap');

    .chatbox {
        display: flex;
        justify-content: center;
        align-items: flex-end;
        height: 100dvh;

        background-color: grey;
    }


    .chatFooter {
      display: flex;
      justify-content: center;
      align-items: center;
      flex-direction: row;
      align-items: center;
      background-color: #fdfdfd;

    }

    input {
        flex: 5;
        padding: 0.5rem;
        border: none;
        border-radius: 0.5rem;
        font-size: 24px;
        font-family: 'Roboto';
      }

    button {
        flex: 1;
        background-color: brown;
        font-size: 24px;
        padding: 0.5rem;
        border: none;
        border-radius: 0.5rem;
        font-family: 'Roboto';
      
    }
</style>