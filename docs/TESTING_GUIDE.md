# Terranova Ecommerce - Testing Guide

This guide describes how testing is organized in the frontend and how to validate critical user flows, including the Help & Feedback widget.

## Overview

Current test suites in `Client/src/test`:

- **Unit tests (Vitest + React Testing Library)**: component-level behavior and validation
- **E2E tests (Playwright)**: browser-level flow coverage

### Current test inventory

- **Unit tests**: 30 (BurgerMenu, CategoryManager, HelpFeedbackWidget)
- **E2E tests**: 16 (Burger Menu, Category Manager)

> Note: Help & Feedback currently has unit coverage only.

---

## Directory structure

```text
Client/
└── src/
    └── test/
        ├── BurgerMenu.test.tsx
        ├── CategoryManager.test.tsx
        ├── HelpFeedbackWidget.test.tsx
        ├── setup.ts
        └── e2e/
            ├── burger-menu.spec.ts
            └── category-manager.spec.ts
```

---

## Running tests

Run commands from the `Client` directory.

### Unit tests

```bash
npm test              # watch mode
npm test -- --run     # single run (CI style)
npm run test:ui       # Vitest UI
npm run test:coverage # coverage report
```

### E2E tests

```bash
npm run test:e2e
npm run test:e2e:headed
npm run test:e2e:ui
```

---

## Help & Feedback test coverage

File: `Client/src/test/HelpFeedbackWidget.test.tsx`

Covered behaviors:

1. Renders floating action button and opens/closes the panel
2. Switches tabs (FAQ, Bug Report, Feature Suggestion)
3. Shows validation errors for required fields
4. Submits bug report payload to `/api/help-feedback/bug-report`
5. Handles pending and error UI states during submission

### Testing notes for this module

- Wrap component with `QueryClientProvider` in tests
- Stub `fetch` to assert endpoint and payload behavior
- Use accessible queries (`getByRole`, `getByLabelText`) based on ARIA labels

---

## Good testing practices

- Prefer user-centric assertions over implementation details
- Keep tests independent and deterministic
- Use accessible selectors by default
- Cover success, error, and loading states for async forms
- Add E2E tests for high-impact cross-page flows before release

---

## Pre-merge checklist

- [ ] Unit tests pass locally
- [ ] Relevant E2E suite passes locally
- [ ] No debug logs left in test files
- [ ] New frontend behavior includes test coverage
- [ ] Docs updated when behavior or architecture changes

---

**Last updated**: February 2026
