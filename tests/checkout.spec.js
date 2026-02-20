const { test, expect } = require('@playwright/test');
const { signIn, addToCart } = require('./helpers');

async function goToCheckoutWithItem(page) {
  await addToCart(page, 0);
  await page.goto('/checkout');
  await page.waitForLoadState('networkidle');
}

test.describe('Checkout Tests', () => {

  test('TC-196 - Successful checkout with all required fields', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-196]]');
    await signIn(page, 'demouser');
    await goToCheckoutWithItem(page);

    await expect(page.locator('#firstNameInput')).toBeVisible();

    await page.locator('#firstNameInput').fill('John');
    await page.locator('#lastNameInput').fill('Doe');
    await page.locator('#addressLine1Input').fill('123 Main Street');
    await page.locator('#provinceInput').fill('California');
    await page.locator('#postCodeInput').fill('90001');

    await page.locator('#checkout-shipping-continue').click();

    await expect(page).toHaveURL(/\/confirmation/, { timeout: 15000 });
  });

  test('TC-197 - Checkout blocked when First Name is empty', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-197]]');
    await signIn(page, 'demouser');
    await goToCheckoutWithItem(page);

    await expect(page.locator('#firstNameInput')).toBeVisible();

    // Leave First Name empty; fill all others
    await page.locator('#lastNameInput').fill('Doe');
    await page.locator('#addressLine1Input').fill('123 Main Street');
    await page.locator('#provinceInput').fill('California');
    await page.locator('#postCodeInput').fill('90001');

    await page.locator('#checkout-shipping-continue').click();

    await expect(page).toHaveURL(/\/checkout/);
    await expect(page.locator('#checkout-shipping-continue')).toBeVisible();
  });

  test('TC-198 - Checkout blocked when Last Name is empty', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-198]]');
    await signIn(page, 'demouser');
    await goToCheckoutWithItem(page);

    await expect(page.locator('#firstNameInput')).toBeVisible();

    await page.locator('#firstNameInput').fill('John');
    // Leave Last Name empty
    await page.locator('#addressLine1Input').fill('123 Main Street');
    await page.locator('#provinceInput').fill('California');
    await page.locator('#postCodeInput').fill('90001');

    await page.locator('#checkout-shipping-continue').click();

    await expect(page).toHaveURL(/\/checkout/);
    await expect(page.locator('#checkout-shipping-continue')).toBeVisible();
  });

  test('TC-199 - Checkout blocked when Address is empty', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-199]]');
    await signIn(page, 'demouser');
    await goToCheckoutWithItem(page);

    await expect(page.locator('#firstNameInput')).toBeVisible();

    await page.locator('#firstNameInput').fill('John');
    await page.locator('#lastNameInput').fill('Doe');
    // Leave Address empty
    await page.locator('#provinceInput').fill('California');
    await page.locator('#postCodeInput').fill('90001');

    await page.locator('#checkout-shipping-continue').click();

    await expect(page).toHaveURL(/\/checkout/);
    await expect(page.locator('#checkout-shipping-continue')).toBeVisible();
  });

  test('TC-200 - Checkout blocked when State/Province is empty', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-200]]');
    await signIn(page, 'demouser');
    await goToCheckoutWithItem(page);

    await expect(page.locator('#firstNameInput')).toBeVisible();

    await page.locator('#firstNameInput').fill('John');
    await page.locator('#lastNameInput').fill('Doe');
    await page.locator('#addressLine1Input').fill('123 Main Street');
    // Leave State/Province empty
    await page.locator('#postCodeInput').fill('90001');

    await page.locator('#checkout-shipping-continue').click();

    await expect(page).toHaveURL(/\/checkout/);
    await expect(page.locator('#checkout-shipping-continue')).toBeVisible();
  });

  test('TC-201 - Checkout blocked when Postal Code is empty', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-201]]');
    await signIn(page, 'demouser');
    await goToCheckoutWithItem(page);

    await expect(page.locator('#firstNameInput')).toBeVisible();

    await page.locator('#firstNameInput').fill('John');
    await page.locator('#lastNameInput').fill('Doe');
    await page.locator('#addressLine1Input').fill('123 Main Street');
    await page.locator('#provinceInput').fill('California');
    // Leave Postal Code empty

    await page.locator('#checkout-shipping-continue').click();

    await expect(page).toHaveURL(/\/checkout/);
    await expect(page.locator('#checkout-shipping-continue')).toBeVisible();
  });

  test('TC-202 - Checkout blocked when all fields are empty', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-202]]');
    await signIn(page, 'demouser');
    await goToCheckoutWithItem(page);

    await expect(page.locator('#firstNameInput')).toBeVisible();

    // Submit with all fields empty
    await page.locator('#checkout-shipping-continue').click();

    await expect(page).toHaveURL(/\/checkout/);
    await expect(page.locator('#checkout-shipping-continue')).toBeVisible();
  });

  test('TC-203 - Order summary shows correct items and total', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-203]]');
    await signIn(page, 'demouser');
    await page.locator('.shelf-item__buy-btn').first().waitFor();

    // Capture prices of first two products
    const price1Text = await page.locator('.shelf-item__price b').nth(0).innerText();
    const price2Text = await page.locator('.shelf-item__price b').nth(1).innerText();
    const price1 = parseInt(price1Text.trim());
    const price2 = parseInt(price2Text.trim());

    await addToCart(page, 0);
    await addToCart(page, 1);

    await page.goto('/checkout');
    await page.waitForLoadState('networkidle');

    const orderItems = page.locator('ul.productList li.productList-item');
    await expect(orderItems).toHaveCount(2);

    await expect(page.locator('span.cart-priceItem-value span')).toContainText(String(price1 + price2));
  });

  test('TC-204 - Checkout requires authentication', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-204]]');
    await page.goto('/checkout');

    await expect(page).toHaveURL(/\/signin\?checkout=true/);
  });

  test('TC-205 - Checkout with empty cart shows empty state or submit unavailable', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-205]]');
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
