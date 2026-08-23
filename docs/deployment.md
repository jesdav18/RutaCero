# Despliegue Ubuntu

Instalar .NET 9 Runtime, PostgreSQL, Apache, `libapache2-mod-proxy-html`, Certbot y rsync. Crear el usuario de sistema `rutacero` y `/var/www/rutacero/{api,web}/releases` más `/var/www/rutacero/shared/storage`.

Copiar `deploy/systemd/rutacero-api.service` a `/etc/systemd/system`, crear `/etc/rutacero/rutacero.env` propiedad de root con modo `600`, ejecutar `systemctl daemon-reload` y habilitar el servicio.

Copiar `deploy/apache/rutacero.conf` a `sites-available`, reemplazar el dominio, habilitar `proxy`, `proxy_http`, `rewrite`, `ssl` y `headers`, habilitar el sitio y recargar Apache. Obtener el certificado con `certbot --apache -d finanzas.example.com`.

Configurar los secretos `VPS_HOST`, `VPS_PORT`, `VPS_USER` y `VPS_SSH_PRIVATE_KEY` en GitHub. El workflow publica artefactos, cambia symlinks, reinicia systemd y revierte automáticamente si `/health` falla.

Para rollback manual ejecutar `bash scripts/rollback-vps.sh` en el servidor. No modifica PostgreSQL.
