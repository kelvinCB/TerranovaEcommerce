import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import BurgerMenu from '../components/ui/BurgerMenu';

describe('BurgerMenu Component', () => {
    it('should render the burger menu button when closed', () => {
        render(<BurgerMenu />);
        const button = screen.getByLabelText('Open menu');
        expect(button).toBeInTheDocument();
    });

    it('should toggle menu when burger button is clicked', async () => {
        render(<BurgerMenu />);
        const openButton = screen.getByLabelText('Open menu');

        // Find the sidebar
        const sidebar = document.querySelector('.fixed.left-0.top-0');

        // Menu should be closed initially (has -translate-x-full class)
        expect(sidebar?.className).toContain('-translate-x-full');

        // Click to open
        fireEvent.click(openButton);

        await waitFor(() => {
            const sidebar = document.querySelector('.fixed.left-0.top-0');
            // Menu should be open (has translate-x-0 class)
            expect(sidebar?.className).toContain('translate-x-0');
        });

        // Close button should now be visible
        const closeButton = screen.getByLabelText('Close menu');
        expect(closeButton).toBeInTheDocument();
    });

    it('should close menu when close button is clicked', async () => {
        render(<BurgerMenu />);
        const openButton = screen.getByLabelText('Open menu');

        // Open menu
        fireEvent.click(openButton);

        await waitFor(() => {
            const sidebar = document.querySelector('.fixed.left-0.top-0');
            expect(sidebar?.className).toContain('translate-x-0');
        });

        // Click close button
        const closeButton = screen.getByLabelText('Close menu');
        fireEvent.click(closeButton);

        await waitFor(() => {
            const sidebar = document.querySelector('.fixed.left-0.top-0');
            // Menu should be closed again
            expect(sidebar?.className).toContain('-translate-x-full');
        });
    });

    it('should close menu when overlay is clicked', async () => {
        render(<BurgerMenu />);
        const openButton = screen.getByLabelText('Open menu');

        // Open menu
        fireEvent.click(openButton);

        await waitFor(() => {
            const sidebar = document.querySelector('.fixed.left-0.top-0');
            expect(sidebar?.className).toContain('translate-x-0');
        });

        // Click overlay to close
        const overlay = document.querySelector('.fixed.inset-0.bg-black\\/50');
        if (overlay) {
            fireEvent.click(overlay);
        }

        await waitFor(() => {
            const sidebar = document.querySelector('.fixed.left-0.top-0');
            expect(sidebar?.className).toContain('-translate-x-full');
        });
    });

    it('should change icon when menu is opened', async () => {
        render(<BurgerMenu />);
        const openButton = screen.getByLabelText('Open menu');

        // Initially should show menu icon (IoMenu)
        expect(openButton).toBeInTheDocument();

        // Click to open
        fireEvent.click(openButton);

        await waitFor(() => {
            // Close button with IoClose icon should be visible
            const closeButton = screen.getByLabelText('Close menu');
            expect(closeButton).toBeInTheDocument();
        });
    });

    it('should be positioned correctly to not overlap logo', () => {
        render(<BurgerMenu />);
        const button = screen.getByLabelText('Open menu');
        const classes = button.className;

        // Should have top-20, top-24, or top-28 to avoid logo overlap
        expect(classes).toMatch(/top-20|top-24|top-28/);
    });

    it('should have responsive width classes', () => {
        render(<BurgerMenu />);

        // Find the sidebar element
        const sidebar = document.querySelector('.fixed.left-0.top-0');
        const classes = sidebar?.className || '';

        // Should have responsive width classes
        expect(classes).toMatch(/w-full/);
        expect(classes).toMatch(/sm:w-96/);
    });
});
