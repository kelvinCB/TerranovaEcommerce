# PRD Guide — TerranovaEcommerce

Guía estándar para documentar requisitos de producto (PRD) en TerranovaEcommerce.

## 🎯 Project Overview

- **Project Name:** TerranovaEcommerce
- **Purpose:** Plataforma e-commerce moderna para catálogo, carrito, checkout y gestión de pedidos.
- **Vision:** Entregar una experiencia rápida, segura y escalable para clientes finales y equipo interno.

## 📊 Current Feature Status

| Feature ID | Feature | Status | Completion | Owner | Notes |
|---|---|---|---:|---|---|
| TE-001 | Auth (Login/Register/Session) | In Progress | 70% | Frontend | Flujo base habilitado, falta hardening.
| TE-002 | Product Catalog + Filters | Open | 35% | Frontend | Pendiente optimización de búsqueda.
| TE-003 | Cart & Checkout UX | Open | 20% | UX/Frontend | Definir validaciones y estados de error.
| TE-004 | Orders Dashboard | Open | 10% | Backend | Dependiente de contratos API finales.

> Estados permitidos: `Open`, `In Progress`, `Review`, `Done`.

## 🗂️ Documentation Structure

Documentos principales del repositorio:

- `README.md` — visión general y quickstart
- `docs/FRONTEND_GUIDE.md` — arquitectura y convenciones frontend
- `docs/TESTING_GUIDE.md` — estrategia de pruebas
- `docs/DEV_NOTES.md` — notas técnicas y decisiones rápidas
- `docs/PRD_GUIDE.md` — (este archivo) estándar PRD para nuevas iniciativas

## 🏗️ Architecture Overview

- **Frontend:** React + TypeScript + Vite
- **Styling/UI:** Tailwind CSS + componentes UI reutilizables
- **Data Layer:** TanStack Query para caché/sync de datos
- **Routing:** React Router
- **Quality:** ESLint + Prettier + pruebas (unit/E2E según caso)

### Diagrama de alto nivel (texto)

```text
User
  └─> React UI (features/routes/components)
       └─> API Layer (queries/mutations)
            └─> Backend Services (auth/products/orders/payments)
```

## 🧭 Development Guidelines

- Mantener componentes pequeños y orientados a dominio (`features/*`).
- Definir criterios de aceptación medibles en cada PRD.
- Usar nombres explícitos en branches y PRs.
- Añadir evidencia de pruebas para todo cambio funcional.
- Mantener trazabilidad entre: **Task (Kolium) → Branch → PR → Deploy**.

## 🚀 Future Roadmap

- Mejoras de search y relevancia en catálogo
- Checkout resiliente (retry/rollback UX)
- Panel administrativo de inventario y órdenes
- Métricas de conversión y embudo
- Observabilidad (logs/alerts) para incidencias críticas

---

## 🧩 PRD Template (Copiar y completar)

```markdown
# [Feature ID] Título de la Feature

## 1) Meta
Resumen breve del objetivo de negocio/técnico.

## 2) Problema
Qué dolor resuelve y por qué importa ahora.

## 3) Alcance
- Incluye:
- No incluye:

## 4) Requisitos Funcionales
1. ...
2. ...

## 5) Requisitos No Funcionales
- Performance:
- Seguridad:
- Accesibilidad:
- Escalabilidad:

## 6) Estado y Progreso
- **Status:** Open | In Progress | Review | Done
- **Completion:** 0-100%
- **Owner:** Nombre/Rol

## 7) Criterios de Aceptación
- [ ] Criterio 1
- [ ] Criterio 2

## 8) Dependencias
Repos, servicios, APIs, decisiones previas.

## 9) Riesgos
Riesgo + mitigación.

## 10) Pruebas
- Unit:
- Integration:
- E2E:

## 11) Entregables
Artefactos esperados: código, docs, pruebas, PR.
```

---

## ✅ Ejemplo 1

### TE-005 — Feature: Wishlist persistente por usuario

- **Status:** Open
- **Completion:** 0%
- **Description:** Permitir guardar productos en wishlist sincronizada por usuario autenticado para mejorar retención.
- **Acceptance Criteria:**
  - [ ] Agregar/quitar desde card y detalle de producto.
  - [ ] Persistencia entre sesiones.
  - [ ] Vista “My Wishlist” con paginación.

## ✅ Ejemplo 2

### TE-006 — Feature: Checkout con validación de dirección + método de envío

- **Status:** In Progress
- **Completion:** 45%
- **Description:** Validar dirección y opciones de envío antes de confirmar orden para reducir errores operativos.
- **Acceptance Criteria:**
  - [ ] Validación de campos obligatorios y formato.
  - [ ] Cálculo de envío por zona.
  - [ ] Bloqueo de confirmación si faltan datos.

---

## 📌 Nota Operativa para agentes IA

Para tareas ejecutadas por IA, mantener este flujo mínimo:
1. Mover tarea a `In Progress` al iniciar.
2. Asignar responsable y estimación.
3. Trabajar en rama dedicada.
4. Crear PR a `main` con evidencia de pruebas.
5. Mover a `Review` y notificar para revisión humana.
