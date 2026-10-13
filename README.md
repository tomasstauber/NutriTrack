# NutriTrack

Sistema de gestión ganadera. Backend en ASP.NET Core Web API con PostgreSQL y app cliente en .NET MAUI.

Para probarlo hacen falta dos cosas:

1. **El backend** (API + base de datos), que se levanta con Docker.
2. **La app de Windows**, que se descarga ya compilada desde el Release de GitHub.

## Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y corriendo.
- Windows 10 u 11 (64 bits) para la app.

No hace falta instalar .NET, Visual Studio ni PostgreSQL.

## Paso 1: levantar el backend

1. Cloná el repositorio.
2. Abrí una terminal en la raíz del proyecto (donde están el `Dockerfile` y el `docker-compose.yml`).
3. Corré:

   ```
   docker compose up --build
   ```

4. Esperá a que en la terminal aparezca:

   ```
   nutritrack-api  | Now listening on: http://[::]:8080
   ```

   La primera vez puede tardar unos minutos porque descarga las imágenes base y compila el proyecto. Las siguientes veces es mucho más rápido.

La API queda disponible en `http://localhost:5068`. Dejá la terminal abierta mientras uses la app.

La base arranca con los datos de prueba ya cargados (scripts de `db/scripts/`): usuarios, rodeos, animales, planes alimenticios, ingredientes, medicamentos, eventos sanitarios y pesajes. Las fechas de eventos y pesajes se calculan a partir del día en que se crea la base, así las alertas y los reportes siempre tienen datos.

## Paso 2: abrir la app

1. Entrá a la sección **Releases** del repositorio en GitHub y descargá `NutriTrack-Windows-x64.zip` de la última versión.
2. Descomprimilo en cualquier carpeta.
3. Abrí `NutriTrack.MAUI.exe`.

> **Aviso de Windows:** como la app no está firmada, la primera vez puede aparecer un cartel de SmartScreen ("Windows protegió su PC"). Hacé clic en **Más información → Ejecutar de todas formas**.

## Usuarios de prueba

| Usuario    | Contraseña  | Rol              |
|------------|-------------|------------------|
| `jperez`   | `juana1234` | Administrador    |
| `mgomez`   | `maria1234` | AsesorTecnico    |
| `pfuentes` | `pablo1234` | EncargadoDeCampo |

## Apagar y reiniciar el entorno

Para detener los contenedores sin perder los datos:

```
docker compose down
```

Para reiniciar desde cero (borra los datos y vuelve a cargar los datos de prueba la próxima vez que levantes):

```
docker compose down -v
docker compose up --build
```

Usá esta opción si algo quedó en un estado raro después de varias pruebas.

## Problemas comunes

- **"port is already allocated"**: algún otro proceso está usando el puerto 5068 o 5470. Cerralo antes de levantar el entorno. El puerto 5068 no se puede cambiar porque la app lo tiene configurado.
- **La app no conecta o no deja iniciar sesión**: revisá que el backend esté levantado (Paso 1) y que la terminal muestre `Now listening on`.

## Documentación de la API (opcional)

Con el backend levantado, la documentación interactiva de la API (Scalar) está en:

```
http://localhost:5068/scalar/
```

Los endpoints exigen sesión: primero hacé login con `POST /api/Login/login` y usá el token que devuelve.

---

## Para desarrollo

### Abrir el proyecto en Visual Studio

Alternativa a la app descargada, o para ver el código:

1. Instalá Visual Studio 2026 con .NET 10 y la carga de trabajo **Desarrollo de .NET MAUI**.
2. Abrí `NutriTrack.slnx`.
3. Elegí `NutriTrack.MAUI` como proyecto de inicio, el destino **Windows Machine** y presioná F5.

La app usa la API en `http://localhost:5068`, ya sea la de Docker o la que levantás desde Visual Studio.

### Clave JWT para correr la API sin Docker

En Docker, la clave `Jwt:Key` ya viene configurada en `docker-compose.yml` (es solo para testing). Si corrés la API desde Visual Studio o con `dotnet run`, la clave no se sube al repo: cada integrante la configura en su máquina con user secrets. El `UserSecretsId` ya está en el `.csproj`, así que **no hace falta correr `dotnet user-secrets init`**.

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

> Los user secrets solo se cargan en el entorno Development (`ASPNETCORE_ENVIRONMENT` en `launchSettings.json`).

### Generar el ejecutable de Windows

Desde la raíz de la solución:

```
dotnet publish NutriTrack.MAUI -f net10.0-windows10.0.19041.0 -c Release -p:RuntimeIdentifierOverride=win-x64 -p:SelfContained=true -p:WindowsAppSDKSelfContained=true
```

La app queda en `NutriTrack.MAUI/bin/Release/net10.0-windows10.0.19041.0/win-x64/publish/`. Es autocontenida: no necesita .NET instalado. Comprimí esa carpeta como `NutriTrack-Windows-x64.zip` y adjuntala a un Release de GitHub. El `.zip` no se commitea.
