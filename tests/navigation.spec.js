const { test, expect } = require('@playwright/test');
const { signIn, addToCart, completeCheckout } = require('./helpers');

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

    await expect(page.locator('.bag__quantity').first()).toHaveText('0');

    await addToCart(page, 0);
    await expect(page.locator('.bag__quantity').first()).toHaveText('1');

    await addToCart(page, 1);
    await expect(page.locator('.bag__quantity').first()).toHaveText('2');

    await completeCheckout(page);
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('.bag__quantity').first()).toHaveText('0');
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
    await addToCart(page, 0);
    await page.goto('/checkout');
    await page.waitForLoadState('networkidle');
    await expect(page.locator('footer').first()).toBeVisible();

    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');
    await expect(page.locator('footer').first()).toBeVisible();
  });

});
