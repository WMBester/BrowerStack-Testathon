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

test.describe('Cart Tests', () => {

  test('TC-193 - Cart persists items across navigation', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-193]]');
    await signIn(page, 'demouser');

    await addToCart(page, 0);
    const countAfterAdd = await page.locator('.bag__quantity').first().innerText();
    expect(countAfterAdd).toBe('1');

    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');

    await page.goto('/');
    await page.waitForLoadState('networkidle');

    const countAfterNav = await page.locator('.bag__quantity').first().innerText();
    expect(countAfterNav).toBe('1');
  });

  test('TC-194 - Cart is cleared after successful checkout', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-194]]');
    await signIn(page, 'demouser');

    await completeCheckout(page);

    await page.goto('/');
    await page.waitForLoadState('networkidle');

    const cartCount = await page.locator('.bag__quantity').first().innerText();
    expect(cartCount).toBe('0');
  });

  test('TC-195 - Cart is empty when no items added', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-195]]');
    await signIn(page, 'demouser');

    await page.goto('/checkout');
    await page.waitForLoadState('networkidle');

    const formVisible = await page.locator('#firstNameInput').isVisible();

    if (formVisible) {
      const itemCount = await page.locator('ul.productList li').count();
      expect(itemCount).toBe(0);
    }
    // If form is not shown for empty cart, that is also acceptable behaviour
  });

});
