const { test, expect } = require('@playwright/test');

async function selectDropdownOption(page, containerId, optionText) {
  await page.locator(`#${containerId}`).click();
  await page.waitForSelector(`#${containerId} [class*='menu']`);
  await page.locator("div[id*='react-select'][id*='option']")
    .filter({ hasText: optionText })
    .first()
    .click();
}

async function signIn(page, username) {
  await selectDropdownOption(page, 'username', username);
  await selectDropdownOption(page, 'password', 'testingisfun99');
  await page.locator('#login-btn').click();
}

test.describe('Sign In Tests', () => {

  test('TC-166 - Successful sign in with valid credentials', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-166]]');
    await page.goto('/signin');

    await signIn(page, 'demouser');

    await expect(page).toHaveURL(/testathon\.live\/(\?|$)/);
    await expect(page.locator('span.username')).toHaveText('demouser');
  });

  test('TC-167 - Sign in redirects to checkout when coming from cart', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-167]]');
    await page.goto('/checkout');

    await expect(page).toHaveURL(/\/signin\?checkout=true/);

    await signIn(page, 'demouser');

    await expect(page).toHaveURL(/\/checkout/);
  });

  test('TC-168 - Sign in redirects to favourites when coming from favourites', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-168]]');
    await page.goto('/favourites');

    await expect(page).toHaveURL(/\/signin\?favourites=true/);

    await signIn(page, 'demouser');

    await expect(page).toHaveURL(/\/favourites/);
  });

  test('TC-169 - Sign in redirects to offers when coming from offers', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-169]]');
    await page.goto('/offers');

    await expect(page).toHaveURL(/\/signin\?offers=true/);

    await signIn(page, 'demouser');

    await expect(page).toHaveURL(/\/offers/);
  });

  test('TC-170 - Locked user cannot sign in', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-170]]');
    await page.goto('/signin');

    await signIn(page, 'locked_user');

    await expect(page.locator('h3.api-error')).toBeVisible();
    await expect(page).toHaveURL(/.*signin/);
  });

  test('TC-171 - Username dropdown displays all expected options', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-171]]');
    await page.goto('/signin');

    await page.locator('#username').click();

    const expectedUsers = [
      'demouser',
      'image_not_loading_user',
      'existing_orders_user',
      'fav_user',
      'locked_user',
    ];

    const options = page.locator("div[id*='react-select'][id*='option']");

    for (const user of expectedUsers) {
      await expect(options.filter({ hasText: user }).first()).toBeVisible();
    }

    await expect(options).toHaveCount(5);
  });

  test('TC-172 - Username field does not allow arbitrary text entry', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-172]]');
    await page.goto('/signin');

    await page.locator('#username').click();
    await page.locator('#react-select-2-input').fill('fakeuser123');

    await expect(
      page.locator("div[id*='react-select'][id*='option']").filter({ hasText: 'fakeuser123' })
    ).toHaveCount(0);

    await page.keyboard.press('Escape');
    await page.locator('#login-btn').click();

    await expect(page.locator('#login-btn')).toBeVisible();
  });

  test('TC-173 - Log In button with no credentials selected shows error', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-173]]');
    await page.goto('/signin');

    await page.locator('#login-btn').click();

    await expect(page.locator('h3.api-error')).toBeVisible();
    await expect(page).toHaveURL(/.*signin/);
  });

  test('TC-174 - Sign in with username selected but no password shows error', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-174]]');
    await page.goto('/signin');

    await selectDropdownOption(page, 'username', 'demouser');
    await page.locator('#login-btn').click();

    await expect(page.locator('h3.api-error')).toBeVisible();
    await expect(page).toHaveURL(/.*signin/);
  });

  test('TC-175 - Sign in page is accessible without authentication and redirects when already logged in', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-175]]');
    // Not logged in: /signin loads normally
    await page.goto('/signin');
    await expect(page).toHaveURL(/.*signin/);
    await expect(page.locator('#login-btn')).toBeVisible();

    // Sign in
    await signIn(page, 'demouser');
    await expect(page).toHaveURL(/testathon\.live\/(\?|$)/);

    // Already logged in: revisiting /signin redirects to home
    await page.goto('/signin');
    await expect(page).toHaveURL(/testathon\.live\/(\?|$)/);
  });

});
