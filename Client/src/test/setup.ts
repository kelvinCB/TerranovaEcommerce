import '@testing-library/jest-dom';
import { vi } from 'vitest';

// Mock de react-icons
vi.mock('react-icons/io5', () => ({
    IoMenu: () => 'Menu Icon',
    IoClose: () => 'Close Icon',
    IoChevronDown: () => 'Chevron Down',
    IoChevronUp: () => 'Chevron Up',
    IoTrash: () => 'Trash Icon',
    IoAdd: () => 'Add Icon',
}));
