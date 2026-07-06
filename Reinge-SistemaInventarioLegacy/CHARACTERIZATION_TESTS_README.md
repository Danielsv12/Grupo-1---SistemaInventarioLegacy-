# Characterization Tests - Suite de Pruebas del Código Legacy

## Descripción

Esta suite de **Characterization Tests** fue generada siguiendo la metodología de Michael Feathers en "Working Effectively with Legacy Code". Los tests capturan el **comportamiento ACTUAL** del código en `AccesoDatos.cs` y `Configuracion.cs`, incluyendo bugs documentados.

### Propósito

- ✅ **Proteger contra regresiones** durante refactorización
- ✅ **Documentar el comportamiento esperado** (aunque sea incorrecto)
- ✅ **Validar que los bugs no se "arreglen accidentalmente"** cambiando el comportamiento
- ❌ **NO validar si el código es correcto**, sino QUÉ hace realmente

## Archivos de Tests

### 1. `AccesoDatosCharacterizationTests.cs` (25+ tests)

Tests principales que capturan el comportamiento de los métodos en `AccesoDatos.cs`:

**Tests de Configuracion:**
- `Configuracion_PropiedadesEstaticas_SonAccesibles` - Verifica acceso a constantes
- `Configuracion_DescuentosPorTipo_TienenValoresEsperados` - Descuentos por cliente
- `Configuracion_DatosCorreoYRutas_EstanConfigurables` - Credenciales y rutas
- `Configuracion_ModoDebug_EstaHabilitadoPorDefecto` - Estado del debug
- `Configuracion_CadenaConexion_UsaTrustedConnection` - Patrón de conexión

**Tests de Comportamiento de Datos:**
- `ObtenerProductos_DevuelveListaNoNula` - Retorno de listas nunca null
- `Producto_MapeoDatos_ManejaNulosEnDescripcion` - Manejo de DBNull
- `ObtenerProductoPorId_DevuelveNullSiNoExiste` - Búsqueda con resultado nulo
- `Producto_CategoriaId_PuedeSer0SiEsNull` - Valores especiales como 0
- `ObtenerProductosBajoStock_DevuelveListaVaciaOConProductos` - Filtrado de datos
- `Cliente_MapeoDatos_ManejaNulosEnCamposOpcionales` - Campos opcionales = ""
- `ResumenVentas_PromedioVenta_EvitaDivisionPorCero` - Prevención de errores
- `CrearPedido_CalculoImpuesto_UsaConfiguracionImpuestoVenta` - Cálculos monetarios

**Tests de Anti-patrones:**
- `Pedido_ContieneDatasDenormalizadasDelCliente` - Denormalización en modelo
- `ValidarUsuario_DevuelveNullSiNoHayCoincidencia` - Autenticación
- `DetallePedido_ContieneNombreProductoMapedoDesdeJoin` - JOIN con datos denormalizados

**Tests de BUGS DOCUMENTADOS:**
- `ActualizarEstadoPedido_BUG_ConcatenacionSQLDinamica` - 🚨 SQL Injection
- `ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection` - 🚨 SQL Injection crítico
- `BuscarProductos_BUG_TermiboBuscadaSinSanitizar` - 🚨 SQL Injection en búsqueda
- `CrearPedido_BUG_NoValidaStockDisponible` - Stock puede ser negativo
- `CrearPedido_BUG_SinTransacciones_InconsistenciaEnFallo` - Sin ACID
- `ActualizarCliente_BUG_NoActualizaFechaModificacion` - Auditoría incompleta

### 2. `AccesoDatosCriticalBehaviorTests.cs` (25+ tests)

Tests avanzados enfocados en casos de borde extremos y comportamientos críticos:

**Tests de Casos de Borde:**
- `Producto_MapeoCamposNumericos_PreservaTypes` - Preservación de tipos
- `ObtenerProductosBajoStock_IncluyeProductosConStockIgualAlMinimo` - Stock <= StockMinimo
- `Producto_PrecioVenta_PuedeSer_MenorQuePrecioCompra` - Márgenes negativos
- `Cliente_TipoCliente_PuedeSerCero` - Valores inválidos aceptados
- `ResumenVentas_ValoresIniciales_SonCero` - Inicialización por defecto
- `Pedido_Estado_PuedeSerValorArbitrario` - Estados sin validación
- `Pedido_FechasEnvioyEntrega_PuedenSerNull` - Campos nullable
- `DetallePedido_Descuento_PuedeSer_MayorQueSubtotal` - Descuentos excesivos
- `Categoria_Descripcion_PuedeSerVacia` - Campos opcionales vacíos
- `Usuario_Rol_PuedeSerCero` - Roles sin validación enum
- `Usuario_UltimoAcceso_PuedeSerNull` - Campos de auditoría vacíos

