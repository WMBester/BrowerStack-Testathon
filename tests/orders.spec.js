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

test.describe('Orders Tests', () => {

  test('TC-211 - Orders page requires authentication', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-211]]');
    await page.goto('/orders');

    await expect(page).toHaveURL(/\/signin\?orders=true/);

    await selectDropdownOption(page, 'username', 'demouser');
    await selectDropdownOption(page, 'password', 'testingisfun99');
    await page.locator('#login-btn').click();

    await expect(page).toHaveURL(/\/orders/);
  });

  test('TC-212 - existing_orders_user sees order history', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-212]]');
    await signIn(page, 'existing_orders_user');
    await page.goto('/orders');
    await page.waitForLoadState('networkidle');

    const orderCards = page.locator('div.order');
    const count = await orderCards.count();
    expect(count).toBeGreaterThan(0);

    const firstOrder = orderCards.first();
    const labels = firstOrder.locator('span.a-color-secondary.label');
    const labelTexts = await labels.allInnerTexts();
    expect(labelTexts.some(t => t.includes('Order placed'))).toBe(true);
    expect(labelTexts.some(t => t.includes('Total'))).toBe(true);
    expect(labelTexts.some(t => t.includes('Ship to'))).toBe(true);

    await expect(firstOrder.locator('div.shipment.shipment-is-delivered')).toBeVisible();
  });

  test('TC-213 - Orders page shows empty state for user with no orders', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-213]]');
    await signIn(page, 'demouser');
    await page.goto('/orders');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('h2')).toHaveText('No orders found');

    const orderCount = await page.locator('div.order').count();
    expect(orderCount).toBe(0);
  });

  test('TC-214 - New order placed via checkout appears in order history', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-214]]');
    await signIn(page, 'demouser');
    await completeCheckout(page);

    await page.goto('/orders');
    await page.waitForLoadState('networkidle');

    const orderCount = await page.locator('div.order').count();
    expect(orderCount).toBeGreaterThan(0);
  });

  test('TC-215 - Order details display correct product information', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-215]]');
    await signIn(page, 'existing_orders_user');
    await page.goto('/orders');
    await page.waitForLoadState('networkidle');

    const firstOrder = page.locator('div.order').first();
    await expect(firstOrder.locator('img.item-image')).toBeVisible();

    const titleRow = firstOrder.locator('div.a-row').filter({ hasText: 'Title:' });
    await expect(titleRow).toBeVisible();

    const price = firstOrder.locator('span.a-size-small.a-color-price');
    await expect(price).toBeVisible();
    const priceText = await price.innerText();
    expect(priceText).toMatch(/\$[\d.,]+/);
  });

});
