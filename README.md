# TerranovaEcommerce

A modern e-commerce platform built with React, TypeScript, and Vite.

## ✨ Frontend Highlights

- Global **Help & Feedback** widget available across all pages
- In-panel **FAQ**, **Bug Report**, and **Feature Suggestion** tabs
- Form submission flow integrated with React Query mutations and API endpoints

## 🚀 Technologies

- **Frontend**: React 19, Vite 6, Tailwind CSS 4
- **State Management**: TanStack Query (React Query)
- **Routing**: React Router
- **UI Components**: Radix UI, Lucide Icons
- **Tooling**: ESLint, Prettier, pnpm

## 📁 Project Structure

```text
TerranovaEcommerce/
├── Client/              # Frontend application
│   ├── src/
│   │   ├── app/         # App-wide providers and configuration
│   │   ├── components/  # Shared components (including shadcn/ui)
│   │   ├── features/    # Domain-driven features
│   │   ├── routes/      # Routing logic and paths
│   │   └── lib/         # External library configurations
├── docs/                # Documentation
└── memory/              # Automation and state tracking
```

## 🛠️ Development

### Prerequisites

- [Node.js](https://nodejs.org/) (v20+)
- [pnpm](https://pnpm.io/)

### Quickstart

1. Install dependencies:
   ```bash
   pnpm -C Client install
   ```

2. Start the development server:
   ```bash
   pnpm -C Client dev
   ```

3. Build for production:
   ```bash
   pnpm -C Client build
   ```

## 📚 Documentation

- [PRD Guide](docs/PRD_GUIDE.md)
- [Frontend Guide](docs/FRONTEND_GUIDE.md)
- [Testing Guide](docs/TESTING_GUIDE.md)
- [Dev Notes](docs/DEV_NOTES.md)

## 📄 License

MIT
