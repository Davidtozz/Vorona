import { expect, test, chromium, type BrowserContext } from '@playwright/test';
import { jwtDecode } from 'jwt-decode';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
let jwtCookie: any;
let pageContext: BrowserContext;

const TEST = {
	USERNAME: 'test',
	ROLE: 'Admin'
} as const;

test.beforeAll(async () => {
    /* await page.goto('/chat'); */
	const chrome = await chromium.launch({ devtools: true, headless: false, args:[
		'--disable-web-security',
		'--disable-features=IsolateOrigins,site-per-process',
		'--allow-running-insecure-content'
	] });
	const page = await chrome.newPage();
	pageContext = page.context();

    await page.goto('/');
    
	const usernameInput = page.getByRole('textbox', { name: 'Username' });
	await usernameInput.fill(TEST.USERNAME);
	const passwordInput = page.getByRole('textbox', { name: 'Password' });
	await passwordInput.fill('test');
    const button = page.getByRole('button', { name: 'Submit' });
    await button.click();

    await page.waitForResponse('http://localhost:5207/api/v1/jwt');
    
});


//TODO: fix test
test('jwt token is set', async () => {

	const cookies = await pageContext.cookies();
	jwtCookie = cookies.find(cookie => cookie.name === 'X-Access-Token');

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