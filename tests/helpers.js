const { expect } = require('@playwright/test');

async function selectDropdownOption(page, containerId, optionText) {
  await page.locator(`#${containerId}`).click();
  await page.waitForSelector(`#${containerId} [class*='menu']`);
  await page.locator("div[id*='react-select'][id*='option']")
    .filter({ hasText: optionText })
    .first()
    .click();
}

// Navigates to /signin, signs in as username, and asserts a successful redirect to home.
// Use for tests where a clean sign-in flow is needed.
async function signIn(page, username) {
  await page.goto('/signin');
  await selectDropdownOption(page, 'username', username);
  await selectDropdownOption(page, 'password', 'testingisfun99');
  await page.locator('#login-btn').click();
  await expect(page).toHaveURL(/testathon\.live\/(\?|$)/);
}

// Fills and submits the sign-in form on the current page without navigating.
// Use when the test has already landed on /signin with a redirect query param
// (e.g. /signin?checkout=true) that must be preserved for the post-login redirect to work,
// or when testing a sign-in failure scenario.
async function signInForm(page, username) {
  await selectDropdownOption(page, 'username', username);
  await selectDropdownOption(page, 'password', 'testingisfun99');
  await page.locator('#login-btn').click();
}

// Adds the product at productIndex to the cart and waits for the cart badge to update.
async function addToCart(page, productIndex = 0) {
  await page.locator('.shelf-item__buy-btn').first().waitFor();
  const countBefore = await page.locator('.bag__quantity').first().innerText();
  await page.locator('.shelf-item__buy-btn').nth(productIndex).click();
  await expect(page.locator('.bag__quantity').first()).not.toHaveText(countBefore);
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

module.exports = { selectDropdownOption, signIn, signInForm, addToCart, completeCheckout };
