# Ruta Cero

Aplicación de finanzas personales para proteger liquidez y salir de deudas. Usa .NET 9, ASP.NET Core, PostgreSQL y Angular standalone.

## Desarrollo

Instalar .NET SDK 9, Node 22 o superior y PostgreSQL. Crear la base `rutacero` y solicitar el esquema vigente al responsable de infraestructura. La carpeta local `database/` se excluye deliberadamente del repositorio y no forma parte del artefacto de despliegue.

Copiar `.env.example` o `.env.production.example` únicamente como referencia. Definir `ConnectionStrings__Default`, `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`, `Storage__Root` y `Cors__Origins__0` mediante variables de entorno o user-secrets; nunca escribir valores reales en archivos versionados. La clave JWT debe tener al menos 32 bytes aleatorios.

Ejecutar la API con `dotnet run --project src/backend/RutaCero.Api`. Ejecutar Angular con `npm start --prefix src/frontend/ruta-cero-web`. Para desarrollo, configurar un proxy local de `/api` y `/health` hacia el puerto de la API.

## Validación

Ejecutar `dotnet restore RutaCero.sln`, `dotnet build RutaCero.sln -c Release`, `dotnet test RutaCero.sln -c Release`, `dotnet publish src/backend/RutaCero.Api/RutaCero.Api.csproj -c Release -o publish/api`, `npm ci --prefix src/frontend/ruta-cero-web` y `npm run build --prefix src/frontend/ruta-cero-web`.

## Producción

Consultar `docs/deployment.md` y `docs/database.md`. Los secretos viven en `/etc/rutacero/rutacero.env`; los workflows solo reciben credenciales SSH. PostgreSQL y sus cambios de esquema nunca son administrados automáticamente por el despliegue.
