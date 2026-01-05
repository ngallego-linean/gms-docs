// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');

const screenshotDir = '/tmp/gms-screenshots';

test.describe('PO Upload Auto-Parse Flow', () => {

  test.beforeAll(async () => {
    const fs = require('fs');
    if (!fs.existsSync(screenshotDir)) {
      fs.mkdirSync(screenshotDir, { recursive: true });
    }
  });

  test('Step 1 - Initial Upload State', async ({ page }) => {
    await page.goto('/FiscalTeam/UploadPO');
    await page.waitForTimeout(500);

    await page.screenshot({
      path: path.join(screenshotDir, 'po-flow-1-initial.png'),
      fullPage: true
    });

    console.log('PO Upload - Initial state captured');
  });

  test('Step 2 - Processing State', async ({ page }) => {
    await page.goto('/FiscalTeam/UploadPO');
    await page.waitForTimeout(500);

    // Trigger the processing state via JavaScript
    await page.evaluate(() => {
      document.getElementById('uploadCard').style.display = 'none';
      document.getElementById('processingCard').style.display = 'block';
    });

    await page.screenshot({
      path: path.join(screenshotDir, 'po-flow-2-processing.png'),
      fullPage: true
    });

    console.log('PO Upload - Processing state captured');
  });

  test('Step 3 - Parsed Results with Auto-Match', async ({ page }) => {
    await page.goto('/FiscalTeam/UploadPO');
    await page.waitForTimeout(500);

    // Trigger the parsed results state
    await page.evaluate(() => {
      document.getElementById('uploadCard').style.display = 'none';
      document.getElementById('parsedResultsCard').style.display = 'block';

      // Populate mock data
      document.getElementById('parsedPONumber').textContent = '6360-0000004237';
      document.getElementById('parsedPODate').textContent = '05-09-2025';
      document.getElementById('parsedSupplier').textContent = 'TEHAMA CNTY DEPT OF EDUCATION';
      document.getElementById('parsedAmount').textContent = '$2,400,000.00';
      document.getElementById('parsedFileName').textContent = 'PO 4237 - 2021TRIE508 - Tehama COE.pdf';

      // Show auto-match alert
      document.getElementById('autoMatchAlert').style.display = 'flex';
      document.getElementById('matchedDistrict').textContent = 'Tehama County Dept of Education';

      // Highlight matched row
      document.querySelector('tr[data-district="tehama"]').classList.add('matched-row');
    });

    await page.screenshot({
      path: path.join(screenshotDir, 'po-flow-3-parsed.png'),
      fullPage: true
    });

    console.log('PO Upload - Parsed results captured');
  });

  test('Step 4 - Success State', async ({ page }) => {
    await page.goto('/FiscalTeam/UploadPO');
    await page.waitForTimeout(500);

    // Show success state
    await page.evaluate(() => {
      document.getElementById('uploadCard').style.display = 'none';
      document.getElementById('successCard').style.display = 'block';
      document.getElementById('successPONumber').textContent = '6360-0000004237';
      document.getElementById('successDistrict').textContent = 'Tehama County Dept of Education';

      // Remove matched row from pending table
      const matchedRow = document.querySelector('tr[data-district="tehama"]');
      if (matchedRow) matchedRow.remove();
    });

    await page.screenshot({
      path: path.join(screenshotDir, 'po-flow-4-success.png'),
      fullPage: true
    });

    console.log('PO Upload - Success state captured');
  });

});