**Tests de Validación Faltante:**
- `InsertarCliente_BUG_EmailVacioNoValidado` - 🚨 Email requerido pero vacío
- `ActualizarStock_BUG_PermiteStockNegativo` - Stock sin límite inferior
- `Producto_CategoriaId_Cero_SignificaSinCategoria` - Convención no enforced
- `Proveedor_Telefono_NoValidaFormato` - Formato sin validación
- `MovimientoInventario_TipoMovimiento_PuedeSerCero` - Enum sin validación
- `RegistrarMovimiento_BUG_PermiteCantidadNegativa` - Cantidad negativa

**Tests de Coherencia:**
- `ObtenerPedidos_BUG_ConcatenacionNombreConEspacioFijo` - Espacios dobles en concatenación
- `Pedido_Total_EsSubTotalMasImpuesto` - Fórmula de cálculo
- `CrearPedido_BUG_ListaDetallesVacia` - Pedidos sin líneas
- `Configuracion_Descuentos_CorrespondenAlTipoCliente` - Descuentos correctos
- `ObtenerVentasPorMes_BUG_NoValidaMes` - Mes 13 aceptado silenciosamente
- `Configuracion_ValoresHardcodeados_NoSonFlexibles` - Configuración rígida
- `ActualizarProducto_ActualizaUltimaModificacion` - Auditoría consistente
- `EliminarProducto_EsEliminacionLogica` - Soft delete (UPDATE, no DELETE)

## Configuración del Proyecto

### Requisitos

- .NET 6.0 o superior
- xUnit
- SQL Server (para pruebas de integración real)

### Instalación de xUnit

```bash
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package xunit.runner.console
```

O editar `.csproj`:

```xml
<ItemGroup>
    <PackageReference Include="xunit" Version="2.6.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
    <PackageReference Include="xunit.runner.console" Version="2.6.2" />
</ItemGroup>
```

## Ejecutar los Tests

### En Visual Studio

1. Abrir **Test Explorer** (Test → Windows → Test Explorer)
2. Click en "Run All Tests"
3. O click derecho en una clase/método específico

### En línea de comandos

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar solo tests de Configuracion
dotnet test --filter "ConfiguracionCharacterizationTests"

# Ejecutar solo tests de AccesoDatos
dotnet test --filter "AccesoDatosCharacterizationTests"

# Ejecutar solo tests de comportamiento crítico
dotnet test --filter "AccesoDatosCriticalBehaviorTests"

# Ejecutar con output verbose
dotnet test -v

# Ejecutar específico
dotnet test --filter "Name~Producto_MapeoDatos"
```

## Resultados Esperados

### Tests que PASARÁN (sin BD)

Tests que solo validan **estructura de tipos** y **lógica sin BD**:
- Todos los tests en `ConfiguracionCharacterizationTests`
- Tests que solo verifican tipos/comportamiento de objetos en memoria
- Tests que reconstruyen SQL para inspeccionar (sin ejecutar)

### Tests que FALLARÁN (requieren BD)

Tests que ejecutan métodos que necesitan conexión SQL:
- `ObtenerProductos_DevuelveListaNoNula`
- `ObtenerProductoPorId_DevuelveNullSiNoExiste`
- `ObtenerProductosBajoStock_DevuelveListaVaciaOConProductos`
- `ObtenerCategorias_DevuelveListaNoNula`
- Cualquier test que llame a `AccesoDatos.*` directamente

**Para que estos tests pasen:**
1. Configurar BD en `Configuracion.CadenaConexion`
2. Ejecutar `setup_database.sql` para crear tablas
3. Insertar datos de prueba
4. Luego ejecutar los tests

## Bugs Documentados

Los tests documentan los siguientes bugs conocidos (marcados con `// BUG:` en los comentarios):

### 🚨 CRÍTICOS - SQL Injection

