# API

La API usa `/api/v1`, JWT Bearer y ProblemDetails. Swagger se expone en `/swagger` y el health check en `/health`. Los recursos financieros obtienen el usuario desde el claim `NameIdentifier`; los identificadores del request nunca sustituyen esa identidad.

Los módulos disponibles incluyen autenticación, cuentas, snapshots, transacciones, categorías, deudas, pagos, obligaciones, ingresos, compromisos, presupuestos, configuración, importaciones, conciliación, recomendaciones, notificaciones y dashboard.
