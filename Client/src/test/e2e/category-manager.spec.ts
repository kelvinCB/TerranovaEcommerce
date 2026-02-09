import { test, expect } from '@playwright/test';

test.describe('Category Manager - Category Management', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/');
        await page.waitForLoadState('domcontentloaded');

        // Open the menu first, as CategoryManager is inside the BurgerMenu
        await page.getByRole('button', { name: 'Open menu' }).click();

        // Wait for the menu to be fully open
        await expect(page.locator('.fixed.left-0.top-0').first()).toHaveClass(/translate-x-0/);
    });

    test('TC-CM-001: Should display default categories', async ({ page }) => {
        // Verify default category "Electrónicos" exists
        await expect(page.getByRole('button', { name: 'Electrónicos' })).toBeVisible();

        // Verify item count badge exists
        await expect(page.getByText('3', { exact: true })).toBeVisible();
    });

    test('TC-CM-002: Should add a new category', async ({ page }) => {
        const categoryInput = page.getByPlaceholder('Nombre de la categoría...');
        const addButton = page.locator('button:has-text("Agregar")').last(); // Second "Agregar" button is for category

        // Add new category
        const randomName = `Hogar ${Date.now()}`;
        await categoryInput.fill(randomName);
        await addButton.click();

        // Verify new category appears
        await expect(page.getByRole('button', { name: randomName })).toBeVisible();

        // Verify input is cleared
        await expect(categoryInput).toHaveValue('');
    });

    test('TC-CM-003: Should add category on Enter key press', async ({ page }) => {
        const categoryInput = page.getByPlaceholder('Nombre de la categoría...');

        const randomName = `Deportes ${Date.now()}`;
        await categoryInput.fill(randomName);
        await categoryInput.press('Enter');

        // Verify new category appears
        await expect(page.getByRole('button', { name: randomName })).toBeVisible();
    });

    test('TC-CM-004: Should not add empty category', async ({ page }) => {
        const categoryInput = page.getByPlaceholder('Nombre de la categoría...');
        const addButton = page.locator('button:has-text("Agregar")').last();

        // Get initial count
        const initialCount = await page.getByRole('button', { name: /Eliminar categoría/ }).count();

        // Try to add empty category
        await categoryInput.fill('');
        await addButton.click(); // or press Enter

        // Verify count is the same
        const finalCount = await page.getByRole('button', { name: /Eliminar categoría/ }).count();
        expect(finalCount).toBe(initialCount);
    });

    test('TC-CM-005: Should delete a category', async ({ page }) => {
        // Add a temporary category first to avoid deleting default data
        const categoryInput = page.getByPlaceholder('Nombre de la categoría...');
        const addButton = page.locator('button:has-text("Agregar")').last();

        const tempName = `Temp to Delete ${Date.now()}`;
        await categoryInput.fill(tempName);
        await addButton.click();

        // Verify it was added
        await expect(page.getByRole('button', { name: tempName })).toBeVisible();

        // Find the specific delete button for this category.
        // The structure is: div -> [header -> [button(name), button(delete)]]
        // We find the container that has our category name, then find the delete button inside it.
        const categoryContainer = page.locator('div.rounded-lg').filter({ hasText: tempName }).first();
        const deleteButton = categoryContainer.getByRole('button', { name: 'Eliminar categoría' });

        await deleteButton.click();

        // Verify category is gone
        await expect(page.getByRole('button', { name: tempName })).not.toBeVisible();
    });

    test('TC-CM-006: Should toggle category expansion', async ({ page }) => {
        const categoryButton = page.getByRole('button', { name: 'Electrónicos' });

        // Verify items are visible initially
        await expect(page.getByText('Laptops')).toBeVisible();

        // Click to collapse
        await categoryButton.click();

        // Verify items are hidden
        await expect(page.getByText('Laptops')).not.toBeVisible();

        // Click to expand
        await categoryButton.click();

        // Verify items are visible again
        await expect(page.getByText('Laptops')).toBeVisible();
    });
});

test.describe('Category Manager - Item Management', () => {
    test.beforeEach(async ({ page }) => {
        await page.goto('/');
        await page.waitForLoadState('domcontentloaded');
        await page.getByRole('button', { name: 'Open menu' }).click();
        await expect(page.locator('.fixed.left-0.top-0').first()).toHaveClass(/translate-x-0/);
    });

    test('TC-IM-001: Should add item to category', async ({ page }) => {
        // We'll work with the default 'Electrónicos' category
        // Find the input within the expanded category section
        // Structure: div(category) -> div(items) -> div(input area) -> input
        const electronicsSection = page.locator('div.rounded-lg').filter({ hasText: 'Electrónicos' }).first();
        const itemInput = electronicsSection.getByPlaceholder('Nombre del item...');
        const addItemButton = electronicsSection.getByRole('button', { name: 'Agregar' });

        const newItemName = `Tablet ${Date.now()}`;
        await itemInput.fill(newItemName);
        await addItemButton.click();

        // Verify item appears
        await expect(page.getByText(newItemName)).toBeVisible();
    });

    test('TC-IM-002: Should add item with Enter key', async ({ page }) => {
        const electronicsSection = page.locator('div.rounded-lg').filter({ hasText: 'Electrónicos' }).first();
        const itemInput = electronicsSection.getByPlaceholder('Nombre del item...');

        const newItemName = `Watch ${Date.now()}`;
        await itemInput.fill(newItemName);
        await itemInput.press('Enter');

        // Verify item appears
        await expect(page.getByText(newItemName)).toBeVisible();
    });

    test('TC-IM-003: Should delete an item', async ({ page }) => {
        // Use 'Electrónicos' category
        const electronicsSection = page.locator('div.rounded-lg').filter({ hasText: 'Electrónicos' }).first();

        // Hover over the item to reveal delete button (CSS: opacity-0 group-hover:opacity-100)
        const item = electronicsSection.getByText('Laptops').first();
        await item.hover();

        // The delete button is in the strict vicinity of the item
        // Structure: li(group) -> [span(name), button(delete)]
        const itemRow = electronicsSection.locator('li').filter({ hasText: 'Laptops' });
        const deleteButton = itemRow.getByRole('button', { name: 'Eliminar item' });

        // Ensure button is "visible" or at least clickable. Playwright can click invisible elements with force:true,
        // but hover should make it visible.
        await expect(deleteButton).toBeVisible();
        await deleteButton.click();

        // Verify item is gone
        await expect(page.getByText('Laptops')).not.toBeVisible();
    });
});
