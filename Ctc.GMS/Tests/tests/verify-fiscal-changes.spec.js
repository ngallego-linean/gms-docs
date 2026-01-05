// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');

const screenshotDir = '/tmp/gms-screenshots';

test.describe('Verify Fiscal Team Changes', () => {

  test.beforeAll(async () => {
    const fs = require('fs');
    if (!fs.existsSync(screenshotDir)) {
      fs.mkdirSync(screenshotDir, { recursive: true });
    }
  });

  test('GAA Preview - Original Agreement', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');
    await page.waitForTimeout(1000);

    await page.screenshot({
      path: path.join(screenshotDir, '10-gaa-original-agreement.png'),
      fullPage: true
    });

    console.log('GAA Original Agreement screenshot captured');
  });

  test('Invoice Preview', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');
    await page.waitForTimeout(1000);

    await page.screenshot({
      path: path.join(screenshotDir, '11-invoice-preview.png'),
      fullPage: true
    });

    console.log('Invoice Preview screenshot captured');
  });

  test('Bulk PO Upload Page', async ({ page }) => {
    await page.goto('/FiscalTeam/UploadPO');
    await page.waitForTimeout(1000);

    await page.screenshot({
      path: path.join(screenshotDir, '12-bulk-po-upload.png'),
      fullPage: true
    });

    console.log('Bulk PO Upload screenshot captured');
  });

  test('Bulk Warrant Entry Page', async ({ page }) => {
    await page.goto('/FiscalTeam/UploadWarrant');
    await page.waitForTimeout(1000);

    await page.screenshot({
      path: path.join(screenshotDir, '13-bulk-warrant-entry.png'),
      fullPage: true
    });

    console.log('Bulk Warrant Entry screenshot captured');
  });

});
