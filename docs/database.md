# Base de datos

La carpeta local `database/` está excluida del repositorio. El esquema y los datos de desarrollo deben obtenerse mediante el proceso de infraestructura autorizado; no se distribuyen dentro del código fuente ni de los artefactos.

Antes de cada cambio productivo, crear un respaldo con `pg_dump --format=custom`, revisar el SQL, verificar su SHA-256, aplicarlo manualmente dentro de una transacción cuando sea posible y registrar versión, nombre, fecha y checksum en `database_versions`.

Los despliegues y rollbacks de código nunca ejecutan scripts SQL. Cualquier script destructivo debe utilizarse únicamente en una base local desechable y después de verificar explícitamente el destino.
