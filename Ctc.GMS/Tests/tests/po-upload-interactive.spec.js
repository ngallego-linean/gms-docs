// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');

const screenshotDir = '/tmp/gms-screenshots';
const testPDF = '/Users/nathanmayer/Downloads/PO 4237 - 2021TRIE508 - Tehama COE.pdf';

test.describe('PO Upload Interactive Flow', () => {

  test.beforeAll(async () => {
    const fs = require('fs');
    if (!fs.existsSync(screenshotDir)) {
      fs.mkdirSync(screenshotDir, { recursive: true });
    }
  });

  test('Full upload flow with real PDF', async ({ page }) => {
    // Step 1: Navigate to upload page
    await page.goto('/FiscalTeam/UploadPO');
    await page.waitForTimeout(500);

    await page.screenshot({
      path: path.join(screenshotDir, 'po-test-1-initial.png'),
      fullPage: true
    });
    console.log('Step 1: Initial page captured');

    // Step 2: Upload the real PO PDF
    const fileInput = page.locator('#poFiles');
    await fileInput.setInputFiles(testPDF);

    // Step 3: Capture processing state (quick)
    await page.screenshot({
      path: path.join(screenshotDir, 'po-test-2-processing.png'),
      fullPage: true
    });
    console.log('Step 2: Processing state captured');

    // Step 4: Wait for parsed results to appear
    await page.waitForSelector('#parsedResultsCard', { state: 'visible', timeout: 5000 });
    await page.waitForTimeout(300);

    await page.screenshot({
      path: path.join(screenshotDir, 'po-test-3-parsed.png'),
      fullPage: true
    });
    console.log('Step 3: Parsed results captured');

    // Verify extracted data is displayed
    const poNumber = await page.locator('#parsedPONumber').textContent();
    const poDate = await page.locator('#parsedPODate').textContent();
    const supplier = await page.locator('#parsedSupplier').textContent();
    const amount = await page.locator('#parsedAmount').textContent();

    console.log(`Extracted: PO=${poNumber}, Date=${poDate}, Supplier=${supplier}, Amount=${amount}`);

    // Step 5: Click confirm button
    await page.click('button:has-text("Confirm & Save PO")');
    await page.waitForTimeout(500);

    await page.screenshot({
      path: path.join(screenshotDir, 'po-test-4-success.png'),
      fullPage: true
    });
    console.log('Step 4: Success state captured');

    // Verify success message
    const successPO = await page.locator('#successPONumber').textContent();
    const successDistrict = await page.locator('#successDistrict').textContent();
    console.log(`Success: PO ${successPO} linked to ${successDistrict}`);
  });

});