1. **ValidarUsuario** - SQL Injection en autenticación (peor lugar posible)
   ```csharp
   string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario + 
                "' AND Contrasena = '" + contrasena + "' AND Activo = 1";
   ```
   **Ataque:** `nombreUsuario = "' OR '1'='1"` → bypass de autenticación

2. **BuscarProductos** - SQL Injection en búsqueda
   ```csharp
   string sql = "SELECT * FROM Productos WHERE ... LIKE '%" + termino + "%'";
   ```
   **Ataque:** `termino = "'; DROP TABLE Productos; --"`

3. **ObtenerVentasPorMes** - SQL Injection en parámetros numéricos
   ```csharp
   "WHERE YEAR(FechaPedido) = " + anio + " AND MONTH(FechaPedido) = " + mes
   ```

### ⚠️ IMPORTANTES - Falta de Validación

4. **CrearPedido** - No valida stock disponible
   - Stock puede volverse negativo
   - Sin transacciones (ACID failure)

5. **ActualizarStock** - Permite stock negativo
6. **InsertarCliente** - Email vacío no validado
7. **RegistrarMovimiento** - Cantidad negativa aceptada
8. **ObtenerVentasPorMes** - Mes fuera de rango (1-12) aceptado silenciosamente

### 📋 MODERADOS - Diseño Pobre

9. **Gestión de conexiones** - No usa `using`, propenso a fugas
10. **ActualizarCliente** - NO actualiza UltimaModificacion (inconsistente con ActualizarProducto)
11. **Contraseñas en texto plano** - Usuario.Contrasena no tiene hash
12. **Configuración hardcodeada** - Rutas y credenciales en código fuente
13. **Denormalización de datos** - Pedido contiene datos del Cliente, DetallePedido contiene NombreProducto

## Estructura de los Tests

Cada test sigue la pauta AAA (Arrange-Act-Assert):

```csharp
[Fact]
public void Test_Name_ExplainsExpectedBehavior()
{
    // Arrange - Preparar datos de prueba
    var producto = new Producto { Stock = 10, StockMinimo = 10 };

    // Act - Ejecutar el código bajo prueba
    bool consideradoBajoStock = producto.Stock <= producto.StockMinimo;

    // Assert - Validar el resultado
    Assert.True(consideradoBajoStock);
    // Comentario: Explica el comportamiento actual
}
```

## Características de los Tests

✅ **Ningún Mock externo** - Usan tipos concretos de `Modelos.cs`
✅ **Documentación exhaustiva** - Cada test explica QUÉ y POR QUÉ
✅ **Casos de borde** - Al menos 1 por módulo (Stock=0, Cantidad=0, NULL, límites)
✅ **Bugs documentados** - Marcan problemas con `// BUG:` y contexto
✅ **Listos para compilar** - Incluyen todos los `using` necesarios
✅ **xUnit** - No MSTest ni NUnit
✅ **[Fact]** - Todos usan atributo [Fact] para xUnit

## Próximos Pasos para Refactorización

Una vez pasados estos tests, puedes refactorizar con seguridad:

1. ✅ **Parametrizar SQL** - Reemplazar concatenación con `SqlParameter`
2. ✅ **Usar transacciones** - `SqlTransaction` en operaciones multi-paso
3. ✅ **Validar datos** - Guard clauses al inicio de métodos
4. ✅ **Usar `using`** - Garantizar cierre de conexiones
5. ✅ **Extraer métodos** - Reducir duplicación de mapeo de Producto
6. ✅ **Enumeraciones** - Reemplazar `int` con `enum` para Estados, Roles, etc.
7. ✅ **Hash de contraseñas** - Usar bcrypt o similar
8. ✅ **DI de configuración** - Inyectar `CadenaConexion` en lugar de estático
9. ✅ **Separar capas** - Repository pattern para aislación de BD

## Referencias

- **Michael Feathers** - "Working Effectively with Legacy Code" (Characterization Tests: Capítulo 5)
- **xUnit Documentación** - https://xunit.net/
- **OWASP** - SQL Injection Prevention - https://owasp.org/www-community/attacks/SQL_Injection

## Notas Importantes

⚠️ **Estos tests capturan bugs.** Su propósito es:
- Documentar el estado actual (incluyendo problemas)
- Prevenir regresiones durante refactorización
- Servir como especificación de comportamiento existente

**No son una validación de corrección.** Para limpiar el código, refactoriza siguiendo los tests, luego actualiza los tests para reflejar el código mejorado.
