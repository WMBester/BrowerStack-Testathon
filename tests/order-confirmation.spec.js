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

test.describe('Order Confirmation Tests', () => {

  test('TC-206 - Confirmation page displays after successful checkout', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-206]]');
    await signIn(page, 'demouser');
    await completeCheckout(page);

    await expect(page.locator('legend#confirmation-message'))
      .toHaveText('Your Order has been successfully placed.');

    const orderNumberEl = page.locator('legend#confirmation-message ~ div strong');
    await expect(orderNumberEl).toBeVisible();
    const orderNumberText = await orderNumberEl.innerText();
    const orderNum = parseInt(orderNumberText.trim());
    expect(Number.isInteger(orderNum)).toBe(true);
    expect(orderNum).toBeGreaterThanOrEqual(1);
    expect(orderNum).toBeLessThanOrEqual(100);

    await expect(page.locator("article.cart[data-test='cart']")).toBeVisible();
    await expect(page.locator('h3.cart-title')).toHaveText('Order Summary');

    const itemCount = await page.locator('ul.productList li.productList-item').count();
    expect(itemCount).toBeGreaterThan(0);
  });

  test('TC-207 - Continue Shopping button returns to home page', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-207]]');
    await signIn(page, 'demouser');
    await completeCheckout(page);

    await page.locator('button.button--tertiary').click();

    await expect(page).toHaveURL(/testathon\.live\/(\?|$)/);

    const cartCount = await page.locator('.bag__quantity').first().innerText();
    expect(cartCount).toBe('0');
  });

  test('TC-208 - Download order receipt generates a PDF download', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-208]]');
    await signIn(page, 'demouser');
    await completeCheckout(page);

    await expect(page.locator('a#downloadpdf')).toBeVisible();

    const [download] = await Promise.all([
      page.waitForEvent('download'),
      page.locator('a#downloadpdf').click(),
    ]);

    expect(download).not.toBeNull();
    const filename = download.suggestedFilename();
    expect(filename.length).toBeGreaterThan(0);
  });

  test('TC-209 - Confirmation page cannot be accessed directly without placing an order', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-209]]');
    await signIn(page, 'demouser');

    await page.goto('/confirmation');
    await page.waitForLoadState('networkidle');
    await page.waitForTimeout(1000);

    const currentUrl = page.url();
    const isHome = /testathon\.live\/(\?|$)/.test(currentUrl);
    const isConfirmation = currentUrl.includes('/confirmation');

    if (isConfirmation) {
      const successVisible = await page.locator('legend#confirmation-message').isVisible();
      expect(successVisible).toBe(false);
    } else {
      expect(isHome).toBe(true);
    }
  });

  test('TC-210 - Order number is within valid range (1-100)', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-210]]');
    await signIn(page, 'demouser');
    await completeCheckout(page);

    const orderNumberEl = page.locator('legend#confirmation-message ~ div strong');
    await expect(orderNumberEl).toBeVisible();

    const orderNumberText = await orderNumberEl.innerText();
    const orderNum = parseInt(orderNumberText.trim());
    expect(Number.isInteger(orderNum)).toBe(true);
    expect(orderNum).toBeGreaterThanOrEqual(1);
    expect(orderNum).toBeLessThanOrEqual(100);
  });

});
