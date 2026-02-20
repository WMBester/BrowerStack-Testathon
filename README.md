# BrowserStack Testathon — Playwright Test Suite

Automated end-to-end test suite for [testathon.live](https://testathon.live) (StackDemo), built with [Playwright](https://playwright.dev) and integrated with [BrowserStack Automate](https://www.browserstack.com/automate).

---

## Prerequisites

- [Node.js](https://nodejs.org) v18 or later
- npm v9 or later
- A [BrowserStack](https://www.browserstack.com) account (for cloud execution)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/WMBester/BrowerStack-Testathon.git
cd BrowerStack-Testathon
```

### 2. Install dependencies

```bash
npm install
```

### 3. Install Playwright browsers

```bash
npx playwright install
```

---

## Running Tests

### Run all tests locally (headless)

```bash
npm test
```

### Run all tests locally (headed / visible browser)

```bash
npm run test:headed
```

### Run a specific test file

```bash
npx playwright test tests/smoke.spec.js
```

### Run tests for a specific browser project

```bash
npx playwright test --project=Chrome
npx playwright test --project=Safari
npx playwright test --project=Edge
```

### Run tests on a specific mobile device

```bash
npx playwright test --project="iPhone 15 Pro"
npx playwright test --project="Samsung Galaxy S23"
npx playwright test --project="Google Pixel 8"
```

---

## Running Tests on BrowserStack

### 1. Set your credentials

Export your BrowserStack credentials as environment variables:

```bash
export BROWSERSTACK_USERNAME=your_username
export BROWSERSTACK_ACCESS_KEY=your_access_key
```

> On Windows (PowerShell):
> ```powershell
> $env:BROWSERSTACK_USERNAME="your_username"
> $env:BROWSERSTACK_ACCESS_KEY="your_access_key"
> ```

### 2. Run via BrowserStack SDK

```bash
npx browserstack-node-sdk playwright test
```

The SDK reads platform configuration from [browserstack.yml](browserstack.yml) and distributes tests across the configured browsers and devices automatically.

---

## Test Coverage

| File | Area |
|---|---|
| `smoke.spec.js` | Homepage loads, basic sanity |
| `signin.spec.js` | Login, logout, locked user, auth redirects |
| `navigation.spec.js` | Nav links, header elements |
| `product-listing.spec.js` | Product cards, vendor filters, product count |
| `product-cart.spec.js` | Add/remove products, cart quantity |
| `cart.spec.js` | Cart sidebar, subtotal, empty state |
| `favourites.spec.js` | Favourite/unfavourite products |
| `checkout.spec.js` | Shipping form, validation, submission |
| `order-confirmation.spec.js` | Confirmation page, order number, receipt link |
| `orders.spec.js` | Order history, empty state |
| `offers.spec.js` | Geolocation-based promotional offers |

---

## Browser & Device Matrix

Configured in [browserstack.yml](browserstack.yml) and [playwright.config.js](playwright.config.js):

| Platform | Browser | Version |
|---|---|---|
| Windows 11 | Chrome | Latest |
| Windows 11 | Edge | Latest |
| macOS Sequoia | Safari | Latest |
| iPhone 15 Pro (iOS 17) | Safari | — |
| Samsung Galaxy S23 (Android 13) | Chrome | — |
| Google Pixel 8 (Android 14) | Chrome | — |

---

## Project Structure

```
.
├── tests/
│   ├── helpers.js              # Shared utilities (sign-in, navigation helpers)
│   ├── smoke.spec.js
│   ├── signin.spec.js
│   ├── navigation.spec.js
│   ├── product-listing.spec.js
│   ├── product-cart.spec.js
│   ├── cart.spec.js
│   ├── favourites.spec.js
│   ├── checkout.spec.js
│   ├── order-confirmation.spec.js
│   ├── orders.spec.js
│   └── offers.spec.js
├── docs/
│   └── SELECTORS.md            # Element selectors and site reference
├── playwright.config.js        # Local Playwright configuration
├── browserstack.yml            # BrowserStack Automate configuration
└── package.json
```

---

## Test Accounts

The following accounts are available on [testathon.live](https://testathon.live):

| Username | Password | Notes |
|---|---|---|
| `demouser` | `testingisfun99` | Standard test user |
| `existing_orders_user` | `testingisfun99` | Pre-seeded order history |
| `fav_user` | `testingisfun99` | Pre-seeded favourites (5 items) |
| `image_not_loading_user` | `testingisfun99` | Broken image scenario |
| `locked_user` | `testingisfun99` | Locked account scenario |

---

## Test Results

JUnit XML results are written to `test-results/junit-results.xml` after each run. BrowserStack Test Observability is enabled for cloud runs and provides a live dashboard of results.
