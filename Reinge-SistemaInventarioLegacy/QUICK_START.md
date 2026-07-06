# QUICK START - Characterization Tests

## 📋 Archivos Generados

```
AccesoDatosCharacterizationTests.cs          ← 25 tests principales
AccesoDatosCriticalBehaviorTests.cs          ← 25+ tests de casos de borde
DataMappingAndBusinessLogicTests.cs          ← 20+ tests de mapeo y lógica
CHARACTERIZATION_TESTS_README.md             ← Documentación completa
SUMARIO_EJECUTIVO.md                         ← Análisis y recomendaciones
QUICK_START.md                               ← Este archivo
```

## 🚀 EJECUCIÓN RÁPIDA

### Paso 1: Instalar xUnit (si no está)

```bash
cd ./Reinge-SistemaInventarioLegacy
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
```

### Paso 2: Compilar

```bash
dotnet build
```

### Paso 3: Ejecutar Tests

```bash
# Todos los tests
dotnet test

# Solo configuración (sin BD)
dotnet test --filter "Configuracion"

# Solo tests con bugs documentados
dotnet test --filter "BUG"

# Verbose (ver detalles)
dotnet test -v
```

---

## 📊 DISTRIBUCIÓN DE TESTS

### AccesoDatosCharacterizationTests.cs (25 tests)
- Configuracion Properties (5)
- Producto Mapping (6)
- Cliente Mapping (2)
- Pedido & Detalles (3)
- Bugs Críticos (9)

### AccesoDatosCriticalBehaviorTests.cs (25+ tests)
- Data Type Safety (1)
- Edge Cases (11)
- Missing Validation (6)
- Consistency (8+)

### DataMappingAndBusinessLogicTests.cs (20+ tests)
- Data Mapping (10)
- Business Logic (10)

---

## 🎯 TESTS PRINCIPALES

### Sin BD (Pasan siempre) ✅

```bash
dotnet test --filter "Configuracion"
```

**Resultado:** ~15 tests ✅

---

### Con BD (Requieren setup)

```bash
# 1. Editar Configuracion.CadenaConexion
# 2. Ejecutar setup_database.sql
# 3. Ejecutar tests

dotnet test --filter "ObtenerProductos or ObtenerClientes"
```

---

## 🐛 BUGS ENCONTRADOS

| Bug | Test | Severidad |
|-----|------|-----------|
| SQL Injection en ValidarUsuario | `ValidarUsuario_BUG_...` | 🔴 CRÍTICO |
| SQL Injection en BuscarProductos | `BuscarProductos_BUG_...` | 🔴 CRÍTICO |
| Sin Stock Validation | `CrearPedido_BUG_...` | 🟠 IMPORTANTE |
| Sin Transacciones | `CrearPedido_BUG_...` | 🟠 IMPORTANTE |
| Stock Negativo Permitido | `ActualizarStock_BUG_...` | 🟠 IMPORTANTE |
| Sin Auditoría en Clientes | `ActualizarCliente_BUG_...` | 🟡 MODERADO |

---

## 🔍 BÚSQUEDA RÁPIDA

```bash
# Todos los tests de Producto
dotnet test --filter "Producto"

# Todos los tests de bug
dotnet test --filter "BUG"

# Todos los tests de mapeo
dotnet test --filter "Mapeo"

# Tests específicos de SQL Injection
dotnet test --filter "SQLInjection or SQL_Injection or BuscarProductos or ValidarUsuario"

# Tests de validación de stock
dotnet test --filter "Stock"

# Tests de casos de borde
dotnet test --filter "CriticalBehavior"
```

---

## 📝 PATRÓN DE LOS TESTS

Todos los tests siguen la estructura AAA:

```csharp
[Fact]
public void NombreDelTest_DescribeEsperado()
{
    // Arrange - Preparar
    var dato = new TipoDato { Propiedad = valor };
    
    // Act - Ejecutar
    var resultado = EjecutarLogica(dato);
    
    // Assert - Validar
    Assert.Equal(valorEsperado, resultado);
    // Comentario: Explica el comportamiento
}
```

