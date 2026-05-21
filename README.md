# NutriTrack
Sistema de gestión ganadera desarrollado como proyecto final de la Tecnicatura Superior en Desarrollo de Software — Instituto Tecnológico El Molino, 2026.

## Tecnologías
- .NET 10 · C#
- ASP.NET Core Web API
- .NET MAUI
- Entity Framework Core
- PostgreSQL

## Requisitos previos
- Visual Studio 2022 con los workloads:
  - ASP.NET and web development
  - .NET MAUI
- Docker Desktop

## Cómo correr el proyecto
1. Clonar el repositorio
git clone https://github.com/tomasstauber/NutriTrack.git

2. Copiar `.env.example` → `.env` y completar con los valores reales

3. Copiar `appsettings.example.json` → `appsettings.json` y completar con los valores reales

4. Levantar la base de datos:
docker compose up

5. Abrir DBeaver, conectarse a la base de datos y ejecutar `db/scripts/NutriTrack-DLL.sql`

6. Abrir `NutriTrack.sln` en Visual Studio

7. Restaurar dependencias si Visual Studio no lo hace automáticamente:
dotnet restore

8. Establecer `NutriTrack.API` como proyecto de inicio y presionar F5

## Equipo
Lamerata Daniela · Stauber Tomás · Tapia Gala
