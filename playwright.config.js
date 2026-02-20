const { defineConfig, devices } = require('@playwright/test');

module.exports = defineConfig({
  testDir: './tests',
  timeout: 30000,
  reporter: [['list'], ['junit', { outputFile: 'test-results/junit-results.xml' }]],
  use: {
    baseURL: 'https://testathon.live',
    headless: true,
  },
  projects: [
    // Top 3 desktop browsers by market share
    {
      name: 'Chrome',
      use: { browserName: 'chromium' },
    },
    {
      name: 'Safari',
      use: { browserName: 'webkit' },
    },
    {
      name: 'Edge',
      use: { browserName: 'chromium', channel: 'msedge' },
    },
    // Top 3 mobile devices by market share
    {
      name: 'iPhone 15 Pro',
      use: { ...devices['iPhone 15 Pro'] },
    },
    {
      name: 'Samsung Galaxy S23',
      use: { ...devices['Galaxy S9+'] },
    },
    {
      name: 'Google Pixel 8',
      use: { ...devices['Pixel 7'] },
    },
  ],
});
