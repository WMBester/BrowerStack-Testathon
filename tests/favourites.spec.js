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

test.describe('Favourites Tests', () => {

  test('TC-189 - Favourites page requires authentication', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-189]]');
    await page.goto('/favourites');

    await expect(page).toHaveURL(/\/signin\?favourites=true/);

    await selectDropdownOption(page, 'username', 'demouser');
    await selectDropdownOption(page, 'password', 'testingisfun99');
    await page.locator('#login-btn').click();

    await expect(page).toHaveURL(/\/favourites/);
  });

  test('TC-190 - fav_user sees pre-seeded favourites', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-190]]');
    await signIn(page, 'fav_user');
    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');

    const productCount = await page.locator('.shelf-item').count();
    expect(productCount).toBeGreaterThan(0);

    const firstProduct = page.locator('.shelf-item').first();
    await expect(firstProduct.locator('.shelf-item__thumb img')).toBeVisible();
    await expect(firstProduct.locator('.shelf-item__title')).toBeVisible();
    await expect(firstProduct.locator('.shelf-item__price')).toBeVisible();
  });

  test('TC-191 - Empty favourites state for new user', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-191]]');
    await signIn(page, 'demouser');
    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');

    const foundText = await page.locator('small.products-found span').innerText();
    expect(foundText).toContain('0 Product(s) found');

    const productCount = await page.locator('.shelf-item').count();
    expect(productCount).toBe(0);
  });

  test('TC-192 - Add to cart from favourites page', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-192]]');
    await signIn(page, 'fav_user');
    await page.goto('/favourites');
    await page.waitForLoadState('networkidle');

    await page.waitForSelector('.shelf-item__buy-btn');
    const cartBefore = parseInt(await page.locator('.bag__quantity').first().innerText());

    await page.locator('.shelf-item__buy-btn').first().click();
    await page.waitForTimeout(600);

    const cartAfter = parseInt(await page.locator('.bag__quantity').first().innerText());
    expect(cartAfter).toBeGreaterThan(cartBefore);
  });

});
