const { test, expect } = require('@playwright/test');
const { signIn, addToCart } = require('./helpers');

test.describe('Product Cart Tests', () => {

  test('TC-184 - Add a single product to cart', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-184]]');
    await signIn(page, 'demouser');

    await expect(page.locator('.bag__quantity').first()).toHaveText('0');

    await addToCart(page, 0);

    await expect(page.locator('.bag__quantity').first()).toHaveText('1');
  });

  test('TC-185 - Add multiple different products to cart', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-185]]');
    await signIn(page, 'demouser');

    await addToCart(page, 0);
    await expect(page.locator('.bag__quantity').first()).toHaveText('1');

    await addToCart(page, 1);
    await expect(page.locator('.bag__quantity').first()).toHaveText('2');

    const cartItems = page.locator('.float-cart__shelf-container .shelf-item');
    await expect(cartItems).toHaveCount(2);
  });

  test('TC-186 - Add the same product to cart multiple times', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-186]]');
    await signIn(page, 'demouser');

    await addToCart(page, 0);
    const countAfterFirst = parseInt(await page.locator('.bag__quantity').first().innerText());
    expect(countAfterFirst).toBeGreaterThanOrEqual(1);

    await addToCart(page, 0);
    const countAfterSecond = parseInt(await page.locator('.bag__quantity').first().innerText());
    expect(countAfterSecond).toBeGreaterThan(countAfterFirst);
  });

  test('TC-187 - Add to Favourites from product listing', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-187]]');
    await signIn(page, 'demouser');
    await page.waitForSelector('.shelf-stopper button');

    await page.locator('.shelf-stopper button').first().click();

    await expect(page.locator('.shelf-stopper button.clicked').first()).toBeVisible();

    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');

    const favCount = await page.locator('.shelf-item').count();
    expect(favCount).toBeGreaterThan(0);
  });

  test('TC-188 - Remove a product from favourites', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-188]]');
    await signIn(page, 'fav_user');
    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');

    const countBefore = await page.locator('.shelf-item').count();
    expect(countBefore).toBeGreaterThan(0);

    await page.locator('.shelf-stopper button.clicked').first().click();
    await expect(page.locator('.shelf-item')).not.toHaveCount(countBefore);

    const countAfter = await page.locator('.shelf-item').count();
    expect(countAfter).toBeLessThan(countBefore);
  });

});
