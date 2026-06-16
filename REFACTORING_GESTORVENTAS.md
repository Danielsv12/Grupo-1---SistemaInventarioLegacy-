# Refactoring: GestorVentas

**Archivo modificado:** `Reinge-SistemaInventarioLegacy/refactor.cs`  
**Fecha:** 2026-06-09  
**Stack:** C# / .NET 10 — Console Application

---

## Punto de partida — código original

```csharp
public class GestorVentas
{
    public decimal CalcularTotal(List<(int productoId, int cantidad, decimal precio)> items,
        string clienteNombre, string clienteEmail, string clienteCalle,
        string clienteCiudad, string clienteCodigoPostal, string tipoCliente)
    {
        // 1. Calcula subtotal
        // 2. Aplica descuento con if/else sobre string
        // 3. Envía email con SmtpClient instanciado aquí mismo
        // 4. Escribe factura en disco con File.WriteAllText
        return total;
    }
}
```

Un solo método de 50 líneas con cuatro responsabilidades mezcladas.

---

## Code Smells identificados — catálogo Fowler

| # | Smell | Manifestación concreta | Refactorización |
|---|-------|------------------------|-----------------|
| 1 | **God Class** | `GestorVentas` maneja cálculo, email y disco | Extract Class → SRP |
| 2 | **Long Method** | `CalcularTotal` = 50+ líneas, 4 responsabilidades | Extract Method × 5 |
| 3 | **Primitive Obsession** | `string calle, ciudad, codigoPostal` como primitivos sueltos | Value Object `DireccionCliente` |
| 4 | **Data Clumps** | Los 3 strings de dirección siempre viajan juntos | Agrupados en `DireccionCliente` |
| 5 | **Long Parameter List** | 7 parámetros en `CalcularTotal` | Parameter Object `PedidoVenta` |
| 6 | **Feature Envy** | Lógica de descuento vive en `GestorVentas`, no donde pertenece | Strategy `ICalculadorDescuento` |
| 7 | **Switch Statements** | `if/else` sobre `"VIP"/"Mayorista"/"Regular"` | Enum + Strategy + Factory Method |
| 8 | **Magic Strings** | `"VIP"`, `"Mayorista"`, `"Regular"` hardcodeados | `enum TipoCliente` |
| 9 | **Magic Numbers** | `0.13m`, `0.15m`, `0.20m`, `0.05m` inline | `const TasaIVA` + cada Strategy |

---

## Principios SOLID aplicados

### S — Single Responsibility Principle
Antes `GestorVentas` tenía 3 razones para cambiar. Ahora cada clase tiene exactamente una:

| Clase | Única responsabilidad |
|-------|-----------------------|
| `GestorVentas` | Orquestar el flujo de una venta |
| `ServicioNotificacionEmail` | Enviar emails de confirmación |
| `GeneradorFacturaTxt` | Persistir la factura en disco |
| `Descuento{Tipo}` | Calcular el porcentaje del tipo de cliente |

### O — Open/Closed Principle
Agregar `TipoCliente.Distribuidor` con 12% requiere **solo** 3 pasos, sin modificar `GestorVentas`:

```csharp
// 1. Enum
public enum TipoCliente { Regular, Mayorista, VIP, Distribuidor }

// 2. Nueva Strategy
public sealed class DescuentoDistribuidor : ICalculadorDescuento
{
    public decimal Calcular(decimal subtotal) => subtotal * 0.12m;
}

// 3. Un case en la fábrica
TipoCliente.Distribuidor => new DescuentoDistribuidor(),
```

### D — Dependency Inversion Principle
```csharp
// ANTES: dependencia concreta hardcodeada
var smtp = new System.Net.Mail.SmtpClient("smtp.empresa.com");
File.WriteAllText($"factura_{DateTime.Now:yyyyMMdd}.txt", factura);

// DESPUÉS: depende de abstracciones inyectadas
public GestorVentas(IServicioNotificacion notificacion, IGeneradorFactura generadorFactura)
```
Permite sustituir SMTP por SendGrid, o TXT por PDF, sin tocar el dominio.

### I — Interface Segregation Principle
Interfaces pequeñas con un solo método. Ninguna implementación está obligada a satisfacer contratos que no le corresponden.

---

## Patrones de diseño aplicados

### Strategy — `ICalculadorDescuento`
Encapsula el algoritmo de descuento. Cada tipo de cliente es una clase independiente.

```
ICalculadorDescuento
    ├── DescuentoRegular    → 5%
    ├── DescuentoMayorista  → 20%
    └── DescuentoVIP        → 15%
```

### Factory Method — `FabricaDescuento.Crear()`
Centraliza la selección de estrategia. Único punto de cambio al agregar tipos nuevos.

```csharp
public static ICalculadorDescuento Crear(TipoCliente tipo) => tipo switch
{
    TipoCliente.Regular   => new DescuentoRegular(),
    TipoCliente.Mayorista => new DescuentoMayorista(),
    TipoCliente.VIP       => new DescuentoVIP(),
    _                     => throw new ArgumentOutOfRangeException(nameof(tipo))
};
```

### Value Object — `DireccionCliente`, `ItemVenta`, `ResultadoVenta`
- Inmutables por diseño (solo getters, sin setters)
- Validación de invariantes en el constructor
- `Subtotal` en `ItemVenta` es propiedad calculada, no campo

### Parameter Object — `PedidoVenta`
Reemplaza los 7 parámetros primitivos del método original. Agrupa datos cohesivos y hace el contrato legible.

### Dependency Injection — constructor de `GestorVentas`
```csharp
// Composición en Program.cs / contenedor DI
var gestor = new GestorVentas(
    new ServicioNotificacionEmail(Configuracion.ServidorSMTP, Configuracion.EmailAdmin),
    new GeneradorFacturaTxt(Configuracion.RutaReportes));
```

---

## Extract Method — mapa completo

```
ProcesarVenta(PedidoVenta)
    ├── ValidarPedido(PedidoVenta)          ← validación
    └── CalcularTotales(PedidoVenta)        ← cálculo
            ├── CalcularSubtotal(items)
            ├── AplicarDescuento(subtotal, tipo)
            └── AplicarImpuesto(baseImponible)
```

---

## Estructura resultante

```
refactor.cs
│
├── enum TipoCliente
│
├── Value Objects
│   ├── DireccionCliente
│   ├── ItemVenta
│   ├── PedidoVenta          (Parameter Object)
│   └── ResultadoVenta
│
├── Strategy
│   ├── interface ICalculadorDescuento
│   ├── DescuentoRegular
│   ├── DescuentoMayorista
│   ├── DescuentoVIP
│   └── FabricaDescuento     (Factory Method)
│
├── Abstracciones DIP
│   ├── interface IServicioNotificacion
│   └── interface IGeneradorFactura
│
├── Implementaciones de infraestructura
│   ├── ServicioNotificacionEmail
│   └── GeneradorFacturaTxt
│
└── GestorVentas             (orquestador puro, 40 líneas)
```

---

## Comparativa antes / después

| Métrica | Antes | Después |
|---------|-------|---------|
| Parámetros en método principal | 7 primitivos | 1 objeto tipado |
| Líneas en `GestorVentas` | 50 (todo en uno) | ~40 (solo orquestación) |
| Clases con responsabilidades propias | 1 | 9 |
| Testeable en aislamiento | No | Sí (mock de interfaces) |
| Agregar nuevo tipo de cliente | Modificar `GestorVentas` | Solo `FabricaDescuento` |
| Cambiar proveedor de email | Modificar `GestorVentas` | Solo `IServicioNotificacion` |
