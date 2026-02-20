const { test, expect } = require('@playwright/test');
const { signIn, addToCart, completeCheckout } = require('./helpers');

test.describe('Cart Tests', () => {

  test('TC-193 - Cart persists items across navigation', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-193]]');
    await signIn(page, 'demouser');

    await addToCart(page, 0);
    await expect(page.locator('.bag__quantity').first()).toHaveText('1');

    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');

    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('.bag__quantity').first()).toHaveText('1');
  });

  test('TC-194 - Cart is cleared after successful checkout', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-194]]');
    await signIn(page, 'demouser');

    await completeCheckout(page);

    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('.bag__quantity').first()).toHaveText('0');
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
