import { expect, test, chromium } from '@playwright/test';
import { jwtDecode, type JwtPayload } from 'jwt-decode';
import isJWT from 'validator/lib/isJWT.js';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
let jwtCookie: any;

type TestJwtPayload = JwtPayload & {
	unique_name: string;
	role: string;
}


test.beforeAll('get jwt', async () => {
	const chrome = await chromium.launch({ headless: false, args:[
		'--disable-web-security',
		'--disable-features=IsolateOrigins,site-per-process',
		'--allow-running-insecure-content'
	] });
	const page = await chrome.newPage();
	page.context();

    await page.goto('/');
    
	const usernameInput = page.getByRole('textbox', { name: 'Username' });
	await usernameInput.fill("test");
	const passwordInput = page.getByRole('textbox', { name: 'Password' });
	await passwordInput.fill('test');
    const button = page.getByRole('button', { name: 'Submit' });
	
	const responseFromApi = page.waitForResponse(response => response.url().includes('/api/v1/jwt'), { timeout: 5000 });

    await button.click();

	await responseFromApi;

	const cookies = await page.context().cookies();
	jwtCookie = cookies.find(cookie => cookie.name === 'X-Access-Token');

	expect(jwtCookie).toBeDefined();
	expect(jwtCookie).toBeTruthy();
	expect(jwtCookie?.value).toBeTruthy();
});

test('jwt token is valid', async () => {
	expect(isJWT(jwtCookie.value)).toBeTruthy();
});

test('jwt token contains expected attributes', async () => {
	const decoded = jwtDecode(jwtCookie.value);
	expect(decoded).toBeDefined();
	expect(decoded).toBeTruthy();
	expect(decoded).toHaveProperty('unique_name');
	expect(decoded).toHaveProperty('role');
});

test('jwt token has expected credentials', async () => {
	const decoded: TestJwtPayload = jwtDecode(jwtCookie.value);
	expect(decoded.unique_name).toBe("test");
	expect(decoded.role).toBe("test");
});