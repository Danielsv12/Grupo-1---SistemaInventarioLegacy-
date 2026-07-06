# ÍNDICE COMPLETO DE TESTS

## 📑 Indice por Archivo

### 1. AccesoDatosCharacterizationTests.cs (25 tests)

#### ConfiguracionCharacterizationTests (5 tests)
1. `Configuracion_PropiedadesEstaticas_SonAccesibles` - Verifica acceso a constantes
2. `Configuracion_DescuentosPorTipo_TienenValoresEsperados` - Descuentos (0%, 10%, 15%)
3. `Configuracion_DatosCorreoYRutas_EstanConfigurables` - Email, SMTP, rutas
4. `Configuracion_ModoDebug_EstaHabilitadoPorDefecto` - Debug = true
5. `Configuracion_CadenaConexion_UsaTrustedConnection` - Trusted_Connection=True

#### AccesoDatosCharacterizationTests (20 tests)
6. `ObtenerProductos_DevuelveListaNoNula` - List<Producto>, nunca null
7. `Producto_MapeoDatos_ManejaNulosEnDescripcion` - NULL → ""
8. `ObtenerProductoPorId_DevuelveNullSiNoExiste` - No encontrado → null
9. `Producto_CategoriaId_PuedeSer0SiEsNull` - 0 = sin categoría
10. `ObtenerProductosBajoStock_DevuelveListaVaciaOConProductos` - Stock <= Min
11. `Cliente_MapeoDatos_ManejaNulosEnCamposOpcionales` - NULL → ""
12. `ResumenVentas_PromedioVenta_EvitaDivisionPorCero` - Prevención de error
13. `CrearPedido_CalculoImpuesto_UsaConfiguracionImpuestoVenta` - Impuesto = 13%
14. `Pedido_ContieneDatasDenormalizadasDelCliente` - JOIN con Clientes
15. `ValidarUsuario_DevuelveNullSiNoHayCoincidencia` - No encontrado → null
16. `ObtenerCategorias_DevuelveListaNoNula` - List<Categoria>, nunca null
17. `DetallePedido_ContieneNombreProductoMapedoDesdeJoin` - JOIN con Productos
18. `DetallePedido_CalculoSubtotal_IncorporaDescuento` - Subtotal - Descuento
19. `Pedido_Estado_TieneMappingDefined` - Estados 1-5
20. `ActualizarEstadoPedido_BUG_ConcatenacionSQLDinamica` - 🚨 SQL Injection
21. `ObtenerVentasPorMes_ConParametrosValidos` - YEAR/MONTH filtraje
22. `ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection` - 🚨 CRÍTICO
23. `BuscarProductos_BUG_TermiboBuscadaSinSanitizar` - 🚨 SQL Injection
24. `InsertarProducto_DevuelveIdGenerado` - SCOPE_IDENTITY()
25. `CrearPedido_BUG_NoValidaStockDisponible` - Stock negativo posible
26. `CrearPedido_BUG_SinTransacciones_InconsistenciaEnFallo` - Sin ACID
27. `ActualizarCliente_BUG_NoActualizaFechaModificacion` - Auditoría incompleta

---

### 2. AccesoDatosCriticalBehaviorTests.cs (25+ tests)

#### Casos de Borde (11 tests)
1. `Producto_MapeoCamposNumericos_PreservaTypes` - int, decimal types
2. `ObtenerProductosBajoStock_IncluyeProductosConStockIgualAlMinimo` - Stock <= Min
3. `Producto_PrecioVenta_PuedeSer_MenorQuePrecioCompra` - Margen negativo
4. `Cliente_TipoCliente_PuedeSerCero` - 0 = inválido
5. `ResumenVentas_ValoresIniciales_SonCero` - Inicialización
6. `Pedido_Estado_PuedeSerValorArbitrario` - Sin enum validation
7. `Pedido_FechasEnvioyEntrega_PuedenSerNull` - Campos nullable
8. `DetallePedido_Descuento_PuedeSer_MayorQueSubtotal` - Descuento excesivo
9. `Categoria_Descripcion_PuedeSerVacia` - String vacío
10. `Usuario_Rol_PuedeSerCero` - 0 = inválido
11. `Usuario_UltimoAcceso_PuedeSerNull` - DateTime? vacío

