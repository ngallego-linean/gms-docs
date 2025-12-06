// @ts-check
const { test, expect } = require('@playwright/test');

test.describe('Invoice Preview Feature', () => {

  test('Invoice list page loads', async ({ page }) => {
    await page.goto('/FiscalTeam/Invoices');

    // Verify page loads
    await expect(page).toHaveTitle(/Invoice Generation/);

    // Check that the page header is visible
    await expect(page.locator('h2').first()).toContainText('Invoice Generation');

    // Look for Preview buttons (may be 0 if no approved students in mock data)
    const previewButtons = page.locator('a:has-text("Preview")');
    const count = await previewButtons.count();
    console.log(`Found ${count} preview button(s) on invoice list`);
  });

  test('Invoice preview page loads with correct data', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Verify page loads
    await expect(page).toHaveTitle(/Invoice Preview/);

    // Check header
    await expect(page.locator('h2').first()).toContainText('Invoice Preview');

    // Check invoice document elements
    await expect(page.locator('.invoice-title')).toContainText('INVOICE');
    await expect(page.locator('.invoice-number')).not.toBeEmpty();

    // Check FROM section (grantee)
    await expect(page.locator('.invoice-from .party-name')).not.toBeEmpty();

    // Check TO section (CTC)
    await expect(page.locator('.invoice-to .party-name')).toContainText('Commission on Teacher Credentialing');

    // Check student table has rows
    const studentRows = page.locator('.invoice-student-table tbody tr');
    const rowCount = await studentRows.count();
    expect(rowCount).toBeGreaterThan(0);
    console.log(`Found ${rowCount} student(s) in attachment`);
  });

  test('Invoice preview shows correct LEA for group 1', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check the grantee is Fresno Unified (group 1)
    await expect(page.locator('.invoice-from .party-name'))
      .toContainText('Fresno Unified School District');
  });

  test('Invoice preview shows correct LEA for group 2', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/2');

    // Check the grantee is Sacramento (group 2)
    await expect(page.locator('.invoice-from .party-name'))
      .toContainText('Sacramento City Unified School District');
  });

  test('Invoice preview shows invoice number and date', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check invoice number format
    const invoiceNumber = await page.locator('.invoice-number').textContent();
    expect(invoiceNumber).toMatch(/INV-STS-\d{4}-\d{3}/);

    // Check date is present
    await expect(page.locator('.invoice-meta-row:has-text("Date") .value')).not.toBeEmpty();
  });

  test('Invoice preview shows program information', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check program section
    await expect(page.locator('.program-name')).toContainText('Student Teacher Stipend Program');
    await expect(page.locator('.grant-number')).toContainText('STS-2025');
  });

  test('Invoice preview shows line items table', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check invoice table exists
    await expect(page.locator('.invoice-table')).toBeVisible();

    // Check total row
    await expect(page.locator('.invoice-table .invoice-total-row')).toBeVisible();
    await expect(page.locator('.invoice-total')).toContainText('$');
  });

  test('Invoice preview shows accounting information', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check ENY field
    await expect(page.locator('.accounting-field:has-text("ENY")')).toBeVisible();

    // Check Account Code field
    await expect(page.locator('.accounting-field:has-text("Account Code")')).toBeVisible();
  });

  test('Invoice preview has FisCal submission checkbox', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check FisCal checkbox exists
    const checkbox = page.locator('#fiscalSubmitted');
    await expect(checkbox).toBeVisible();

    // Check label
    await expect(page.locator('.fiscal-checkbox-label')).toContainText('Mark as Submitted to FisCal');
  });

  test('Invoice preview has Print button', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check Print button exists
    const printButton = page.locator('button:has-text("Print Invoice")');
    await expect(printButton).toBeVisible();
  });

  test('Invoice preview has back navigation', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check Back to Invoice List link exists
    const backLink = page.locator('a:has-text("Back to Invoice List")');
    await expect(backLink).toBeVisible();

    // Click and verify navigation
    await backLink.click();
    await expect(page).toHaveURL(/\/FiscalTeam\/Invoices/);
  });

  test('Invoice preview summary card shows correct info', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check summary card items
    await expect(page.locator('.summary-item:has-text("Invoice Number")')).toBeVisible();
    await expect(page.locator('.summary-item:has-text("Grant Number")')).toBeVisible();
    await expect(page.locator('.summary-item:has-text("Students")')).toBeVisible();
    await expect(page.locator('.summary-item:has-text("Total Amount")')).toBeVisible();
  });

  test('Invoice preview student table has total row', async ({ page }) => {
    await page.goto('/FiscalTeam/InvoicePreview/1');

    // Check total row in student table
    const totalRow = page.locator('.invoice-student-table .invoice-total-row');
    await expect(totalRow).toBeVisible();
    await expect(totalRow).toContainText('Total:');

    // Verify total amount contains dollar sign
    const totalText = await totalRow.textContent();
    expect(totalText).toContain('$');
  });

});
