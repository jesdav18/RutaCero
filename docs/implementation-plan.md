# Plan de implementación

## Objetivo

Construir un MVP desplegable de Ruta Cero con arquitectura hexagonal, esquema PostgreSQL manual, API autenticada y una interfaz Angular en español.

## Etapas

1. Crear solución, proyectos, referencias permitidas y pruebas de arquitectura.
2. Implementar objetos de valor y agregados financieros con pruebas unitarias.
3. Implementar casos de uso de autenticación, cuentas, deudas, pagos, efectivo disponible y recomendaciones.
4. Implementar persistencia PostgreSQL, JWT, almacenamiento privado y parsers CSV/XLSX/PDF.
5. Exponer API v1 con validación, ProblemDetails, rate limiting, Swagger y health checks.
6. Construir Angular standalone con autenticación, dashboard y módulos financieros lazy-loaded.
7. Crear esquema SQL, datos de desarrollo, documentación e infraestructura de VPS.
8. Configurar CI, publicación, despliegue y rollback.
9. Ejecutar restore, build, tests, publish y build del frontend; corregir fallos encontrados.

## Entregas verticales

- Base: autenticación, configuración personal y aislamiento por usuario.
- Operación: cuentas, saldos, transacciones, categorías, deudas y pagos.
- Planeación: presupuesto, compromisos, ingresos esperados y obligaciones.
- Inteligencia: efectivo disponible, bloqueos y recomendaciones de capital.
- Datos: importación, deduplicación, revisión y conciliación.
- Producción: Apache, systemd, HTTPS, scripts de despliegue y rollback.

## Validación

Cada módulo se considera terminado únicamente cuando compila y sus reglas críticas tienen pruebas. Las integraciones que dependan de PostgreSQL se documentarán como no verificadas si el servicio no está disponible en el entorno local.
