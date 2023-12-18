import { describe, it, expect } from 'vitest';
import '@testing-library/svelte';
import { render } from "@testing-library/svelte";
import Register from "../src/lib/auth/Register.svelte";
import 'vitest-dom/extend-expect'


/**
 * @vitest-environment jsdom
 */

describe('Register.svelte', () => {

    it('Renders correctly', () => {
        const { container } = render(Register);
        expect(container).toBeInTheDocument();
    })

    it.fails('(fixme) registers the user', () => {
        const registerComponent = render(Register);
        expect(registerComponent.component.validateForm()).toBeTruthy();
    })

    it('fails to register with empty username', () => {
        const registerComponent = render(Register);
        registerComponent.component.username = '';
        expect(registerComponent.component.validateForm()).toBeFalsy();
    })

    it('fails to register with empty password', () => {
        const registerComponent = render(Register);
        registerComponent.component.password = '';
        expect(registerComponent.component.validateForm()).toBeFalsy();
    })

    it('fails to register with password shorter than 8 characters', () => {
        const registerComponent = render(Register);
        registerComponent.component.password = '1234567';
        expect(registerComponent.component.validateForm()).toBeFalsy();
    })

});

