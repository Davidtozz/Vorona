import {messageHistoryStore} from "$lib/stores";

export function onReceiveMessage(user: string, message: string) {
    const timestamp = new Date();

    messageHistoryStore.update(history => [...history, {
        sender: user, 
        content: message,
        timestamp: `${timestamp.getHours()}:${timestamp.getMinutes()}`
    }]);
}

export function onReceivePrivateMessage(fromUser: string, message: string) {
    const timestamp = new Date();

    console.log("Private message received from " + fromUser + ": " + message + "")

    messageHistoryStore.update(history => [...history, {
        sender: fromUser, 
        content: message,
        timestamp: `${timestamp.getHours()}:${timestamp.getMinutes()}`
    }]);

}

export function onConnectionEstablished(generatedGuestName: string, connectedGuests: object)  {
    console.log("Connection started! Connected as: " + generatedGuestName);
    console.table(connectedGuests)
}

export function onGetUsers(users: object) {
    console.table(users)
}