---

## 🛠️ CUSTOMIZACIÓN

### Ejecutar solo un test

```bash
# Por nombre exacto
dotnet test --filter "FullyQualifiedName=SistemaInventarioLegacy.Tests.ConfiguracionCharacterizationTests.Configuracion_PropiedadesEstaticas_SonAccesibles"

# Por substring
dotnet test --filter "Name~Producto"

# Múltiples condiciones (AND)
dotnet test --filter "Name~Producto&Name~Stock"

# Múltiples condiciones (OR)
dotnet test --filter "ClassName=ConfiguracionCharacterizationTests|ClassName=AccesoDatosCharacterizationTests"
```

---

## ✅ CHECKLIST DE VERIFICACIÓN

Después de ejecutar los tests, verifica:

- [ ] Tests de Configuracion pasan (15+) ✅
- [ ] Tests de Mapeo pasan (10+) ✅
- [ ] Tests de Business Logic pasan (10+) ✅
- [ ] Bugs están documentados (17) 🚨
- [ ] No hay falsos positivos
- [ ] Todos los `[Fact]` están presentes
- [ ] No hay excepciones inesperadas

---

## 🔧 TROUBLESHOOTING

### Error: "No tests found"
```bash
# Asegúrate de estar en el directorio del proyecto
cd ./Reinge-SistemaInventarioLegacy

# Verifica que xUnit esté instalado
dotnet package list | grep xunit
```

### Error: "Connection failed"
```bash
# Los tests que necesitan BD fallarán si no está configurada
# Para ejecutar solo los tests sin BD:
dotnet test --filter "Configuracion"

# Para todos los tests, configura:
# 1. Editar Configuracion.CadenaConexion
# 2. Ejecutar setup_database.sql
```

### Error: "Test method not found"
```bash
# Asegúrate de que los archivos .cs están en la carpeta del proyecto
# Compila primero:
dotnet build

# Luego ejecuta:
dotnet test
```

---

## 📚 DOCUMENTACIÓN COMPLETA

Para detalles exhaustivos, consulta:

- `CHARACTERIZATION_TESTS_README.md` - Guía completa
- `SUMARIO_EJECUTIVO.md` - Análisis de bugs
- Comentarios en los tests (.cs) - Explican cada test

---

## 🎓 CONCEPTOS CLAVE

**Characterization Tests** (Michael Feathers):
- Capturan el comportamiento ACTUAL (incluyendo bugs)
- Protegen contra regresiones durante refactorización
- Documentan la especificación existente
- NO validan si el código es correcto

**Por qué documentamos bugs:**
- Evita cambios accidentales durante refactorización
- Proporciona contexto para mejoras futuras
- Especifica el comportamiento actual para compatibilidad

---

## 🚀 PRÓXIMOS PASOS

1. ✅ Ejecutar todos los tests
2. ✅ Verificar que pasan (sin BD: 45+, con BD: 70+)
3. 📋 Leer SUMARIO_EJECUTIVO.md
4. 🔧 Comenzar refactorización en orden de severidad
5. 🔄 Mantener tests verdes durante cambios
6. ✨ Actualizar tests cuando corrijas bugs

---

## 💡 TIPS

- Los tests SIN BD pasan **siempre** → úsalos para verificación rápida
- Los tests CON BD son **opcionales** → necesitan setup de BD
- Los comentarios "// BUG:" marcan problemas conocidos
- Cada test es **independiente** → puedes ejecutarlos en cualquier orden
- Los nombres de tests son **descriptivos** → dicen exactamente qué validan

---

**Estado:** ✅ Listo para usar  
**Archivos:** 3 suites de tests + 3 docs  
**Total Tests:** 70+  
**Cobertura:** 30+ métodos en AccesoDatos  
**Bugs:** 17 documentados  

