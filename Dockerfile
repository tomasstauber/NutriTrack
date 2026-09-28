# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar solo los .csproj primero (aprovecha el cache de Docker)
COPY NutriTrack.API/NutriTrack.API.csproj NutriTrack.API/
COPY NutriTrack.Core/NutriTrack.Core.csproj NutriTrack.Core/
COPY NutriTrack.Infraestructure/NutriTrack.Infraestructure.csproj NutriTrack.Infraestructure/

RUN dotnet restore NutriTrack.API/NutriTrack.API.csproj

# Copiar el resto del código fuente
COPY NutriTrack.API/ NutriTrack.API/
COPY NutriTrack.Core/ NutriTrack.Core/
COPY NutriTrack.Infraestructure/ NutriTrack.Infraestructure/

RUN dotnet publish NutriTrack.API/NutriTrack.API.csproj -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "NutriTrack.API.dll"]