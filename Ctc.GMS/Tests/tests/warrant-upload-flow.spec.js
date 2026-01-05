// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');

const screenshotDir = '/tmp/gms-screenshots';

test.describe('Warrant Upload Flow', () => {

  test.beforeAll(async () => {
    const fs = require('fs');
    if (!fs.existsSync(screenshotDir)) {
      fs.mkdirSync(screenshotDir, { recursive: true });
    }
  });

  test('Full warrant CSV upload flow', async ({ page }) => {
    // Step 1: Initial page
    await page.goto('/FiscalTeam/UploadWarrant');
    await page.waitForTimeout(500);

    await page.screenshot({
      path: path.join(screenshotDir, 'warrant-1-initial.png'),
      fullPage: true
    });
    console.log('Step 1: Initial page captured');

    // Step 2: Simulate file upload - trigger processing state
    await page.evaluate(() => {
      document.getElementById('uploadCard').style.display = 'none';
      document.getElementById('processingCard').style.display = 'block';
    });

    await page.screenshot({
      path: path.join(screenshotDir, 'warrant-2-processing.png'),
      fullPage: true
    });
    console.log('Step 2: Processing state captured');

    // Step 3: Show parsed results
    await page.evaluate(() => {
      document.getElementById('processingCard').style.display = 'none';
      document.getElementById('parsedResultsCard').style.display = 'block';

      // Update file info
      document.getElementById('parsedFileName').textContent = 'fiscal_warrants_nov2025.csv';
      document.getElementById('fileMeta').textContent = '5 warrants found';
      document.getElementById('matchedCount').textContent = '4';
      document.getElementById('totalCount').textContent = '5';

      // Populate table with mock data
      const mockWarrantData = [
        { warrantNumber: 'W-2025-12450', date: '11/25/2025', vendor: 'LOS ANGELES UNIFIED SCHOOL DIST', amount: 80000, matchedInvoice: 'STS-2025-001-01', status: 'matched' },
        { warrantNumber: 'W-2025-12451', date: '11/25/2025', vendor: 'SAN DIEGO UNIFIED SCHOOL DIST', amount: 50000, matchedInvoice: 'STS-2025-002-01', status: 'matched' },
        { warrantNumber: 'W-2025-12452', date: '11/25/2025', vendor: 'FRESNO UNIFIED SCHOOL DIST', amount: 30000, matchedInvoice: 'STS-2025-003-01', status: 'matched' },
        { warrantNumber: 'W-2025-12453', date: '11/25/2025', vendor: 'SACRAMENTO CITY USD', amount: 40000, matchedInvoice: 'STS-2025-004-01', status: 'matched' },
        { warrantNumber: 'W-2025-12454', date: '11/25/2025', vendor: 'KERN COUNTY SUPERINTENDENT', amount: 25000, matchedInvoice: null, status: 'unmatched' }
      ];

      const tbody = document.getElementById('parsedWarrantsBody');
      tbody.innerHTML = mockWarrantData.map((warrant, idx) => `
        <tr class="${warrant.status === 'matched' ? 'matched-row' : 'unmatched-row'}">
          <td>
            <input type="checkbox" class="warrant-checkbox" value="${idx}" ${warrant.status === 'matched' ? 'checked' : ''} ${warrant.status === 'unmatched' ? 'disabled' : ''}>
          </td>
          <td><code>${warrant.warrantNumber}</code></td>
          <td>${warrant.date}</td>
          <td>${warrant.vendor}</td>
          <td>$${warrant.amount.toLocaleString()}</td>
          <td>${warrant.matchedInvoice ? `<code>${warrant.matchedInvoice}</code>` : '<span class="text-muted">-</span>'}</td>
          <td>
            ${warrant.status === 'matched'
              ? '<span class="status-badge status-success"><i class="mdi mdi-check"></i> Matched</span>'
              : '<span class="status-badge status-warning"><i class="mdi mdi-alert"></i> No Match</span>'}
          </td>
        </tr>
      `).join('');
    });

    await page.screenshot({
      path: path.join(screenshotDir, 'warrant-3-parsed.png'),
      fullPage: true
    });
    console.log('Step 3: Parsed results captured');

    // Step 4: Show success state
    await page.evaluate(() => {
      document.getElementById('parsedResultsCard').style.display = 'none';
      document.getElementById('successCard').style.display = 'block';
      document.getElementById('successCount').textContent = '4';
      document.getElementById('successMonth').textContent = 'November 2025';
      document.getElementById('successAmount').textContent = '$200,000';

      // Remove matched rows from pending table
      ['lausd', 'sdusd', 'fresno', 'sacramento'].forEach(lea => {
        const row = document.querySelector(`#pendingInvoicesBody tr[data-lea="${lea}"]`);
        if (row) row.remove();
      });
    });

    await page.screenshot({
      path: path.join(screenshotDir, 'warrant-4-success.png'),
      fullPage: true
    });
    console.log('Step 4: Success state captured');
  });

});
