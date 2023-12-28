<script>
    import Message from "$components/Message.svelte";
</script>

<div>
    <h1>DEV ONLY</h1>
    <span>
        <Message 
            sender={"TEST"} 
            content={"Lorem ipsum dolor sit amet, consectetur adipiscing elit."} 
            isSentFromMe={true}/>
        
        <Message 
            sender={"TEST"} 
            content={"Lorem ipsum dolor sit amet, consectetur adipiscing elit."} 
            isSentFromMe={false}/>
        
        <Message 
            sender={"TEST"} 
            content={"Lorem ipsum dolor sit amet, consectetur adipiscing elit."} 
            isSentFromMe={true}/>
    
    </span>
</div>

<style lang="scss">

//center the message bubble
div {
    position: absolute;
    display: flex;
    align-items: center;
    flex-direction: column;
    justify-content: space-evenly;
    height: 100vh;
    background-color: black;
    width: 100vw;
}

h1 {
    color: white;
}
</style>