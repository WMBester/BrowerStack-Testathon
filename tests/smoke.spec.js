const { test, expect } = require('@playwright/test');

test.describe('Smoke Tests', () => {
  test('TC-99 - Homepage loads successfully', async ({ page }) => {
    await page.goto('https://testathon.live');

    // Verify page has a title
    await expect(page).toHaveTitle(/.+/);

    // Verify the body is visible
    await expect(page.locator('body')).toBeVisible();

    console.log('[[PROPERTY|id=TC-99]]');
  });
});
