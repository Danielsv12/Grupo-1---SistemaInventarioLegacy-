# Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src

# Copiar archivos de proyecto
COPY ["SistemaInventario.Domain/SistemaInventario.Domain.csproj", "SistemaInventario.Domain/"]
COPY ["SistemaInventario.Application/SistemaInventario.Application.csproj", "SistemaInventario.Application/"]
COPY ["SistemaInventario.Infrastructure/SistemaInventario.Infrastructure.csproj", "SistemaInventario.Infrastructure/"]
COPY ["SistemaInventario.Presentation/SistemaInventario.Presentation.csproj", "SistemaInventario.Presentation/"]

# Restaurar dependencias
RUN dotnet restore "SistemaInventario.Presentation/SistemaInventario.Presentation.csproj"

# Copiar código fuente completo
COPY . .

# Compilar
RUN dotnet build "SistemaInventario.Presentation/SistemaInventario.Presentation.csproj" -c Release -o /app/build

# Publicar (incluye archivos de contenido como appsettings.json)
RUN dotnet publish "SistemaInventario.Presentation/SistemaInventario.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Ejecución
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Copiar binarios y archivos de configuración publicados
COPY --from=builder /app/publish .

# Variables de entorno (se pueden sobrescribir desde docker-compose)
ENV ASPNETCORE_URLS=http://+:5000
ENV DefaultConnection="Server=sqlserver;Database=InventarioDb;User Id=sa;Password=YourPassword123!;Encrypt=false;"

ENTRYPOINT ["dotnet", "SistemaInventario.Presentation.dll"]
