# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia los archivos del proyecto
COPY ["src/Million.Api/Million.Api.csproj", "Million.Api/"]
COPY ["src/Million.Application/Million.Application.csproj", "Million.Application/"]
COPY ["src/Million.Domain/Million.Domain.csproj", "Million.Domain/"]
COPY ["src/Million.Infrastructure/Million.Infrastructure.csproj", "Million.Infrastructure/"]

# Restaura las dependencias
RUN dotnet restore "Million.Api/Million.Api.csproj"

# Copia todo el código fuente
COPY src/ .

# Construye la aplicación
RUN dotnet build "Million.Api/Million.Api.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "Million.Api/Million.Api.csproj" -c Release -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Crear usuario no-root para seguridad
RUN addgroup --system --gid 1001 dotnetgroup \
    && adduser --system --uid 1001 --gid 1001 dotnetuser

COPY --from=publish /app/publish .
RUN chown -R dotnetuser:dotnetgroup /app

# Configuración del servidor
ENV ASPNETCORE_URLS=http://+:5249
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 5249

# Cambiar a usuario no-root
USER dotnetuser

ENTRYPOINT ["dotnet", "Million.Api.dll"] 