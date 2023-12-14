import { expect, test } from '@playwright/test';

test('index page has expected h1', async ({ page }) => {
	await page.goto('/');
	await expect(page.getByRole('heading', { name: 'Welcome to SvelteKit' })).toBeVisible();
});

/* test('message is rendered', async ({ page }) => {
	await page.goto('/chat')
	const inputField = await page.getByRole('textbox', { name: 'Message' });
	await inputField.fill('Hello World!');
	await page.keyboard.press('Enter');
	expect(await page.getByText('Hello World!')).toBeVisible();
}); */