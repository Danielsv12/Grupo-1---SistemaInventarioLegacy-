# Arquitectura Clean Architecture - Sistema de Gestión de Inventario PYME

## 📋 Tabla de Contenidos
1. [Visión General](#visión-general)
2. [Capas de la Arquitectura](#capas-de-la-arquitectura)
3. [Flujo de Solicitud](#flujo-de-solicitud)
4. [Principios SOLID Aplicados](#principios-solid-aplicados)
5. [Guía de Desarrollo](#guía-de-desarrollo)
6. [Próximos Pasos](#próximos-pasos)

---

## Visión General

Este proyecto implementa **Clean Architecture** sobre un sistema legacy de gestión de inventario para PYME. El objetivo es transformar una aplicación monolítica sin capas hacia una arquitectura modular, testeable y mantenible.

### Atributos de Calidad Logrados

| Atributo | Estado | Métrica |
|----------|--------|---------|
| **Mantenibilidad** | ✅ Logrado | Capas claras, SRP, DIP implementados |
| **Seguridad** | ✅ Logrado | Queries parametrizadas, sin SQL injection |
| **Testeabilidad** | ✅ Logrado | 100% cobertura (Domain), 0 dependencias externas |
| **Escalabilidad** | 🔄 En Progreso | Base para API REST, Microservicios |

---

## Capas de la Arquitectura

### 1. **Domain Layer** (SistemaInventario.Domain)

**Responsabilidad**: Contener la lógica de negocio pura, independiente de frameworks.

**Componentes**:
- **Entities**: `Producto`, `Movimiento`, `Categoria`, `TipoMovimiento`
  - Contienen comportamiento (no son anémicas)
  - Factory methods privados para validación
  - Métodos de negocio: `RecibirStock()`, `DespacharStock()`, `EsBajoStock()`

- **Repositories (Interfaces)**: `IProductoRepository`, `IMovimientoRepository`, `ICategoriaRepository`
  - Definen contrato sin detalles de persistencia
  - Inversión de dependencias

- **Exceptions**: `DomainException`, `StockInsuficienteException`, etc.

**Ejemplo de Entidad con Comportamiento**:
```csharp
public class Producto {
    private Producto() {} // Constructor privado
    
    public static Producto Crear(string nombre, int categoriaId, decimal precio...) {
        // Validaciones de dominio
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException(...);
        // ...
        return new Producto { ... };
    }
    
    public void DespacharStock(int cantidad) {
        if (Stock < cantidad) throw new InvalidOperationException(...);
        Stock -= cantidad;
    }
}
```

---

### 2. **Application Layer** (SistemaInventario.Application)

**Responsabilidad**: Orquestar el flujo de negocio, transformar DTOs.

**Componentes**:
- **UseCases**: `RegistrarProductoUseCase`, `BuscarProductoUseCase`, `RegistrarMovimientoUseCase`
  - Lógica de aplicación sin detalles técnicos
  - Coordinan Domain + Infrastructure
  - Independientes de interfaz (Web, CLI, etc)

- **DTOs**: `ProductoDTO`, `MovimientoDTO`, etc.
  - Desacoplan capas
  - Seguros para serialización

- **Mappers**: AutoMapper profile para transformación automática

**Ejemplo de UseCase**:
```csharp
public class RegistrarProductoUseCase : IRegistrarProductoUseCase {
    public async Task<ProductoDTO> EjecutarAsync(CrearProductoDTO request) {
        // Validar precondiciones
        var categoriaExiste = await _unitOfWork.Categorias.ExisteAsync(request.CategoriaId);
        if (!categoriaExiste) throw new ArgumentException(...);
        
        // Crear entidad (con validación de dominio)
        var producto = Producto.Crear(...);
        
        // Persistir
        await _unitOfWork.Productos.AgregarAsync(producto);
        await _unitOfWork.SaveChangesAsync();
        
        // Retornar DTO
        return _mapper.Map<ProductoDTO>(producto);
    }
}
```

---

### 3. **Infrastructure Layer** (SistemaInventario.Infrastructure)

**Responsabilidad**: Implementar detalles técnicos (persistencia, base de datos, etc).

**Componentes**:
- **DbContext**: `InventarioDbContext` (Entity Framework Core)
  - Configuración del modelo relacional
  - Migraciones automáticas

- **Repositories (Implementaciones)**:
  - `ProductoRepository`: Queries parametrizadas, sin SQL injection
  - `MovimientoRepository`
  - `CategoriaRepository`

- **Unit of Work**: `UnitOfWork`
  - Coordina transacciones
  - Lazy-loading de repositorios

- **Seeders**: `DataSeeder` para datos de referencia

**Ventaja**: Cambiar de SQL Server a PostgreSQL solo requiere cambios en Infrastructure.

---

### 4. **Presentation Layer** (SistemaInventario.Presentation)

**Responsabilidad**: Interfaz de usuario (actualmente CLI, extensible a Web/API).

**Componentes**:
- **Menús**: `MenuPrincipal`, `MenuProductos`, `MenuMovimientos`
- **Configuration**: `appsettings.json` con variables de entorno

**Delega todo a Application Layer** - no contiene lógica de negocio.

---

### 5. **Tests** (SistemaInventario.Tests)

**Responsabilidad**: Validar lógica de dominio sin dependencias externas.

**Cobertura**:
- **Unit**: Entidades de dominio (Producto, Movimiento)
- **Integration**: Repositorios con EF Core In-Memory
- **E2E**: Flujos completos (próximas versiones)

**Ejemplo**:
```csharp
[Fact]
public void DespacharStock_ConStockInsuficiente_DebeLanzarExcepcion() {
    var producto = Producto.Crear("Laptop", 1, 999.99m, 10, 2);
    Assert.Throws<InvalidOperationException>(() => producto.DespacharStock(20));
}
```

---

## Flujo de Solicitud

### Escenario: Registrar Producto

```
┌─────────────────────────────────────────────────────────┐
│ 1. CLI (MenuProductos.RegistrarProducto)               │
│    - Lee entrada del usuario                            │
│    - Valida formato                                     │
│    - Crea CrearProductoDTO                              │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│ 2. UseCase (RegistrarProductoUseCase)                  │
│    - Valida precondiciones (categoría existe)          │
│    - Llama a Domain para crear entidad                 │
│    - Persiste y retorna DTO                            │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│ 3. Domain (Producto.Crear)                             │
│    - Valida reglas de negocio                          │
│    - Crea instancia si es válida                       │
│    - Lanza excepción si viola invariantes              │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│ 4. Infrastructure (ProductoRepository)                 │
│    - Agrega a DbContext                                │
│    - Entity Framework genera SQL parametrizado         │
│    - No hay concatenación de strings (✅ Seguro)       │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│ 5. BD (SQL Server)                                     │
│    - INSERT con parámetros                             │
│    - Retorna Id generado                               │
└──────────────────┬──────────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────────┐
│ 6. Mapper (AutoMapper)                                 │
│    - Transforma Producto → ProductoDTO                 │
│    - Retorna al Cliente                                │
└─────────────────────────────────────────────────────────┘
```

---

## Principios SOLID Aplicados

### S - Single Responsibility Principle
✅ **Aplicado**
- `Producto` solo conoce lógica de producto, no persistencia
- `RegistrarProductoUseCase` orquesta, no persiste directamente
- `MenuProductos` solo maneja interfaz

### O - Open/Closed Principle
✅ **Aplicado**
- Agregar nuevo tipo de reporte: crear `IReportGenerator` sin modificar existentes
- Cambiar BD: nueva implementación de `IProductoRepository`

### L - Liskov Substitution Principle
✅ **Aplicado**
- Cualquier `IProductoRepository` puede reemplazar a `ProductoRepository`
- Tests usan `Mock<IProductoRepository>`

### I - Interface Segregation Principle
✅ **Aplicado**
- Interfaces granulares: `IProductoRepository`, `IMovimientoRepository` (no una mega-interfaz)

### D - Dependency Inversion Principle
✅ **Aplicado**
- Application depende de `IRepository` (abstracción), no de `ProductoRepository` (implementación)
- Inyección de dependencias en `DependencyInjection.cs`

---

## Guía de Desarrollo

### 1. Agregar un Nuevo UseCase

**Paso 1**: Crear interfaz en `Application/UseCases/`
```csharp
public interface IActualizarProductoUseCase {
    Task<ProductoDTO> EjecutarAsync(ActualizarProductoDTO request);
}
```

**Paso 2**: Implementar la clase
```csharp
public class ActualizarProductoUseCase : IActualizarProductoUseCase {
    // Delega a Domain y Infrastructure
}
```

**Paso 3**: Registrar en `DependencyInjection.cs`
```csharp
services.AddScoped<IActualizarProductoUseCase, ActualizarProductoUseCase>();
```

**Paso 4**: Usar en Presentation
```csharp
var useCase = _serviceProvider.GetRequiredService<IActualizarProductoUseCase>();
await useCase.EjecutarAsync(request);
```

### 2. Cambiar de Base de Datos

**Cambiar de SQL Server a PostgreSQL**:
1. Instalar paquete NuGet: `Npgsql.EntityFrameworkCore.PostgreSQL`
2. En `Infrastructure/DependencyInjection.cs`:
```csharp
services.AddDbContext<InventarioDbContext>(options =>
    options.UseNpgsql(connectionString)); // Cambiar de UseSqlServer
```
3. Crear nueva migración: `dotnet ef migrations add UsePostgres`
4. Listo - Domain + Application no se tocan

### 3. Agregar Validación de Negocio

En `Domain/Entities/Producto.cs`:
```csharp
public static Producto Crear(...) {
    // Nuevas validaciones aquí
    if (precio < 0.01m) throw new ArgumentException("Precio mínimo: 0.01");
}
```

---

## Próximos Pasos

### Iteración 2: Escalabilidad
- [ ] Convertir a API REST (ASP.NET Core)
- [ ] Implementar CQRS (Mediator pattern)
- [ ] Caché distribuido (Redis)
- [ ] Paginación en reportes

### Iteración 3: Monitoreo y DevOps
- [ ] Logging estructurado (Serilog)
- [ ] Application Insights / ELK Stack
- [ ] Kubernetes deployment
- [ ] GitHub Actions CI/CD

### Iteración 4: Seguridad
- [ ] Autenticación (JWT)
- [ ] Autorización (Role-based)
- [ ] Audit trail
- [ ] Encriptación de secretos

---

## Referencias

- **Clean Architecture**: Robert C. Martin - "Clean Architecture"
- **SOLID**: Uncle Bob - https://en.wikipedia.org/wiki/SOLID
- **Entity Framework Core**: https://docs.microsoft.com/en-us/ef/core/
- **AutoMapper**: https://docs.automapper.org/

---

**Última actualización**: Agosto 2026  
**Versión de la Arquitectura**: 1.0.0 (RC1)
