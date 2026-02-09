# Terranova Ecommerce - Guía de Testing

Esta guía documenta el enfoque de testing para la aplicación Terranova Ecommerce, incluyendo pruebas unitarias, E2E y mejores prácticas.

## Índice

1. [Resumen General](#resumen-general)
2. [Pruebas Unitarias](#pruebas-unitarias)
3. [Pruebas E2E (End-to-End)](#pruebas-e2e-end-to-end)
4. [Cómo ejecutar las pruebas](#cómo-ejecutar-las-pruebas)
5. [Mejores Prácticas](#mejores-prácticas)
6. [Contribución](#contribución)

---

## Resumen General

### Estado Actual

✅ **26 pruebas unitarias** (100% pasando)  
✅ **21 pruebas E2E** (100% pasando)  
✅ **Cobertura completa** de componentes críticos  
✅ **Testing centrado en el usuario** con accesibilidad

### Tecnologías

- **Unitarias**: Vitest + React Testing Library + jsdom
- **E2E**: Playwright + Chromium
- **Enfoque**: Testing centrado en el usuario, accesible y robusto

### Cobertura de Componentes

| Componente | Tests Unitarios | Tests E2E | Estado |
|------------|----------------|-----------|--------|
| BurgerMenu | 6 | 7 | ✅ |
| CategoryManager | 20 | 14 | ✅ |
| **Total** | **26** | **21** | **✅** |

---

## Pruebas Unitarias

### Estructura de archivos

```
Client/
└── src/
    └── test/
        ├── BurgerMenu.test.tsx        # 7 tests
        ├── CategoryManager.test.tsx   # 17 tests
        └── setup.ts                   # Configuración de Vitest
```

### Configuración de Vitest

El proyecto utiliza `vite.config.ts` para la configuración, excluyendo automáticamente los tests E2E:

```typescript
// Client/vite.config.ts
test: {
  globals: true,
  environment: "jsdom",
  setupFiles: "./src/test/setup.ts",
  include: ["src/test/**/*.{test,spec}.{js,ts,jsx,tsx}"],
  exclude: ["src/test/e2e/**", "node_modules", "dist"],
},
```

### Cobertura Principal

#### BurgerMenu Component (7 tests)

**Funcionalidad y UI:**
- ✅ Renderizado inicial
- ✅ Toggle de apertura/cierre (verifica clases CSS de transformación)
- ✅ Cierre al hacer click en overlay
- ✅ Cambio de icono

**Diseño Responsivo:**
- ✅ Clases de ancho responsivas
- ✅ Posicionamiento correcto

#### CategoryManager Component (17 tests)

**Estado Inicial:**
- ✅ Renderizado con categoría por defecto
- ✅ Badge con contador de items

**Gestión de Categorías:**
- ✅ CRUD completo (Crear, Leer, Eliminar)
- ✅ Validaciones (no permite vacíos)
- ✅ Interacción con teclado (Enter)

**Gestión de Items:**
- ✅ CRUD de items dentro de categorías
- ✅ Actualización de contadores

---

## Pruebas E2E (End-to-End)

### Estructura de archivos E2E

```
Client/
└── src/
    └── test/
        └── e2e/
            ├── burger-menu.spec.ts        # Tests de navegación y UI
            └── category-manager.spec.ts   # Tests funcionales completos
```

### Casos de Prueba E2E

**Burger Menu:**
- ✅ Verificación de apertura/cierre usando animaciones CSS.
- ✅ Tests responsivos para Desktop, Laptop y Mobile.
- ✅ Validación de "no superposición" con el logo.

**Category Manager:**
- ✅ Flujos completos de usuario: Crear categoría -> Agregar item -> Borrar item -> Borrar categoría.
- ✅ Uso de selectores robustos y accesibles (`getByRole`).
- ✅ Manejo correcto de tiempos de espera (`waitForLoadState`).

---

## Cómo ejecutar las pruebas

Todas las pruebas se ejecutan desde el directorio `Client`.

### Pruebas Unitarias

```bash
cd Client
npm test              # Modo watch (desarrollo)
npm test -- --run     # Ejecución única (CI/CD)
npm run test:ui       # Interfaz visual interactiva
npm run test:coverage # Reporte de cobertura
```

### Pruebas E2E

```bash
cd Client

# Ejecutar todos los tests (headless)
npm run test:e2e

# Modo visual (ver el navegador)
npm run test:e2e:headed

# Modo UI interactivo (Recomendado para depuración)
npm run test:e2e:ui
```


**Salida esperada:**
```
Running 21 tests using 1 worker

✓ burger-menu.spec.ts:7:1 › TC-BM-001: should open burger menu
✓ burger-menu.spec.ts:15:1 › TC-BM-002: should close menu with X button
✓ burger-menu.spec.ts:23:1 › TC-BM-003: should close menu with overlay click
✓ burger-menu.spec.ts:31:1 › TC-BM-004: should not overlap with logo
✓ burger-menu.spec.ts:39:1 › TC-RD-001: responsive desktop
✓ burger-menu.spec.ts:47:1 › TC-RD-002: responsive laptop
✓ burger-menu.spec.ts:55:1 › TC-RD-003: responsive mobile

✓ category-manager.spec.ts:7:1 › TC-CM-001: default category
✓ category-manager.spec.ts:15:1 › TC-CM-002: add category
... (14 tests total)

21 passed (30.2s)
```

### Comandos Útiles

```bash
# Ver todos los tests disponibles
npx vitest list                    # Unitarios
npx playwright test --list         # E2E

# Ejecutar solo tests que fallaron
npx vitest --run --reporter=verbose --bail=1

# Actualizar snapshots (si se usan)
npx vitest -u

# Limpiar caché de Playwright
npx playwright install --force
```

---

## Mejores Prácticas

### Escritura de Tests

#### 1. Principios AAA (Arrange, Act, Assert)

```typescript
test('should add a new category', () => {
  // Arrange: Preparar el escenario
  render(<CategoryManager />);
  const input = screen.getByPlaceholderText(/nombre de la categoría/i);
  const button = screen.getByRole('button', { name: /agregar/i });
  
  // Act: Ejecutar la acción
  fireEvent.change(input, { target: { value: 'Ropa' } });
  fireEvent.click(button);
  
  // Assert: Verificar el resultado
  expect(screen.getByText('Ropa')).toBeInTheDocument();
});
```

#### 2. Usar Queries Accesibles

```typescript
// ✅ Bueno: Queries basadas en accesibilidad
screen.getByRole('button', { name: /agregar/i });
screen.getByLabelText(/nombre de la categoría/i);
screen.getByPlaceholderText(/buscar/i);

// ❌ Evitar: Queries basadas en implementación
screen.getByClassName('category-button');
screen.getByTestId('add-button');
```

#### 3. Tests Independientes

```typescript
// ✅ Bueno: Cada test es independiente
test('should add category', () => {
  render(<CategoryManager />);
  // ... test logic
});

test('should delete category', () => {
  render(<CategoryManager />);
  // ... test logic
});

// ❌ Evitar: Tests que dependen de otros
let categories;
test('should add category', () => {
  categories = addCategory('Ropa');
});
test('should delete category', () => {
  deleteCategory(categories[0]); // Depende del test anterior
});
```

#### 4. Nombres Descriptivos

```typescript
// ✅ Bueno: Nombres descriptivos
test('should prevent adding empty category when input is blank', () => {});
test('should display delete button on hover over category item', () => {});

// ❌ Evitar: Nombres vagos
test('test 1', () => {});
test('category test', () => {});
```

### Testing de Componentes Responsivos

```typescript
test('should display full width on mobile', () => {
  // Simular viewport móvil
  global.innerWidth = 375;
  global.dispatchEvent(new Event('resize'));
  
  render(<BurgerMenu />);
  
  const sidebar = screen.getByTestId('sidebar');
  expect(sidebar).toHaveClass('w-full');
});
```

### Testing de Interacciones de Usuario

```typescript
test('should add item when Enter key is pressed', () => {
  render(<CategoryManager />);
  
  const input = screen.getByPlaceholderText(/nombre del item/i);
  
  // Simular escritura y Enter
  fireEvent.change(input, { target: { value: 'Tablet' } });
  fireEvent.keyDown(input, { key: 'Enter', code: 'Enter' });
  
  expect(screen.getByText('Tablet')).toBeInTheDocument();
});
```

### Testing de Estados de Carga y Error

```typescript
test('should show loading state while fetching data', async () => {
  render(<ProductList />);
  
  // Verificar estado de carga
  expect(screen.getByText(/cargando/i)).toBeInTheDocument();
  
  // Esperar a que termine la carga
  await waitFor(() => {
    expect(screen.queryByText(/cargando/i)).not.toBeInTheDocument();
  });
});

test('should display error message when fetch fails', async () => {
  // Mock de error
  vi.spyOn(global, 'fetch').mockRejectedValueOnce(new Error('Network error'));
  
  render(<ProductList />);
  
  await waitFor(() => {
    expect(screen.getByText(/error al cargar/i)).toBeInTheDocument();
  });
});
```

---

## Contribución

### Antes de enviar PR

- ✅ Ejecutar suite completa de pruebas unitarias
- ✅ Ejecutar pruebas E2E relevantes
- ✅ Verificar que no hay logs indebidos en consola
- ✅ Actualizar documentación si es necesario
- ✅ Asegurar que todos los tests pasan al 100%

### Agregar Nuevos Tests

#### Para Tests Unitarios:

1. Crear archivo en `Tests/Unit-Tests/`
2. Seguir convención de nombres: `ComponentName.test.tsx`
3. Importar utilidades de testing:
   ```typescript
   import { render, screen, fireEvent } from '@testing-library/react';
   import { describe, it, expect } from 'vitest';
   ```
4. Agrupar tests relacionados con `describe`
5. Usar nombres descriptivos para cada `test` o `it`

#### Para Tests E2E:

1. Crear archivo en `Tests/E2E/`
2. Seguir convención de nombres: `feature-name.spec.ts`
3. Usar IDs de test case: `TC-XX-001`, `TC-XX-002`, etc.
4. Importar Playwright:
   ```typescript
   import { test, expect } from '@playwright/test';
   ```
5. Usar `test.describe` para agrupar tests relacionados
6. Agregar `test.beforeEach` para setup común

### Checklist de Calidad

- [ ] Tests tienen nombres descriptivos
- [ ] Tests son independientes entre sí
- [ ] Se usan queries accesibles (getByRole, getByLabelText)
- [ ] Se prueban casos de éxito y error
- [ ] Se prueban interacciones de usuario (click, keyboard)
- [ ] Se verifica diseño responsivo cuando aplica
- [ ] Tests E2E tienen IDs de caso de prueba (TC-XX-XXX)
- [ ] No hay `console.log` o código de debug
- [ ] Todos los tests pasan localmente

---

## Troubleshooting

### Problemas Comunes

#### Tests Unitarios Fallan

```bash
# Limpiar caché de Vitest
npx vitest --clearCache

# Reinstalar dependencias
rm -rf node_modules package-lock.json
npm install

# Verificar configuración de Vitest
npx vitest --version
```

#### Tests E2E Fallan

```bash
# Verificar que el servidor está corriendo
curl http://localhost:5173

# Reinstalar navegadores de Playwright
npx playwright install

# Limpiar estado de Playwright
npx playwright install --force

# Ejecutar en modo debug
npx playwright test --debug
```

#### Tests Lentos

```typescript
// Aumentar timeout para tests específicos
test('slow test', async ({ page }) => {
  test.setTimeout(60000); // 60 segundos
  // ... test logic
});

// Usar múltiples workers
npx playwright test --workers=5
```

---

## Métricas de Calidad

### Cobertura Actual

| Tipo | Tests | Pasando | Cobertura |
|------|-------|---------|-----------|
| Unitarios | 26 | 26 (100%) | Componentes críticos |
| E2E | 21 | 21 (100%) | Flujos principales |
| **Total** | **47** | **47 (100%)** | **Alta** |

### Objetivos de Cobertura

- ✅ Componentes UI: 100%
- 🎯 Componentes de Features: 80%+ (futuro)
- 🎯 Utilidades: 90%+ (futuro)
- ✅ Flujos críticos E2E: 100%

---

## Recursos Adicionales

### Documentación

- [Vitest Documentation](https://vitest.dev/)
- [React Testing Library](https://testing-library.com/react)
- [Playwright Documentation](https://playwright.dev/)
- [Testing Best Practices](https://kentcdodds.com/blog/common-mistakes-with-react-testing-library)

### Guías Relacionadas

- [Frontend Guide](./FRONTEND_GUIDE.md) - Desarrollo de componentes
- [DEV_NOTES.md](./DEV_NOTES.md) - Notas de desarrollo

---

**Última actualización**: Febrero 2026 - Suite de testing completamente funcional con **47 tests** (26 Unitarios + 21 E2E).
