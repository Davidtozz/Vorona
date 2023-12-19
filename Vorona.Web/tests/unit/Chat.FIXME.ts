import { describe, it, expect, beforeAll } from "vitest";
import { render, fireEvent } from '@testing-library/svelte';
import Chat from "../../src/routes/chat/+page.svelte";
import sinon from "sinon";
import 'vitest-dom/extend-expect'
import { HttpTransportType, HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

/**
 * @vitest-environment jsdom
 */

const mockConnection: HubConnection =  new HubConnectionBuilder()
.withUrl('http://localhost:5207/chat', {
    skipNegotiation: true,
    transport: HttpTransportType.WebSockets
})
.build();

describe('Chat.svelte', () => {

    beforeAll(() => {
        sinon.stub(mockConnection, 'start').resolves();
        sinon.stub(mockConnection, 'invoke').resolves();
        //console.log(mockConnection);
    })

    it.fails('(fixme) DOES NOT send a message', async () => {
        const {  getAllByText, getByText, getByPlaceholderText } = render(Chat, { currentMessage: 'test', connection: mockConnection});
        
        const button = getAllByText('Send')[0];
        const input = getByPlaceholderText('Type your message here...');
        
        fireEvent.input(input, { target: { value: 'test' } });
        await fireEvent.click(button);
        const message = getByText('test');        
        expect(message).toBeInTheDocument();
    })

    /* it("doesn't submit an empty message", () => {
        const { getByText } = render(Chat, { currentMessage: 'test', connection: mockConnection});
        const button = getByText('Send');
        fireEvent.click(button);

        expect(mockConnection.invoke).not.toHaveBeenCalled();
    }) */
});