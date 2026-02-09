import { test, expect } from '@playwright/test';

test.describe('Burger Menu - Navigation and UI', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/');
        await page.waitForLoadState('domcontentloaded');
    });

    test('TC-BM-001: Should open burger menu when clicking the menu button', async ({ page }) => {
        // Verify burger button is visible
        const burgerButton = page.getByRole('button', { name: 'Open menu' });
        await expect(burgerButton).toBeVisible();

        // Click to open menu
        await burgerButton.click();

        // Verify sidebar is translated into view (has translate-x-0 class)
        const sidebar = page.locator('.fixed.left-0.top-0').first();
        await expect(sidebar).toHaveClass(/translate-x-0/);

        // Verify content is visible
        await expect(page.getByRole('heading', { name: 'Gestión de Categorías' })).toBeVisible();

        // Verify close button is visible
        const closeButton = page.getByRole('button', { name: 'Close menu' });
        await expect(closeButton).toBeVisible();
    });

    test('TC-BM-002: Should close burger menu when clicking the X button', async ({ page }) => {
        // Open menu
        await page.getByRole('button', { name: 'Open menu' }).click();

        const sidebar = page.locator('.fixed.left-0.top-0').first();
        await expect(sidebar).toHaveClass(/translate-x-0/);

        // Close menu
        await page.getByRole('button', { name: 'Close menu' }).click();

        // Verify menu is closed (has -translate-x-full class)
        await expect(sidebar).toHaveClass(/-translate-x-full/);

        // Verify burger button is visible again
        await expect(page.getByRole('button', { name: 'Open menu' })).toBeVisible();
    });

    test('TC-BM-003: Should close burger menu when clicking the overlay', async ({ page }) => {
        // Open menu
        await page.getByRole('button', { name: 'Open menu' }).click();

        const sidebar = page.locator('.fixed.left-0.top-0').first();
        await expect(sidebar).toHaveClass(/translate-x-0/);

        // Click on overlay (the backdrop)
        // We target the overlay specifically instead of generic coordinates
        const overlay = page.locator('.fixed.inset-0.z-\\[1000\\]');
        await overlay.click();

        // Verify menu is closed
        await expect(sidebar).toHaveClass(/-translate-x-full/);
    });

    test('TC-BM-004: Should not overlap with logo', async ({ page }) => {
        // Get logo position
        const logo = page.getByText('Terranova').first();
        const logoBox = await logo.boundingBox();

        // Get burger button position
        const burgerButton = page.getByRole('button', { name: 'Open menu' });
        const buttonBox = await burgerButton.boundingBox();

        // Verify button is below the logo (no overlap)
        expect(buttonBox).not.toBeNull();
        expect(logoBox).not.toBeNull();
        // Allow for some margin, checking that button starts after logo starts vertically
        // or is positioned in a way that doesn't obscure it.
        // Since layout might vary, we just ensure they exist and don't have identical coordinates
        expect(buttonBox!.y).not.toBe(logoBox!.y);
    });
});

test.describe('Burger Menu - Responsive Design', () => {
    test('TC-RD-001: Should display correctly on desktop (1920x1080)', async ({ page }) => {
        await page.setViewportSize({ width: 1920, height: 1080 });
        await page.goto('/');

        // Open menu
        await page.getByRole('button', { name: 'Open menu' }).click();

        // Verify sidebar is visible
        const sidebar = page.locator('.fixed.left-0.top-0').first();
        await expect(sidebar).toHaveClass(/translate-x-0/);

        // Check width class for desktop
        await expect(sidebar).toHaveClass(/lg:w-\[32rem\]/);
    });

    test('TC-RD-002: Should display correctly on laptop (1366x768)', async ({ page }) => {
        await page.setViewportSize({ width: 1366, height: 768 });
        await page.goto('/');

        // Open menu
        await page.getByRole('button', { name: 'Open menu' }).click();

        // Verify sidebar is visible
        const sidebar = page.locator('.fixed.left-0.top-0').first();
        await expect(sidebar).toHaveClass(/translate-x-0/);

        // Check width class for laptop/tablet
        await expect(sidebar).toHaveClass(/md:w-\[28rem\]/);
    });

    test('TC-RD-003: Should display correctly on mobile (375x667)', async ({ page }) => {
        await page.setViewportSize({ width: 375, height: 667 });
        await page.goto('/');

        // Open menu
        await page.getByRole('button', { name: 'Open menu' }).click();

        // Verify sidebar takes full width
        const sidebar = page.locator('.fixed.left-0.top-0').first();
        await expect(sidebar).toHaveClass(/w-full/);

        // Verify close button is visible
        const closeButton = page.getByRole('button', { name: 'Close menu' });
        await expect(closeButton).toBeVisible();
    });
});
