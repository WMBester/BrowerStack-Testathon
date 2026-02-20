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

test.describe('Product Cart Tests', () => {

  test('TC-184 - Add a single product to cart', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-184]]');
    await signIn(page, 'demouser');

    const cartBadgeBefore = await page.locator('.bag__quantity').first().innerText();
    expect(cartBadgeBefore).toBe('0');

    await addToCart(page, 0);

    const cartBadgeAfter = await page.locator('.bag__quantity').first().innerText();
    expect(cartBadgeAfter).toBe('1');
  });

  test('TC-185 - Add multiple different products to cart', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-185]]');
    await signIn(page, 'demouser');
    await page.waitForSelector('.shelf-item__buy-btn');

    await addToCart(page, 0);
    const countAfterFirst = await page.locator('.bag__quantity').first().innerText();
    expect(countAfterFirst).toBe('1');

    await addToCart(page, 1);
    const countAfterSecond = await page.locator('.bag__quantity').first().innerText();
    expect(countAfterSecond).toBe('2');

    const cartItems = page.locator('.float-cart__shelf-container .shelf-item');
    await expect(cartItems).toHaveCount(2);
  });

  test('TC-186 - Add the same product to cart multiple times', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-186]]');
    await signIn(page, 'demouser');
    await page.waitForSelector('.shelf-item__buy-btn');

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
    await page.waitForTimeout(600);

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
    await page.waitForTimeout(600);

    const countAfter = await page.locator('.shelf-item').count();
    expect(countAfter).toBeLessThan(countBefore);
  });

});