#### Validación Faltante (6 tests)
12. `InsertarCliente_BUG_EmailVacioNoValidado` - 🚨 UNIQUE constraint
13. `ActualizarStock_BUG_PermiteStockNegativo` - Sin límite
14. `Producto_CategoriaId_Cero_SignificaSinCategoria` - Convención
15. `Proveedor_Telefono_NoValidaFormato` - String sin validar
16. `MovimientoInventario_TipoMovimiento_PuedeSerCero` - Sin enum
17. `RegistrarMovimiento_BUG_PermiteCantidadNegativa` - Cantidad < 0

#### Coherencia (8+ tests)
18. `ObtenerPedidos_BUG_ConcatenacionNombreConEspacioFijo` - Espacios dobles
19. `Pedido_Total_EsSubTotalMasImpuesto` - Total = Sub + Imp
20. `CrearPedido_BUG_ListaDetallesVacia` - Pedido sin líneas
21. `Configuracion_Descuentos_CorrespondenAlTipoCliente` - Descuentos OK
22. `ObtenerVentasPorMes_BUG_NoValidaMes` - Mes 13 aceptado
23. `Configuracion_ValoresHardcodeados_NoSonFlexibles` - Constantes
24. `ActualizarProducto_ActualizaUltimaModificacion` - Auditoría OK
25. `EliminarProducto_EsEliminacionLogica` - Soft delete

---

### 3. DataMappingAndBusinessLogicTests.cs (20+ tests)

#### Data Mapping (10 tests)
1. `Producto_Mapeo_DesdeReader_DocumentaLaEstructura` - Estructura completa
2. `Cliente_Mapeo_CamposOpcionales_BecomeEmptyStrings` - NULL → ""
3. `Pedido_Mapeo_DatosCliente_Denormalizados` - JOIN Clientes
4. `DetallePedido_Mapeo_ConNombreProducto` - JOIN Productos
5. `MovimientoInventario_Mapeo_ConProducto` - JOIN Productos
6. `ResumenVentas_Estructura_DespuesDeCargarse` - 5 queries
7. `Categoria_ObtenerCategorias_NoMapea_FechaCreacion` - Campo omitido
8. `Producto_ObtenerProductosBajoStock_OmiteAlgunosCampos` - SELECT * pero mapeo parcial
9. `Proveedor_Mapeo_DesdeBD` - Mapeo completo
10. `Usuario_MapeoDatos` - Usuario + contraseña plana

#### Business Logic (10+ tests)
11. `Descuento_PorTipoCliente_Escalado` - 0%, 10%, 15%
12. `PedidoTotal_CalculoConImpuesto` - Total = Sub + (Sub * 0.13)
13. `Producto_Margen_Ganancia` - Margen % calculation
14. `Producto_Bajo_Stock_Logica` - Stock <= Min trigger
15. `Pedido_Estados_Validos` - Estados 1-5
16. `Usuario_Rol_Permisos` - Jerarquía de permisos
17. `DetallePedido_CantidadValida` - Cantidad > 0 (no validado)
18. `Producto_Filtro_Activos` - Filtro Activo = 1
19. `Cliente_Filtro_Activos` - Filtro Activo = 1
20. `Pedido_Subtotal_DesdeDetalles` - Sumatoria

---

## 🎯 TESTS POR TIPO

### Configuracion (5)
- ✅ Configuracion_PropiedadesEstaticas_SonAccesibles
- ✅ Configuracion_DescuentosPorTipo_TienenValoresEsperados
- ✅ Configuracion_DatosCorreoYRutas_EstanConfigurables
- ✅ Configuracion_ModoDebug_EstaHabilitadoPorDefecto
- ✅ Configuracion_CadenaConexion_UsaTrustedConnection

