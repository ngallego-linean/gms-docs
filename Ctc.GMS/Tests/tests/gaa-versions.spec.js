// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');

const screenshotDir = '/tmp/gms-screenshots';

test.describe('GAA Original vs Amendment', () => {

  test('GAA Original Agreement', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');
    await page.waitForTimeout(500);

    await page.screenshot({
      path: path.join(screenshotDir, 'gaa-original.png'),
      fullPage: true
    });

    console.log('GAA Original captured');
  });

  test('GAA Amendment A', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/2?amendment=true');
    await page.waitForTimeout(500);

    await page.screenshot({
      path: path.join(screenshotDir, 'gaa-amendment.png'),
      fullPage: true
    });

    console.log('GAA Amendment captured');
  });

});
