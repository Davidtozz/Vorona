import { expect, test, chromium } from '@playwright/test';
import { jwtDecode } from 'jwt-decode';
import isJWT from 'validator/lib/isJWT.js';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
let jwtCookie: any;

const TEST = {
	USERNAME: 'test',
	ROLE: 'User'
} as const;



test('jwt is set as cookie', async () => {
	const chrome = await chromium.launch({ headless: false, args:[
		'--disable-web-security',
		'--disable-features=IsolateOrigins,site-per-process',
		'--allow-running-insecure-content'
	] });
	const page = await chrome.newPage();
	page.context();

    await page.goto('/');

	//! Uncomment to log 
	/* page.on('requestfinished', request => console.log('PAGE LOG:', request.postDataJSON(), request.url(), request.method(), request.response(), request.allHeaders()));
	page.on('response', async response => {
		console.log(`Response: ${response.status()} ${response.url()}`);
		if (response.status() === 400) {
			const body = await response.text();
			console.log(`Response Body: ${body}`);
		}
	}); */
    
	const usernameInput = page.getByRole('textbox', { name: 'Username' });
	await usernameInput.fill(TEST.USERNAME);
	const passwordInput = page.getByRole('textbox', { name: 'Password' });
	await passwordInput.fill('test');
    const button = page.getByRole('button', { name: 'Submit' });
	
	//! Uncomment to log request 
	/* page.on('request', request => {
        console.log(`Request: 
		${request.method()} 
		${request.url()} 
		${JSON.stringify(request.postDataJSON())}`);
    }); */	
	
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