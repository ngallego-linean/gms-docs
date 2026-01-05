// @ts-check
const { test, expect } = require('@playwright/test');

test.describe('GAA Preview Feature', () => {

  test('GAA list page loads and shows preview button', async ({ page }) => {
    // Navigate to GAA page
    await page.goto('/FiscalTeam/GAA');

    // Verify page loads
    await expect(page).toHaveTitle(/Generate GAA/);

    // Check that the page header is visible
    await expect(page.locator('h2')).toContainText('Generate Grant Award Agreements');

    // Look for Preview buttons
    const previewButtons = page.locator('a:has-text("Preview")');
    const count = await previewButtons.count();

    // Should have at least one preview button if there are disbursement groups
    console.log(`Found ${count} preview button(s)`);
  });

  test('GAA preview page loads with correct data', async ({ page }) => {
    // Navigate directly to preview for group 1
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Verify page loads
    await expect(page).toHaveTitle(/GAA Document Preview/);

    // Check header (use first() since there are multiple h2 elements)
    await expect(page.locator('h2').first()).toContainText('Grant Award Agreement');

    // Check GAA document elements
    await expect(page.locator('.gaa-title')).toContainText('Student Teacher Stipend Program');
    await expect(page.locator('.gaa-subtitle')).toContainText('Grant Award Agreement');

    // Check grantee information is populated
    await expect(page.locator('.gaa-info-row:has-text("Grantee Name") .gaa-value')).not.toBeEmpty();
    await expect(page.locator('.gaa-info-row:has-text("Grant Number") .gaa-value')).not.toBeEmpty();
    await expect(page.locator('.gaa-info-row:has-text("Agreement Term") .gaa-value')).not.toBeEmpty();

    // Check payment section
    await expect(page.locator('.gaa-payment-value')).toBeVisible();

    // Check Attachment A has students
    const studentRows = page.locator('.gaa-student-table tbody tr');
    const rowCount = await studentRows.count();
    expect(rowCount).toBeGreaterThan(0);
    console.log(`Found ${rowCount} student(s) in Attachment A`);

    // Check summary card
    await expect(page.locator('.summary-item:has-text("Grant Number")')).toBeVisible();
    await expect(page.locator('.summary-item:has-text("Total Amount")')).toBeVisible();
  });

  test('GAA preview shows correct LEA for group 1', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check the grantee is Fresno Unified (group 1)
    await expect(page.locator('.gaa-info-row:has-text("Grantee Name") .gaa-value'))
      .toContainText('Fresno Unified School District');
  });

  test('GAA preview shows correct LEA for group 2', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/2');

    // Check the grantee is Sacramento (group 2)
    await expect(page.locator('.gaa-info-row:has-text("Grantee Name") .gaa-value'))
      .toContainText('Sacramento City Unified School District');
  });

  test('GAA preview has signer information form', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check signer form fields exist
    await expect(page.locator('#signerName')).toBeVisible();
    await expect(page.locator('#signerEmail')).toBeVisible();

    // Verify they have default values
    const signerName = await page.locator('#signerName').inputValue();
    const signerEmail = await page.locator('#signerEmail').inputValue();

    expect(signerName.length).toBeGreaterThan(0);
    expect(signerEmail).toContain('@');

    console.log(`Signer: ${signerName} (${signerEmail})`);
  });

  test('GAA preview has Send to DocuSign button', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check Send to DocuSign buttons exist (there are two - header and sidebar)
    const docusignButtons = page.locator('button:has-text("Send to DocuSign")');
    expect(await docusignButtons.count()).toBeGreaterThanOrEqual(1);
    await expect(docusignButtons.first()).toBeVisible();

    // Also check the sidebar button specifically
    const sidebarDocusignButton = page.locator('.col-lg-4 button:has-text("Send to DocuSign")');
    await expect(sidebarDocusignButton).toBeVisible();
  });

  test('GAA preview has Print Preview button', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check Print Preview button exists
    const printButton = page.locator('button:has-text("Print Preview")');
    await expect(printButton.first()).toBeVisible();
  });

  test('GAA preview has back navigation', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check Back to GAA List link exists
    const backLink = page.locator('a:has-text("Back to GAA List")');
    await expect(backLink).toBeVisible();

    // Click and verify navigation
    await backLink.click();
    await expect(page).toHaveURL(/\/FiscalTeam\/GAA/);
  });

  test('GAA preview shows definitions section', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check definitions section exists
    await expect(page.locator('.gaa-section h3:has-text("Definitions")')).toBeVisible();

    // Check key definitions are present
    await expect(page.locator('.gaa-definitions')).toContainText('Grantee');
    await expect(page.locator('.gaa-definitions')).toContainText('Commission');
    await expect(page.locator('.gaa-definitions')).toContainText('Student Teaching');
  });

  test('GAA preview shows terms and conditions', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check terms section exists
    await expect(page.locator('.gaa-section h3:has-text("Terms and Conditions")')).toBeVisible();

    // Check key terms are present
    await expect(page.locator('.gaa-terms')).toContainText('funding to the student teacher');
    await expect(page.locator('.gaa-terms')).toContainText('Grants Management System');
  });

  test('GAA preview shows signature blocks', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check signature section exists
    await expect(page.locator('.gaa-section h3:has-text("Signers")')).toBeVisible();

    // Check all three signature blocks
    await expect(page.locator('.gaa-signature-title:has-text("Grantee Authorized Representative")')).toBeVisible();
    await expect(page.locator('.gaa-signature-title:has-text("Commission on Teacher Credentialing")')).toBeVisible();
    await expect(page.locator('.gaa-signature-title:has-text("Accounting Officer")')).toBeVisible();
  });

  test('GAA preview student table has total row', async ({ page }) => {
    await page.goto('/FiscalTeam/GAAPreview/1');

    // Check total row in student table
    const totalRow = page.locator('.gaa-total-row');
    await expect(totalRow).toBeVisible();
    await expect(totalRow).toContainText('Total:');

    // Verify total amount contains dollar sign
    const totalText = await totalRow.textContent();
    expect(totalText).toContain('$');
  });

});