### Producto (12)
- ✅ ObtenerProductos_DevuelveListaNoNula
- ✅ Producto_MapeoDatos_ManejaNulosEnDescripcion
- ✅ ObtenerProductoPorId_DevuelveNullSiNoExiste
- ✅ Producto_CategoriaId_PuedeSer0SiEsNull
- ✅ ObtenerProductosBajoStock_DevuelveListaVaciaOConProductos
- ✅ ObtenerProductosBajoStock_IncluyeProductosConStockIgualAlMinimo
- ✅ Producto_PrecioVenta_PuedeSer_MenorQuePrecioCompra
- ✅ Producto_MapeoCamposNumericos_PreservaTypes
- ✅ BuscarProductos_BUG_TermiboBuscadaSinSanitizar
- ✅ ObtenerProductosBajoStock_OmiteAlgunosCampos
- ✅ Producto_Mapeo_DesdeReader_DocumentaLaEstructura
- ✅ Producto_Bajo_Stock_Logica

### Cliente (7)
- ✅ Cliente_MapeoDatos_ManejaNulosEnCamposOpcionales
- ✅ Cliente_TipoCliente_PuedeSerCero
- ✅ InsertarCliente_BUG_EmailVacioNoValidado
- ✅ ActualizarCliente_BUG_NoActualizaFechaModificacion
- ✅ Cliente_Mapeo_CamposOpcionales_BecomeEmptyStrings
- ✅ Cliente_Filtro_Activos
- ✅ Descuento_PorTipoCliente_Escalado

### Pedido (14)
- ✅ CrearPedido_CalculoImpuesto_UsaConfiguracionImpuestoVenta
- ✅ Pedido_ContieneDatasDenormalizadasDelCliente
- ✅ Pedido_Estado_TieneMappingDefined
- ✅ ActualizarEstadoPedido_BUG_ConcatenacionSQLDinamica
- ✅ CrearPedido_BUG_NoValidaStockDisponible
- ✅ CrearPedido_BUG_SinTransacciones_InconsistenciaEnFallo
- ✅ Pedido_Estado_PuedeSerValorArbitrario
- ✅ Pedido_FechasEnvioyEntrega_PuedenSerNull
- ✅ Pedido_Total_EsSubTotalMasImpuesto
- ✅ CrearPedido_BUG_ListaDetallesVacia
- ✅ ObtenerPedidos_BUG_ConcatenacionNombreConEspacioFijo
- ✅ Pedido_Mapeo_DatosCliente_Denormalizados
- ✅ Pedido_Estados_Validos
- ✅ Pedido_Subtotal_DesdeDetalles

### DetallePedido (4)
- ✅ DetallePedido_ContieneNombreProductoMapedoDesdeJoin
- ✅ DetallePedido_CalculoSubtotal_IncorporaDescuento
- ✅ DetallePedido_Descuento_PuedeSer_MayorQueSubtotal
- ✅ DetallePedido_Mapeo_ConNombreProducto
- ✅ DetallePedido_CantidadValida

### Usuario (5)
- ✅ ValidarUsuario_DevuelveNullSiNoHayCoincidencia
- ✅ ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection
- ✅ Usuario_Rol_PuedeSerCero
- ✅ Usuario_UltimoAcceso_PuedeSerNull
- ✅ Usuario_Rol_Permisos
- ✅ Usuario_MapeoDatos

### Categoria (3)
- ✅ ObtenerCategorias_DevuelveListaNoNula
- ✅ Categoria_Descripcion_PuedeSerVacia
- ✅ Categoria_ObtenerCategorias_NoMapea_FechaCreacion

### Proveedor (2)
- ✅ Proveedor_Telefono_NoValidaFormato
- ✅ Proveedor_Mapeo_DesdeBD

### Movimiento (3)
- ✅ MovimientoInventario_TipoMovimiento_PuedeSerCero
- ✅ RegistrarMovimiento_BUG_PermiteCantidadNegativa
- ✅ MovimientoInventario_Mapeo_ConProducto

### Resumen/Reportes (5)
- ✅ ResumenVentas_PromedioVenta_EvitaDivisionPorCero
- ✅ ResumenVentas_ValoresIniciales_SonCero
- ✅ ObtenerVentasPorMes_ConParametrosValidos
- ✅ ObtenerVentasPorMes_BUG_NoValidaMes
- ✅ ResumenVentas_Estructura_DespuesDeCargarse
- ✅ PedidoTotal_CalculoConImpuesto

---

## 🔴 TESTS CON BUGS (17)

