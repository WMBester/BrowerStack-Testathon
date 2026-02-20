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
  await page.goto('/signin');
  await selectDropdownOption(page, 'username', username);
  await selectDropdownOption(page, 'password', 'testingisfun99');
  await page.locator('#login-btn').click();
  await expect(page).toHaveURL(/testathon\.live\/(\?|$)/);
}

async function addToCart(page, productIndex = 0) {
  await page.waitForSelector('.shelf-item__buy-btn');
  await page.locator('.shelf-item__buy-btn').nth(productIndex).click();
  await page.waitForTimeout(600);
}

async function completeCheckout(page) {
  await addToCart(page, 0);
  await page.goto('/checkout');
  await page.waitForLoadState('networkidle');
  await page.locator('#firstNameInput').fill('John');
  await page.locator('#lastNameInput').fill('Doe');
  await page.locator('#addressLine1Input').fill('123 Main Street');
  await page.locator('#provinceInput').fill('California');
  await page.locator('#postCodeInput').fill('90001');
  await page.locator('#checkout-shipping-continue').click();
  await expect(page).toHaveURL(/\/confirmation/, { timeout: 15000 });
}

test.describe('Navigation Tests', () => {

  test('TC-223 - Header navigation links are accessible when signed in', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-223]]');
    await signIn(page, 'demouser');

    await expect(page.locator('a#offers')).toBeVisible();
    await expect(page.locator('a#orders')).toBeVisible();
    await expect(page.locator('a#favourites')).toBeVisible();

    await page.locator('a#offers').click();
    await expect(page).toHaveURL(/\/offers/);

    await page.locator('a#orders').click();
    await expect(page).toHaveURL(/\/orders/);

    await page.locator('a#favourites').click();
    await expect(page).toHaveURL(/\/favourites/);
  });

  test('TC-224 - Sign out clears session and redirects to sign in', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-224]]');
    await signIn(page, 'demouser');

    await page.locator('a#logout').click();
    await page.waitForLoadState('networkidle');

    await expect(page).toHaveURL(/\/signin/);

    await page.goto('/orders');
    await expect(page).toHaveURL(/\/signin/);

    await page.goto('/checkout');
    await expect(page).toHaveURL(/\/signin/);
  });

  test('TC-225 - Cart count in header reflects items added and resets after checkout', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-225]]');
    await signIn(page, 'demouser');
    await page.waitForSelector('.shelf-item__buy-btn');

    const countBefore = await page.locator('.bag__quantity').first().innerText();
    expect(countBefore).toBe('0');

    await addToCart(page, 0);
    const countAfterOne = await page.locator('.bag__quantity').first().innerText();
    expect(countAfterOne).toBe('1');

    await addToCart(page, 1);
    const countAfterTwo = await page.locator('.bag__quantity').first().innerText();
    expect(countAfterTwo).toBe('2');

    await completeCheckout(page);
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    const countAfterCheckout = await page.locator('.bag__quantity').first().innerText();
    expect(countAfterCheckout).toBe('0');
  });

  test('TC-226 - StackDemo logo navigates to home page', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-226]]');
    await signIn(page, 'demouser');

    await page.goto('/orders');
    await expect(page).toHaveURL(/\/orders/);

    await page.locator('a.Navbar_logo__26S5Y').click();

    await expect(page).toHaveURL(/testathon\.live\/(\?|$)/);
  });

  test('TC-227 - Footer is present on all pages', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-227]]');
    await signIn(page, 'demouser');

    await page.goto('/');
    await page.waitForLoadState('networkidle');
    await expect(page.locator('footer').first()).toBeVisible();

    await page.goto('/orders');
    await page.waitForLoadState('networkidle');
    await expect(page.locator('footer').first()).toBeVisible();

    // Checkout page — need an item in cart
    await page.goto('/');
    await page.waitForSelector('.shelf-item__buy-btn');
    await page.locator('.shelf-item__buy-btn').first().click();
    await page.waitForTimeout(600);
    await page.goto('/checkout');
    await page.waitForLoadState('networkidle');
    await expect(page.locator('footer').first()).toBeVisible();

    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');
    await expect(page.locator('footer').first()).toBeVisible();
  });

});
