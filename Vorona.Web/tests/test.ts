import { expect, test } from '@playwright/test';
import { jwtDecode } from 'jwt-decode';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
let jwtCookie: any;

const TEST = {
	USERNAME: 'TestUser',
	ROLE: 'Admin'
} as const;

test.beforeEach(async ({ page }) => {
    await page.goto('/chat');
    
	//get rid of UsernameModal
	const usernameInput = await page.getByRole('textbox', { name: 'Nickname' });
    await usernameInput.fill('test');
    const usernameButton = await page.getByRole('button', { name: 'Submit' });
    await usernameButton.click();
    
    const button = page.getByRole('button', { name: 'GET JWT' });
    await button.click();

    await page.waitForResponse('http://localhost:5207/api/v1/jwt');
    const cookies = await page.context().cookies();
    jwtCookie = cookies.find(cookie => cookie.name === 'X-Access-Token');
});

test('jwt token is set', async () => {
    expect(jwtCookie).toBeDefined();
    expect(jwtCookie).toBeTruthy();
    expect(jwtCookie?.value).toBeTruthy();
});

test('jwt token contains "name" and "role" attributes', async () => {
	const decoded = jwtDecode(jwtCookie.value);
	expect(decoded).toBeDefined();
	expect(decoded).toBeTruthy();
	expect(decoded).toHaveProperty('unique_name', TEST.USERNAME);
	expect(decoded).toHaveProperty('role', TEST.ROLE);
});




//TODO:  message is properly rendered on screen
/* test('message is rendered', async ({ page }) => {
	await page.goto('/chat')
	const inputField = await page.getByRole('textbox', { name: 'Message' });
	await inputField.fill('Hello World!');
	await page.keyboard.press('Enter');
	expect(await page.getByText('Hello World!')).toBeVisible();
}); */