1. `ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection` - 🚨 CRÍTICO
2. `BuscarProductos_BUG_TermiboBuscadaSinSanitizar` - 🚨 CRÍTICO
3. `ActualizarEstadoPedido_BUG_ConcatenacionSQLDinamica` - 🚨 CRÍTICO
4. `ObtenerVentasPorMes_BUG_NoValidaMes` - 🚨 CRÍTICO
5. `CrearPedido_BUG_NoValidaStockDisponible` - 🟠 IMPORTANTE
6. `CrearPedido_BUG_SinTransacciones_InconsistenciaEnFallo` - 🟠 IMPORTANTE
7. `ActualizarStock_BUG_PermiteStockNegativo` - 🟠 IMPORTANTE
8. `InsertarCliente_BUG_EmailVacioNoValidado` - 🟠 IMPORTANTE
9. `RegistrarMovimiento_BUG_PermiteCantidadNegativa` - 🟠 IMPORTANTE
10. `ActualizarCliente_BUG_NoActualizaFechaModificacion` - 🟡 MODERADO
11. `ObtenerPedidos_BUG_ConcatenacionNombreConEspacioFijo` - 🟡 MODERADO
12. `Producto_PrecioVenta_PuedeSer_MenorQuePrecioCompra` - 🟡 MODERADO
13. `Cliente_TipoCliente_PuedeSerCero` - 🟡 MODERADO
14. `Pedido_Estado_PuedeSerValorArbitrario` - 🟡 MODERADO
15. `Usuario_Rol_PuedeSerCero` - 🟡 MODERADO
16. `MovimientoInventario_TipoMovimiento_PuedeSerCero` - 🟡 MODERADO
17. `Configuracion_ValoresHardcodeados_NoSonFlexibles` - 🟡 MODERADO

---

## ✅ TESTS SIN BD (Pasan Siempre)

Estos 45+ tests NO necesitan conexión a BD:

- Todos en `ConfiguracionCharacterizationTests` (5)
- Todos en `AccesoDatosCriticalBehaviorTests` (25+)
- Todos en `DataMappingAndBusinessLogicTests` (20+)
- Tests que solo verifican tipos/estructura en memory

**Ejecutar:** `dotnet test --filter "Configuracion"`

---

## ⚠️ TESTS CON BD (Opcionales)

Estos ~25 tests necesitan BD configurada:

- `ObtenerProductos_DevuelveListaNoNula`
- `ObtenerProductoPorId_DevuelveNullSiNoExiste`
- `ObtenerProductosBajoStock_DevuelveListaVaciaOConProductos`
- `ObtenerCategorias_DevuelveListaNoNula`
- Otros que llaman `AccesoDatos.*` directamente

**Prerequisitos:**
1. Editar `Configuracion.CadenaConexion`
2. Ejecutar `setup_database.sql`
3. Insertar datos de prueba

---

## 🔍 BÚSQUEDA RÁPIDA

```bash
# Por nombre de test
dotnet test --filter "Producto"

# Por clase
dotnet test --filter "ClassName=ConfiguracionCharacterizationTests"

# Por múltiples
dotnet test --filter "Name~Producto|Name~Cliente"

# Todos excepto BD
dotnet test --filter "Name~Mapeo|Name~BUG|Name~Configuracion"
```

---

## 📊 ESTADÍSTICAS

| Métrica | Valor |
|---------|-------|
| Total Tests | 70+ |
| Tests Sin BD | 45+ ✅ |
| Tests Con BD | 25+ ⚠️ |
| Tests con Bugs | 17 🚨 |
| Métodos Cubiertos | 30+ |
| Casos Borde | 20+ |
| Líneas de Test | ~3,200 |

---

## 🎓 CÓMO USAR ESTE ÍNDICE

1. **Busca por nombre de método** - Encuentra qué test lo prueba
2. **Busca por Bug** - Encuentra tests que documentan problemas
3. **Busca por Tipo** - Encuentra tests relacionados
4. **Usa en búsqueda CLI** - Filtra tests con `--filter`

---

**Generado:** Characterization Tests Suite  
**Metodología:** Michael Feathers  
**Estado:** Completo y listo para usar  
