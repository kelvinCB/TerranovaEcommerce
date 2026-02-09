import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import CategoryManager from '../components/ui/CategoryManager';

describe('CategoryManager Component', () => {
    describe('Initial State', () => {
        it('should render with default "Electrónicos" category', () => {
            render(<CategoryManager />);
            expect(screen.getByText('Electrónicos')).toBeInTheDocument();
        });

        it('should display default items in Electrónicos category', () => {
            render(<CategoryManager />);
            expect(screen.getByText('Laptops')).toBeInTheDocument();
            expect(screen.getByText('Celulares')).toBeInTheDocument();
            expect(screen.getByText('PC Torres')).toBeInTheDocument();
        });

        it('should show item count badge', () => {
            render(<CategoryManager />);
            const badge = screen.getByText('3');
            expect(badge).toBeInTheDocument();
        });
    });

    describe('Category Management', () => {
        it('should add a new category', async () => {
            render(<CategoryManager />);
            const input = screen.getByPlaceholderText('Nombre de la categoría...');
            const addButton = screen.getAllByText('Agregar').find(btn =>
                btn.closest('.border-dashed')
            );

            fireEvent.change(input, { target: { value: 'Hogar' } });
            fireEvent.click(addButton!);

            await waitFor(() => {
                expect(screen.getByText('Hogar')).toBeInTheDocument();
            });
        });

        it('should add category on Enter key press', async () => {
            render(<CategoryManager />);
            const input = screen.getByPlaceholderText('Nombre de la categoría...');

            fireEvent.change(input, { target: { value: 'Deportes' } });
            fireEvent.keyPress(input, { key: 'Enter', code: 'Enter', charCode: 13 });

            await waitFor(() => {
                expect(screen.getByText('Deportes')).toBeInTheDocument();
            });
        });

        it('should not add empty category', () => {
            render(<CategoryManager />);
            const addButton = screen.getAllByText('Agregar').find(btn =>
                btn.closest('.border-dashed')
            );

            const initialCategories = screen.getAllByRole('button', {
                name: /Eliminar categoría/i
            });
            const initialCount = initialCategories.length;

            fireEvent.click(addButton!);

            const finalCategories = screen.getAllByRole('button', {
                name: /Eliminar categoría/i
            });
            expect(finalCategories.length).toBe(initialCount);
        });

        it('should delete a category', async () => {
            render(<CategoryManager />);
            const deleteButton = screen.getAllByLabelText('Eliminar categoría')[0];

            fireEvent.click(deleteButton);

            await waitFor(() => {
                expect(screen.queryByText('Electrónicos')).not.toBeInTheDocument();
            });
        });

        it('should toggle category expansion', async () => {
            render(<CategoryManager />);
            const categoryButton = screen.getByText('Electrónicos').closest('button');

            // Initially expanded, items should be in the document
            expect(screen.getByText('Laptops')).toBeInTheDocument();

            // Click to collapse
            fireEvent.click(categoryButton!);

            await waitFor(() => {
                // When collapsed, items are not rendered in DOM
                expect(screen.queryByText('Laptops')).not.toBeInTheDocument();
            });

            // Click to expand again
            fireEvent.click(categoryButton!);

            await waitFor(() => {
                expect(screen.getByText('Laptops')).toBeInTheDocument();
            });
        });
    });

    describe('Item Management', () => {
        it('should add a new item to a category', async () => {
            render(<CategoryManager />);
            const itemInput = screen.getByPlaceholderText('Nombre del item...');
            const addItemButton = screen.getAllByText('Agregar')[0];

            fireEvent.change(itemInput, { target: { value: 'Tablets' } });
            fireEvent.click(addItemButton);

            await waitFor(() => {
                expect(screen.getByText('Tablets')).toBeInTheDocument();
            });
        });

        it('should update item count when adding item', async () => {
            render(<CategoryManager />);
            const itemInput = screen.getByPlaceholderText('Nombre del item...');
            const addItemButton = screen.getAllByText('Agregar')[0];

            // Initial count is 3
            expect(screen.getByText('3')).toBeInTheDocument();

            fireEvent.change(itemInput, { target: { value: 'Tablets' } });
            fireEvent.click(addItemButton);

            await waitFor(() => {
                expect(screen.getByText('4')).toBeInTheDocument();
            });
        });

        it('should delete an item from category', async () => {
            render(<CategoryManager />);
            const laptopsItem = screen.getByText('Laptops').closest('li');

            // Find delete button within the item
            const deleteButton = laptopsItem!.querySelector('button[aria-label="Eliminar item"]');
            fireEvent.click(deleteButton!);

            await waitFor(() => {
                expect(screen.queryByText('Laptops')).not.toBeInTheDocument();
            });
        });

        it('should not add empty item', () => {
            render(<CategoryManager />);
            const addItemButton = screen.getAllByText('Agregar')[0];

            // Count before
            expect(screen.getByText('3')).toBeInTheDocument();

            // Try to add empty item
            fireEvent.click(addItemButton);

            // Count should remain the same
            expect(screen.getByText('3')).toBeInTheDocument();
        });

        it('should add item on Enter key press', async () => {
            render(<CategoryManager />);
            const itemInput = screen.getByPlaceholderText('Nombre del item...');

            fireEvent.change(itemInput, { target: { value: 'Smartwatches' } });
            fireEvent.keyPress(itemInput, { key: 'Enter', code: 'Enter', charCode: 13 });

            await waitFor(() => {
                expect(screen.getByText('Smartwatches')).toBeInTheDocument();
            });
        });
    });

    describe('Responsive Design', () => {
        it('should have responsive container classes', () => {
            render(<CategoryManager />);
            const containers = document.querySelectorAll('.flex.flex-col, .sm\\:flex-row');
            expect(containers.length).toBeGreaterThan(0);
        });

        it('should have responsive input classes', () => {
            render(<CategoryManager />);
            const inputs = screen.getAllByRole('textbox');

            inputs.forEach(input => {
                const classes = input.className;
                // Should have min-w-0 or flex-1 for proper responsive behavior
                expect(classes).toMatch(/min-w-0|flex-1/);
            });
        });
    });

    describe('Accessibility', () => {
        it('should have proper aria labels for delete buttons', () => {
            render(<CategoryManager />);
            const categoryDeleteButtons = screen.getAllByLabelText('Eliminar categoría');
            expect(categoryDeleteButtons.length).toBeGreaterThan(0);

            const itemDeleteButtons = screen.getAllByLabelText('Eliminar item');
            expect(itemDeleteButtons.length).toBeGreaterThan(0);
        });

        it('should have proper placeholder text', () => {
            render(<CategoryManager />);
            expect(screen.getByPlaceholderText('Nombre del item...')).toBeInTheDocument();
            expect(screen.getByPlaceholderText('Nombre de la categoría...')).toBeInTheDocument();
        });
    });
});
