import { describe, it, expect } from 'vitest';
import '@testing-library/svelte';
import { render } from "@testing-library/svelte";
import Register from "../../src/lib/auth/Register.svelte";
import 'vitest-dom/extend-expect'


/**
 * @vitest-environment jsdom
 */

describe('Register.svelte', () => {
    it('Renders correctly', () => {
        const { container } = render(Register);

        expect(container).toBeInTheDocument();
    })

    it('Validates correctly', () => {
        const { component } = render(Register, { username: 'test', password: 'test' });
        expect(component.validateForm()).toBeTruthy();
    })

    it('Fails to register with empty username and password', () => {
        const { component } = render(Register, { username: '', password: '' });
        expect(component.validateForm()).toBeFalsy();
    })

    it('Fails to register with either empty username or password', () => {
        const { component } = render(Register, { username: 'NOT_EMPTY', password: '' });
        expect(component.validateForm()).toBeFalsy();
    })


});