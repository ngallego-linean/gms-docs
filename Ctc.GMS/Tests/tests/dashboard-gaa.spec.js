// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');

const screenshotDir = '/tmp/gms-screenshots';

test('Dashboard with GAA and Amendment buttons', async ({ page }) => {
  await page.goto('/GrantsTeam/Dashboard');
  await page.waitForTimeout(500);

  // Scroll to the Disbursement Processing section
  await page.evaluate(() => {
    const section = document.querySelector('.disbursement-table');
    if (section) section.scrollIntoView({ behavior: 'instant', block: 'center' });
  });

  await page.waitForTimeout(300);

  await page.screenshot({
    path: path.join(screenshotDir, 'dashboard-disbursement.png'),
    fullPage: true
  });

  console.log('Dashboard disbursement section captured');
});
