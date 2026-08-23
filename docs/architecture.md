# Arquitectura

Ruta Cero utiliza arquitectura hexagonal con cuatro proyectos. Domain contiene reglas y objetos de valor sin dependencias externas. Application contiene casos de uso y puertos y solo referencia Domain. Infrastructure implementa persistencia PostgreSQL, seguridad, parsers y almacenamiento. Api compone dependencias y traduce HTTP a casos de uso.

Las dependencias permitidas son `Api → Infrastructure → Application → Domain`. Api también referencia Application. El esquema se instala exclusivamente mediante SQL manual y la API nunca invoca migraciones ni creación automática.
