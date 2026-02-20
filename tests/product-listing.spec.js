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

test.describe('Product Listing Tests', () => {

  test('TC-176 - Home page loads with product listings', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-176]]');
    await signIn(page, 'demouser');

    await page.waitForSelector('.shelf-item');

    const productCount = await page.locator('.shelf-item').count();
    expect(productCount).toBeGreaterThan(0);

    const firstProduct = page.locator('.shelf-item').first();
    await expect(firstProduct.locator('.shelf-item__thumb img')).toBeVisible();
    await expect(firstProduct.locator('.shelf-item__title')).toBeVisible();
    await expect(firstProduct.locator('.shelf-item__price')).toBeVisible();
  });

  test('TC-177 - Product images load correctly for demouser', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-177]]');
    await signIn(page, 'demouser');
    await page.waitForSelector('.shelf-item__thumb img');

    const images = page.locator('.shelf-item__thumb img');
    const count = await images.count();
    expect(count).toBeGreaterThan(0);

    for (let i = 0; i < count; i++) {
      const naturalWidth = await images.nth(i).evaluate(img => img.naturalWidth);
      expect(naturalWidth).toBeGreaterThan(0);
    }
  });

  test('TC-178 - Product images fail to load for image_not_loading_user', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-178]]');
    await signIn(page, 'image_not_loading_user');
    await page.waitForSelector('.shelf-item');
    await page.waitForTimeout(2000);

    const images = page.locator('.shelf-item__thumb img');
    const count = await images.count();
    expect(count).toBeGreaterThan(0);

    let brokenCount = 0;
    for (let i = 0; i < count; i++) {
      const naturalWidth = await images.nth(i).evaluate(img => img.naturalWidth);
      if (naturalWidth === 0) brokenCount++;
    }

    expect(brokenCount).toBeGreaterThan(0);
  });

  test('TC-179 - Filter products by a single category', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-179]]');
    await signIn(page, 'demouser');
    await page.waitForSelector('.shelf-item');

    const totalBefore = await page.locator('.shelf-item').count();
    expect(totalBefore).toBeGreaterThan(0);

    await page.locator(".filters input[value='Apple']").check();
    await page.waitForTimeout(500);

    const filteredCount = await page.locator('.shelf-item').count();
    expect(filteredCount).toBeGreaterThan(0);
    expect(filteredCount).toBeLessThanOrEqual(totalBefore);

    const foundText = await page.locator('small.products-found span').innerText();
    expect(foundText).toContain('Product(s) found');

    await page.locator(".filters input[value='Apple']").uncheck();
    await page.waitForTimeout(500);

    const restoredCount = await page.locator('.shelf-item').count();
    expect(restoredCount).toBe(totalBefore);
  });

  test('TC-180 - Filter products by multiple categories simultaneously', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-180]]');
    await signIn(page, 'demouser');
    await page.waitForSelector('.shelf-item');

    await page.locator(".filters input[value='Apple']").check();
    await page.waitForTimeout(500);
    const appleCount = await page.locator('.shelf-item').count();

    await page.locator(".filters input[value='Samsung']").check();
    await page.waitForTimeout(500);
    const bothCount = await page.locator('.shelf-item').count();

    expect(bothCount).toBeGreaterThanOrEqual(appleCount);
    expect(bothCount).toBeGreaterThan(0);
  });

  test.skip('TC-181 - Sort products by price (low to high)', async ({ page }) => {
    // Sort control not present on testathon.live — feature not implemented on live site
    console.log('[[PROPERTY|id=TC-181]]');
  });

  test.skip('TC-182 - Sort products by price (high to low)', async ({ page }) => {
    // Sort control not present on testathon.live — feature not implemented on live site
    console.log('[[PROPERTY|id=TC-182]]');
  });

  test('TC-183 - Home page is accessible without authentication', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-183]]');
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('body')).toBeVisible();
    const title = await page.title();
    expect(title).not.toBe('');
  });

});
