# NutriTrack — Entorno de testing con Docker

Este entorno levanta el backend completo de NutriTrack (API + base de datos PostgreSQL) para pruebas cruzadas, con los datos de prueba ya cargados.

## Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y corriendo.
- No hace falta tener instalado .NET SDK ni PostgreSQL en tu máquina — Docker se encarga de todo.

## Cómo levantar el entorno

1. Cloná el repositorio (o asegurate de tener la última versión de la rama correspondiente).
2. Abrí una terminal en la raíz del proyecto (donde están el `Dockerfile`, el `docker-compose.yml` y `backupOk.sql`).
3. Corré:

   ```
   docker compose up --build
   ```

4. Esperá a que en la terminal aparezca:

   ```
   nutritrack-api  | Now listening on: http://[::]:8080
   nutritrack-db   | database system is ready to accept connections
   ```

   La primera vez puede tardar unos minutos porque descarga las imágenes base y compila el proyecto. Las siguientes veces es mucho más rápido.

## Cómo probar los endpoints

Con el entorno arriba, abrí en el navegador:

```
http://localhost:8080/scalar/
```

Ahí vas a ver la documentación interactiva de la API (Scalar). Elegí el endpoint, presioná **"Test Request"**, pegá el JSON correspondiente de la columna "Datos de Prueba" del documento de casos de prueba, y presioná **Send**.

## Datos de prueba ya cargados

La base arranca automáticamente con los datos definidos en `backupOk.sql` — no hace falta cargar nada a mano. Incluye animales, rodeos, planes alimenticios, ingredientes, usuarios y eventos sanitarios de ejemplo, con los mismos IDs que se usan en el documento de casos de prueba.

## Cómo apagar el entorno

Para detener los contenedores sin perder los datos cargados:

```
docker compose down
```

Para reiniciar completamente desde cero (borra los datos y vuelve a cargar el seed original la próxima vez que levantes):

```
docker compose down -v
docker compose up --build
```

Usá esta última opción si algo quedó en un estado raro después de correr varios casos de prueba y querés volver al punto de partida.

## Problemas comunes

- **"port is already allocated"**: algún otro proceso o contenedor está usando el puerto 5470 u 8080 en tu máquina. Cerralo o cambiá el puerto en `docker-compose.yml`.
- **La API no conecta a la base al arrancar**: esperá unos segundos más — Postgres necesita un momento para inicializarse la primera vez. El `docker-compose.yml` ya tiene un healthcheck configurado para que la API espere a que la base esté lista.

## Configuración local: clave JWT

La API necesita la clave `Jwt:Key` para firmar los tokens. No se sube al repo: cada integrante la configura en su máquina con user secrets. El `UserSecretsId` ya está en el `.csproj`, así que **no hace falta correr `dotnet user-secrets init`**.

Desde la raíz de la solución:

1. Generar una clave aleatoria (PowerShell). Debe tener al menos 32 caracteres; este comando genera 64:
```
   [Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Maximum 256 }))
```
2. Guardarla:
```
   dotnet user-secrets set "Jwt:Key" "<clave generada>" --project NutriTrack.API
```
3. Verificar:
```
   dotnet user-secrets list --project NutriTrack.API
```
4. Levantar la API y probar el login en Scalar.

> Los user secrets solo se cargan en el entorno Development (`ASPNETCORE_ENVIRONMENT` en `launchSettings.json`).
