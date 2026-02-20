const { test, expect } = require('@playwright/test');
const { signIn, signInForm } = require('./helpers');

test.describe('Offers Tests', () => {

  test('TC-216 - Offers page requires authentication', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-216]]');
    await page.goto('/offers');

    await expect(page).toHaveURL(/\/signin\?offers=true/);

    await signInForm(page, 'demouser');

    await expect(page).toHaveURL(/\/offers/);
  });

  test('TC-217 - Offers page requests geolocation permission', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-217]]');
    // Default context — no geolocation permission granted
    await signIn(page, 'demouser');
    await page.goto('/offers');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('body')).toContainText(
      /Please enable Geolocation|promotional offers|Geolocation is not available/
    );
  });

  test('TC-218 - Offers display when geolocation is allowed', async ({ page, context }) => {
    console.log('[[PROPERTY|id=TC-218]]');
    await context.grantPermissions(['geolocation']);
    await context.setGeolocation({ latitude: 37.7749, longitude: -122.4194 });

    await signIn(page, 'demouser');
    await page.goto('/offers');
    await page.waitForLoadState('networkidle');
    await page.locator('div.offer').first().waitFor();

    const offerCards = page.locator('div.offer');
    const count = await offerCards.count();
    expect(count).toBeGreaterThan(0);

    for (let i = 0; i < count; i++) {
      await expect(offerCards.nth(i).locator('img')).toBeVisible();
      await expect(offerCards.nth(i).locator('.offer-title')).toBeVisible();
    }
  });

  test('TC-219 - Error message displayed when geolocation is denied', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-219]]');
    // Default context — geolocation not granted
    await signIn(page, 'demouser');
    await page.goto('/offers');
    await page.waitForLoadState('networkidle');

    await expect(page.getByText('Please enable Geolocation in your browser.')).toBeVisible();

    const offerCardCount = await page.locator('div.offer').count();
    expect(offerCardCount).toBe(0);
  });

  test('TC-220 - No offers available message for current location', async ({ page, context }) => {
    console.log('[[PROPERTY|id=TC-220]]');
    // Use coordinates unlikely to have promotional offers (remote location)
    await context.grantPermissions(['geolocation']);
    await context.setGeolocation({ latitude: -85.0, longitude: 0.0 });

    await signIn(page, 'demouser');
    await page.goto('/offers');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('body')).toContainText(
      /Sorry we do not have any promotional offers in your city|promotional offers in your location/
    );
  });

  test('TC-221 - Offers page handles browser without geolocation support', async ({ page }) => {
    console.log('[[PROPERTY|id=TC-221]]');
    // addInitScript must be registered before any navigation so it runs on every page load
    await page.addInitScript(() => {
      Object.defineProperty(navigator, 'geolocation', { get: () => undefined });
    });

    await signIn(page, 'demouser');
    await page.goto('/offers');
    await page.waitForLoadState('networkidle');

    await expect(page.locator('body')).toContainText(
      /Geolocation is not available in your browser|Please enable Geolocation/
    );
  });

  test('TC-222 - Each offer card displays an image and a title', async ({ page, context }) => {
    console.log('[[PROPERTY|id=TC-222]]');
    await context.grantPermissions(['geolocation']);
    await context.setGeolocation({ latitude: 37.7749, longitude: -122.4194 });

    await signIn(page, 'demouser');
    await page.goto('/offers');
    await page.waitForLoadState('networkidle');
    await page.locator('div.offer').first().waitFor();

    const offerCards = page.locator('div.offer');
    const count = await offerCards.count();
    expect(count).toBeGreaterThan(0);

    for (let i = 0; i < count; i++) {
      const card = offerCards.nth(i);

      const img = card.locator('img');
      await expect(img).toBeVisible();
      const imgHeight = await img.evaluate(el => el.style.height);
      expect(imgHeight).toBe('150px');

      const title = card.locator('.offer-title');
      await expect(title).toBeVisible();
      const titleText = await title.innerText();
      expect(titleText.trim().length).toBeGreaterThan(0);
    }
  });

});
