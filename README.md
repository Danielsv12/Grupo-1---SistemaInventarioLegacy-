# Guía Rápida de Inicio

## 🚀 Inicio Rápido (5 minutos)

### 1. Requisitos
- .NET 8 SDK: https://dotnet.microsoft.com/download
- Docker (opcional, para SQL Server)
- Git

### 2. Clonar y Compilar
```bash
git clone <repo>
cd SistemaInventarioRefactorizado
dotnet restore
dotnet build
```

### 3. Ejecutar Tests
```bash
dotnet test
# Resultado esperado: 10 tests pasados
```

### 4. Ejecutar Aplicación (con BD local)

**Opción A: Con SQL Server en Docker**
```bash
docker-compose up -d
# La app se iniciará automáticamente con migraciones
```

**Opción B: Configurar manualmente**
```bash
# 1. Instalar SQL Server Express
# 2. Actualizar connection string en appsettings.json
# 3. Ejecutar migraciones:
dotnet ef database update -p SistemaInventario.Infrastructure
# 4. Ejecutar aplicación
dotnet run --project SistemaInventario.Presentation
```

---

## 📚 Estructura del Proyecto

```
SistemaInventarioRefactorizado/
├── 📦 SistemaInventario.Domain/              # Lógica de negocio pura
│   ├── Entities/                           # Producto, Movimiento, Categoria
│   ├── Repositories/                       # Interfaces IProductoRepository
│   └── Exceptions/                         # DomainException
│
├── 📦 SistemaInventario.Application/        # Casos de uso
│   ├── UseCases/                           # RegistrarProductoUseCase
│   ├── DTOs/                               # ProductoDTO, MovimientoDTO
│   └── Mappers/                            # AutoMapper profiles
│
├── 📦 SistemaInventario.Infrastructure/     # Implementaciones técnicas
│   ├── Data/                               # InventarioDbContext
│   ├── Repositories/                       # ProductoRepository
│   └── Persistence/                        # DataSeeder
│
├── 📦 SistemaInventario.Presentation/       # Interfaz de usuario (CLI)
│   ├── ConsoleUI/                          # MenuPrincipal, MenuProductos
│   └── appsettings.json                    # Configuración
│
├── 🧪 SistemaInventario.Tests/              # Pruebas unitarias
│   └── Unit/
│       └── Domain/                         # ProductoTests
│
├── Dockerfile                              # Containerización
├── docker-compose.yml                      # Orquestación
└── docs/
    ├── ARCHITECTURE.md                     # Guía de arquitectura
    └── DIAGNOSTICO_SONARQUBE.md            # Análisis de calidad
```

---

## 🧪 Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Solo Domain
dotnet test --filter "FullyQualifiedName~SistemaInventario.Tests.Unit.Domain"

# Con cobertura (instalar: dotnet tool install -g dotnet-reportgenerator-globaltool)
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
reportgenerator -reports:"**/coverage.opencover.xml" -targetdir:"coveragereport"
```

---

## 🔧 Agregar Funcionalidades

### Agregar Nuevo UseCase

1. **Crear interfaz**:
```csharp
// Application/UseCases/Productos/IObtenerProductoUseCase.cs
public interface IObtenerProductoUseCase {
    Task<ProductoDTO> EjecutarAsync(int productoId);
}
```

2. **Implementar**:
```csharp
public class ObtenerProductoUseCase : IObtenerProductoUseCase {
    public async Task<ProductoDTO> EjecutarAsync(int productoId) {
        var producto = await _unitOfWork.Productos.ObtenerPorIdAsync(productoId);
        if (producto == null) throw new ProductoNoEncontradoException(productoId);
        return _mapper.Map<ProductoDTO>(producto);
    }
}
```

3. **Registrar en DI**:
```csharp
// Application/DependencyInjection.cs
services.AddScoped<IObtenerProductoUseCase, ObtenerProductoUseCase>();
```

4. **Usar en CLI**:
```csharp
var useCase = _serviceProvider.GetRequiredService<IObtenerProductoUseCase>();
var resultado = await useCase.EjecutarAsync(productId);
```

---

## 🐳 Docker Cheat Sheet

```bash
# Construir imagen
docker build -t inventario-sistema:latest .

# Ejecutar contenedor
docker run -p 5000:5000 inventario-sistema:latest

# Con compose
docker-compose up -d              # Iniciar
docker-compose logs -f            # Ver logs
docker-compose down               # Detener

# SQL Server standalone
docker run -e ACCEPT_EULA=Y \
           -e SA_PASSWORD=YourPassword123! \
           -p 1433:1433 \
           mcr.microsoft.com/mssql/server:latest
```

---

## 🔐 Gestión de Secretos

**NO hacer**:
```csharp
string conn = "Server=localhost;User Id=admin;Password=MyPassword123";
```

**SÍ hacer**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "${DB_CONNECTION_STRING}"
  }
}
```

```bash
export DB_CONNECTION_STRING="Server=localhost;..."
dotnet run
```

---

## 📊 Analizar Calidad con SonarQube

```bash
# 1. Iniciar SonarQube
docker run -d -p 9000:9000 sonarqube:latest

# 2. Instalar scanner
dotnet tool install --global dotnet-sonarscanner

# 3. Analizar proyecto
dotnet sonarscanner begin /k:Inventario /d:sonar.host.url=http://localhost:9000
dotnet build
dotnet sonarscanner end

# 4. Ver resultados
# http://localhost:9000/dashboard?id=Inventario
```

---

## 🚨 Troubleshooting

### Error: "SQL Server connection failed"
```bash
# Verificar que SQL Server está corriendo
docker ps | grep mssql

# Si no está, iniciar compose
docker-compose up sqlserver -d
```

### Error: "Migration pending"
```bash
# Aplicar migraciones manualmente
dotnet ef database update -p SistemaInventario.Infrastructure
```

### Error: "Port 1433/5000 already in use"
```bash
# Cambiar puerto en docker-compose.yml
# sqlserver: "1434:1433"  (cambiar primer puerto)
```

---

## 📖 Recursos

- **Arquitectura**: [docs/ARCHITECTURE.md](./docs/ARCHITECTURE.md)
- **Diagnóstico**: [docs/DIAGNOSTICO_SONARQUBE.md](./docs/DIAGNOSTICO_SONARQUBE.md)
- **.NET 8 Docs**: https://docs.microsoft.com/en-us/dotnet/
- **Entity Framework**: https://docs.microsoft.com/en-us/ef/core/
- **Clean Architecture**: Martin, Robert C. "Clean Architecture"

---

**Última actualización**: Agosto 2026
