# Frontend Development Guide - Terranova Ecommerce

## Overview

This guide provides comprehensive documentation for frontend development in the Terranova Ecommerce application using React, TypeScript, Vite, and Tailwind CSS.

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Project Structure](#project-structure)
- [Component Development](#component-development)
- [State Management](#state-management)
- [Styling Guidelines](#styling-guidelines)
- [Routing and Navigation](#routing-and-navigation)
- [Help & Feedback Module](#help--feedback-module)
- [Testing Guidelines](#testing-guidelines)
- [Performance Optimization](#performance-optimization)
- [Accessibility Standards](#accessibility-standards)
- [Build and Deployment](#build-and-deployment)
- [Troubleshooting](#troubleshooting)

---

## Architecture Overview

### Tech Stack

- **Framework**: React 18+ with TypeScript
- **Build Tool**: Vite for fast development and optimized builds
- **Styling**: Tailwind CSS for utility-first styling
- **State Management**: React Hooks + Context API
- **Routing**: React Router DOM v6
- **Icons**: Lucide React for consistent iconography
- **Testing**: Vitest + React Testing Library + Playwright

### Design Principles

- **Component-based architecture** with reusable UI components
- **TypeScript-first** development for type safety
- **Responsive design** with mobile-first approach
- **Accessibility** as a core requirement
- **Performance optimization** built-in from the start
- **Clean separation** between UI, business logic, and data layers

---

## Project Structure

### Current Directory Structure

```
Client/
├── src/
│   ├── components/          # Reusable UI components
│   │   ├── ui/             # Basic UI components
│   │   │   ├── AppLayout.tsx      # Main layout wrapper
│   │   │   ├── BurgerMenu.tsx     # Sidebar navigation menu
│   │   │   ├── CategoryManager.tsx # Category CRUD component
│   │   │   ├── Navbar.tsx         # Top navigation bar
│   │   │   └── ...
│   │   └── ...
│   ├── features/           # Feature-specific components
│   │   ├── home/          # Homepage components
│   │   ├── products/      # Product-related components
│   │   └── ...
│   ├── interfaces/         # TypeScript type definitions
│   │   ├── Category.ts    # Category interfaces
│   │   └── Product.ts     # Product interfaces
│   ├── lib/               # Third-party integrations
│   │   └── utils.ts       # Utility functions (cn helper)
│   ├── routes/            # Route definitions
│   │   └── AppRouter.tsx  # Main router configuration
│   ├── utils/             # Utility functions
│   │   └── ...
│   ├── App.tsx            # Main application component
│   ├── main.tsx           # Application entry point
│   └── index.css          # Global styles and Tailwind imports
├── Tests/
│   ├── Unit-Tests/        # Vitest unit tests
│   │   ├── BurgerMenu.test.tsx
│   │   ├── CategoryManager.test.tsx
│   │   └── ...
│   └── E2E/               # Playwright E2E tests
│       ├── burger-menu.spec.ts
│       ├── category-manager.spec.ts
│       └── ...
├── playwright.config.ts   # Playwright configuration
├── vite.config.ts         # Vite configuration
├── tailwind.config.js     # Tailwind CSS configuration
└── package.json           # Dependencies and scripts
```

---

## Component Development

### Component Architecture

#### Component Types

1. **UI Components** (`src/components/ui/`): Basic, reusable components
   - Buttons, inputs, modals, navigation elements
   - Example: `BurgerMenu.tsx`, `CategoryManager.tsx`

2. **Feature Components** (`src/features/`): Business logic components
   - Product displays, shopping cart, checkout
   - Example: Product cards, category displays

3. **Layout Components**: Page structure components
   - `AppLayout.tsx`, `Navbar.tsx`

### Component Structure Template

```typescript
// ComponentName.tsx
import React, { useState } from 'react';

interface ComponentNameProps {
  // Define props with proper types
  title: string;
  isActive?: boolean;
  onAction?: (id: string) => void;
}

export const ComponentName: React.FC<ComponentNameProps> = ({
  title,
  isActive = false,
  onAction
}) => {
  const [localState, setLocalState] = useState<boolean>(false);

  const handleClick = () => {
    if (onAction) {
      onAction('some-id');
    }
  };

  return (
    <div className="component-base">
      <h2>{title}</h2>
      <button onClick={handleClick}>
        {isActive ? 'Active' : 'Inactive'}
      </button>
    </div>
  );
};
```

### Component Best Practices

- **TypeScript interfaces** for all props
- **Default props** using ES6 default parameters
- **Descriptive prop names** that indicate purpose
- **Consistent export patterns** (named exports preferred)
- **Responsive design** considerations
- **Accessibility** attributes (aria-labels, roles)

### Existing Key Components

#### BurgerMenu Component

- **Purpose**: Responsive sidebar navigation menu
- **Features**: 
  - Toggle open/close functionality
  - Overlay with click-to-close
  - Responsive widths (full mobile, 384px tablet, 448px laptop, 512px desktop)
  - Integrates CategoryManager
- **Props**: None (self-contained state)
- **Location**: `src/components/ui/BurgerMenu.tsx`

#### CategoryManager Component

- **Purpose**: Product category management with CRUD operations
- **Features**:
  - Add, delete, expand/collapse categories
  - Add, delete items within categories
  - Item count badges
  - Responsive design
  - Hover effects on delete buttons
- **Props**: None (self-contained state)
- **Location**: `src/components/ui/CategoryManager.tsx`

#### AppLayout Component

- **Purpose**: Main application layout wrapper
- **Features**:
  - Integrates Navbar and BurgerMenu
  - Provides consistent layout structure
  - Renders child routes via `<Outlet />`
  - Mounts the Help & Feedback floating widget globally
- **Location**: `src/components/ui/AppLayout.tsx`

---

## Help & Feedback Module

### What it does

The Help & Feedback module provides a floating support entry point available across the app. It combines:

- **FAQ tab** for quick self-service answers
- **Bug Report tab** with validated form submission
- **Feature Suggestion tab** with validated form submission

This module is implemented as `HelpFeedbackWidget` and is rendered in `AppLayout`, so users can open it from any page.

### Location and structure

```text
Client/src/features/help-feedback/
├── api/helpFeedbackService.ts
├── components/HelpFeedbackWidget.tsx
├── hooks/useHelpFeedbackSubmission.ts
└── types.ts
```

### Interaction model

- Opens from a floating action button (`aria-label="Open help and feedback"`)
- Displays a right-side panel with tabbed content
- Uses `react-hook-form` for client-side validation
- Uses `@tanstack/react-query` mutation for submit states (`pending`, `success`, `error`)

### API integration

The widget submits to these endpoints:

- `POST /api/help-feedback/bug-report`
- `POST /api/help-feedback/feature-suggestion`

Implementation detail: endpoint mapping lives in `helpFeedbackService.ts` (`HELP_FEEDBACK_ENDPOINTS`).

### Accessibility notes

- Dialog semantics: `role="dialog"`, `aria-modal="true"`
- Tab controls include `role="tab"` and selection state via `aria-selected`
- Form fields are label-associated (`htmlFor` / `id`) and queryable by accessible names in tests

---

## State Management

### Current State Architecture

#### Local State with useState

```typescript
// Simple component state
const [isOpen, setIsOpen] = useState<boolean>(false);
const [categories, setCategories] = useState<Category[]>([]);
```

#### State Structure Guidelines

- **Normalize data** to avoid deep nesting
- **Separate concerns** (UI state vs. data state)
- **Use TypeScript** for state shape definitions
- **Immutable updates** for all state changes

#### Example: Category State Management

```typescript
interface Category {
  id: string;
  name: string;
  items: string[];
  isExpanded: boolean;
}

const [categories, setCategories] = useState<Category[]>([
  {
    id: '1',
    name: 'Electrónicos',
    items: ['Laptops', 'Celulares', 'PC Torres'],
    isExpanded: true
  }
]);

// Adding a category
const addCategory = (name: string) => {
  const newCategory: Category = {
    id: Date.now().toString(),
    name,
    items: [],
    isExpanded: true
  };
  setCategories(prev => [...prev, newCategory]);
};

// Deleting a category
const deleteCategory = (id: string) => {
  setCategories(prev => prev.filter(cat => cat.id !== id));
};
```

### When to Use Each Pattern

- **useState**: Simple component-local state
- **useReducer**: Complex state with multiple actions (future consideration)
- **Context API**: Global state (theme, auth, cart - future)
- **Local Storage**: Persistent client-side data (future)

---

## Styling Guidelines

### Tailwind CSS Architecture

#### Utility-First Approach

```tsx
// Good: Utility classes
<button className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors">
  Click me
</button>

// Avoid: Custom CSS classes when utilities suffice
<button className="custom-button">
  Click me
</button>
```

#### Component Variants Pattern

```tsx
// Button variant system
const buttonVariants = {
  primary: 'bg-blue-500 hover:bg-blue-600 text-white',
  secondary: 'bg-gray-200 hover:bg-gray-300 text-gray-900',
  danger: 'bg-red-500 hover:bg-red-600 text-white'
};

interface ButtonProps {
  variant?: keyof typeof buttonVariants;
  children: React.ReactNode;
}

export const Button: React.FC<ButtonProps> = ({ 
  variant = 'primary', 
  children 
}) => {
  return (
    <button className={`px-4 py-2 rounded-lg ${buttonVariants[variant]}`}>
      {children}
    </button>
  );
};
```

### Responsive Design

#### Breakpoint Strategy

```typescript
// Tailwind breakpoints
const breakpoints = {
  sm: '640px',   // Small devices
  md: '768px',   // Medium devices
  lg: '1024px',  // Large devices
  xl: '1280px',  // Extra large devices
  '2xl': '1536px' // 2X large devices
};
```

#### Mobile-First Approach

```tsx
// Mobile-first responsive classes
<div className="
  w-full p-4
  sm:w-1/2 sm:p-6
  md:w-1/3 md:p-8
  lg:w-1/4
">
  Content
</div>
```

### Color System

```tsx
// Consistent color usage
const colors = {
  primary: 'bg-blue-500',
  secondary: 'bg-gray-500',
  success: 'bg-green-500',
  danger: 'bg-red-500',
  warning: 'bg-yellow-500'
};
```

### Utility Helper (cn)

```typescript
// Using the cn utility for conditional classes
import { cn } from '@/lib/utils';

<div className={cn(
  'base-classes',
  isActive && 'active-classes',
  isDisabled && 'disabled-classes'
)}>
  Content
</div>
```

---

## Routing and Navigation

### React Router Setup

```typescript
// routes/AppRouter.tsx
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import AppLayout from '@/components/ui/AppLayout';
import HomePage from '@/features/home/HomePage';

const router = createBrowserRouter([
  {
    path: '/',
    element: <AppLayout />,
    children: [
      {
        index: true,
        element: <HomePage />
      },
      {
        path: 'products',
        element: <ProductsPage />
      },
      // ... more routes
    ]
  }
]);

export const AppRouter = () => <RouterProvider router={router} />;
```

### Navigation Best Practices

- Use `<Link>` from react-router-dom for internal navigation
- Use `useNavigate` hook for programmatic navigation
- Implement loading states for route transitions
- Handle 404 pages with a catch-all route

---

## Testing Guidelines

### Unit Testing with Vitest

#### Component Testing Best Practices

- **Test user behavior** rather than implementation details
- **Use accessible queries** (getByRole, getByLabelText)
- **Test different states** (loading, error, success)
- **Keep tests independent** and clean up after each test

#### Example Component Test

```typescript
// BurgerMenu.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { BurgerMenu } from '../BurgerMenu';

describe('BurgerMenu', () => {
  it('should toggle menu when button is clicked', () => {
    render(<BurgerMenu />);
    
    const button = screen.getByRole('button', { name: /open menu/i });
    fireEvent.click(button);
    
    expect(screen.getByText(/gestión de categorías/i)).toBeInTheDocument();
  });

  it('should close menu when overlay is clicked', () => {
    render(<BurgerMenu />);
    
    // Open menu
    const button = screen.getByRole('button', { name: /open menu/i });
    fireEvent.click(button);
    
    // Click overlay
    const overlay = screen.getByTestId('overlay');
    fireEvent.click(overlay);
    
    expect(screen.queryByText(/gestión de categorías/i)).not.toBeInTheDocument();
  });
});
```

### E2E Testing with Playwright

#### E2E Test Structure

```typescript
// burger-menu.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Burger Menu', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:5173');
  });

  test('TC-BM-001: should open burger menu when button is clicked', async ({ page }) => {
    await page.click('[aria-label="Open menu"]');
    await expect(page.locator('text=Gestión de Categorías')).toBeVisible();
  });

  test('TC-BM-002: should close menu when X button is clicked', async ({ page }) => {
    await page.click('[aria-label="Open menu"]');
    await page.click('[aria-label="Close menu"]');
    await expect(page.locator('text=Gestión de Categorías')).not.toBeVisible();
  });
});
```

#### Running Tests

```bash
# Unit tests
npm test                    # Watch mode
npm run test:coverage       # With coverage

# E2E tests
npm run test:e2e           # Headless mode
npm run test:e2e:headed    # With browser visible
npm run test:e2e:ui        # Interactive UI mode
```

---

## Performance Optimization

### React Performance

#### Component Optimization

```typescript
// React.memo for expensive components
export const ExpensiveComponent = React.memo<Props>(({ data }) => {
  return <div>{/* Complex rendering */}</div>;
});

// useMemo for expensive calculations
const expensiveValue = useMemo(() => {
  return heavyCalculation(data);
}, [data]);

// useCallback for stable function references
const handleClick = useCallback((id: string) => {
  onItemClick(id);
}, [onItemClick]);
```

#### List Optimization

```typescript
// Use keys for list rendering
{categories.map(category => (
  <CategoryItem 
    key={category.id} 
    category={category} 
  />
))}
```

### Bundle Optimization

#### Code Splitting

```typescript
// Route-based code splitting
const ProductsPage = lazy(() => import('./features/products/ProductsPage'));
const CartPage = lazy(() => import('./features/cart/CartPage'));

// Component-based code splitting
const HeavyComponent = lazy(() => 
  import('./components/HeavyComponent').then(module => ({
    default: module.HeavyComponent
  }))
);
```

#### Tree Shaking

```typescript
// Good: Named imports for tree shaking
import { format, addDays } from 'date-fns';

// Avoid: Default imports that include entire library
import * as dateFns from 'date-fns';
```

---

## Accessibility Standards

### ARIA and Semantic HTML

#### Semantic Structure

```tsx
// Good: Semantic HTML elements
<main>
  <section aria-labelledby="products-heading">
    <h2 id="products-heading">Products</h2>
    <article>
      <h3>Product Name</h3>
      <p>Product description</p>
    </article>
  </section>
</main>
```

#### ARIA Labels and Roles

```tsx
// Button with descriptive label
<button 
  aria-label="Delete category: Electronics"
  onClick={() => onDelete(category.id)}
>
  <TrashIcon />
</button>

// Interactive elements with proper roles
<div 
  role="button"
  tabIndex={0}
  aria-pressed={isActive}
  onKeyDown={handleKeyDown}
  onClick={handleClick}
>
  Custom Button
</div>
```

### Keyboard Navigation

```typescript
// Keyboard event handling
const handleKeyDown = (event: KeyboardEvent) => {
  if (event.key === 'Enter' || event.key === ' ') {
    event.preventDefault();
    handleClick();
  }
  if (event.key === 'Escape') {
    handleClose();
  }
};
```

---

## Build and Deployment

### Vite Configuration

```typescript
// vite.config.ts
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    open: true,
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom'],
          router: ['react-router-dom'],
        },
      },
    },
  },
});
```

### Build Commands

```bash
# Development
npm run dev

# Production build
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint
```

---

## Troubleshooting

### Common Issues

#### Build Errors

```bash
# Clear node modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Clear Vite cache
npx vite --force

# Check TypeScript compilation
npx tsc --noEmit
```

#### Performance Issues

```typescript
// Debug re-renders with React DevTools
// Check for unnecessary re-renders in components
// Use React.memo, useMemo, useCallback appropriately
```

#### Styling Issues

```bash
# Rebuild Tailwind
npm run build

# Check for conflicting styles in browser dev tools
# Verify Tailwind classes are being applied correctly
```

---

## Best Practices Summary

### Code Quality

- ✅ Use TypeScript for all components
- ✅ Write unit tests for components
- ✅ Write E2E tests for critical user flows
- ✅ Use ESLint and Prettier for code formatting
- ✅ Follow component naming conventions
- ✅ Document complex logic with comments

### Performance

- ✅ Lazy load routes and heavy components
- ✅ Optimize images and assets
- ✅ Use React.memo for expensive components
- ✅ Minimize bundle size with tree shaking

### Accessibility

- ✅ Use semantic HTML
- ✅ Add ARIA labels where needed
- ✅ Support keyboard navigation
- ✅ Test with screen readers
- ✅ Ensure sufficient color contrast

### Testing

- ✅ Write tests before or alongside features
- ✅ Aim for high test coverage
- ✅ Test user behavior, not implementation
- ✅ Keep tests independent and fast

---

**Last Updated**: February 2026

**References**:
- [Testing Guide](./TESTING_GUIDE.md) for detailed testing strategies
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [React Documentation](https://react.dev)
- [Vite Documentation](https://vitejs.dev)
