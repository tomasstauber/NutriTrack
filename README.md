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
- PostgreSQL instalado localmente

## Cómo correr el proyecto

1. Clonar el repositorio

git clone https://github.com/tomasstauber/NutriTrack.git

2.  Abrir `NutriTrack.sln` en Visual Studio

3. Visual Studio restaura las dependencias automáticamente. Si no lo hace, correr en la terminal:

dotnet restore

4. Copiar `appsettings.example.json` → `appsettings.json` y completar con los datos de tu base de datos local

5. Establecer `NutriTrack.API` como proyecto de inicio y presionar F5

## Equipo
Lamerata Daniela · Stauber Tomás · Tapia Gala
