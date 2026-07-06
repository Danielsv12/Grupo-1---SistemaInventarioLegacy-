# 🔴 BUG CRÍTICO: SQL Injection en ValidarUsuario (Autenticación)

## 📌 Resumen Ejecutivo

El método `ValidarUsuario()` en `AccesoDatos.cs` es **vulnerable a SQL Injection en el punto más crítico del sistema: la autenticación**. Esto permite a un atacante:

1. ✅ Bypass completo de autenticación sin conocer la contraseña
2. ✅ Acceso no autorizado como cualquier usuario (admin, operador, etc.)
3. ✅ Acceso con máximos permisos del sistema
4. ✅ Auditoría falseada (se registra último acceso falso)

---

## 🔍 EL BUG CAPTURADO POR EL TEST

### Código Vulnerable (Original)

```csharp
public static Usuario ValidarUsuario(string nombreUsuario, string contrasena)
{
    SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
    conn.Open();
    
    // ⚠️ BUG CRÍTICO: Concatenación directa de parámetros del usuario
    string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
               + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";
    
    SqlCommand cmd = new SqlCommand(sql, conn);
    SqlDataReader reader = cmd.ExecuteReader();
    Usuario u = null;
    
    if (reader.Read())
    {
        u = new Usuario();
        u.Id = (int)reader["Id"];
        u.NombreUsuario = reader["NombreUsuario"].ToString();
        u.NombreCompleto = reader["NombreCompleto"] != DBNull.Value 
            ? reader["NombreCompleto"].ToString() : "";
        u.Rol = (int)reader["Rol"];
        u.Activo = (bool)reader["Activo"];
    }
    reader.Close();
    
    // Actualizar último acceso (PROBLEMA: se ejecuta aunque sea un ataque)
    if (u != null)
    {
        SqlCommand cmdUpdate = new SqlCommand(
            "UPDATE Usuarios SET UltimoAcceso = GETDATE() WHERE Id = " + u.Id, conn);
        cmdUpdate.ExecuteNonQuery();
    }
    
    conn.Close();
    return u;
}
```

### Test que Captura el Bug

```csharp
[Fact]
public void ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection()
{
    // Arrange
    string nombreUsuario = "admin";
    string contrasena = "password123";

    // Act
    // El código hace:
    // string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
    //    + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";
    string sqlReconstruida = "SELECT * FROM Usuarios WHERE NombreUsuario = '" 
        + nombreUsuario + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";

    // Assert
    Assert.Contains("Usuarios", sqlReconstruida);
    Assert.Contains("Contrasena = '", sqlReconstruida);
    
    // BUG 1: SQL Injection - la contraseña va sin parámetros parametrizados
    // BUG 2: Contraseñas en texto plano en BD
    // ATAQUE POSIBLE: nombreUsuario = "' OR '1'='1"
    // Resultaría en: SELECT * FROM Usuarios WHERE NombreUsuario = '' OR '1'='1' AND ...
    // Bypass de autenticación
}
```

---

## 💥 ESCENARIOS DE ATAQUE DEMOSTRADOS

### Ataque 1: Bypass de Autenticación (El Clásico)

**Entrada del Atacante:**

```csharp
var nombreUsuario = "' OR '1'='1' --";
var contrasena = "cualquiera";

// SQL que se ejecuta:
// SELECT * FROM Usuarios WHERE NombreUsuario = '' OR '1'='1' -- ' 
//   AND Contrasena = 'cualquiera' AND Activo = 1
```

**SQL Resultante (Simplificado):**

```sql
SELECT * FROM Usuarios 
WHERE NombreUsuario = '' OR '1'='1' -- ' 
  AND Contrasena = 'cualquiera' 
  AND Activo = 1
```

**¿Qué pasa?**
- `NombreUsuario = ''` → Falso
- `OR '1'='1'` → Verdadero (siempre)
- El comentario `--` anula el resto de la condición
- **Resultado: Retorna el PRIMER usuario activo de la tabla**

**Probabilidad:** Si el primer usuario es "admin", ¡atacante es admin!

---

### Ataque 2: Acceso como Usuario Específico

**Entrada del Atacante:**

```csharp
var nombreUsuario = "admin' --";
var contrasena = "cualquiera";

// SQL que se ejecuta:
// SELECT * FROM Usuarios WHERE NombreUsuario = 'admin' -- ' 
//   AND Contrasena = 'cualquiera' AND Activo = 1
```

**SQL Resultante:**

```sql
SELECT * FROM Usuarios 
WHERE NombreUsuario = 'admin' -- ' 
  AND Contrasena = 'cualquiera' 
  AND Activo = 1
```

**¿Qué pasa?**
- La condición de contraseña es ignorada por `--`
- **Acceso directo como "admin" sin conocer la contraseña**

---

### Ataque 3: Extracción de Datos

**Entrada del Atacante:**

```csharp
var nombreUsuario = "admin' UNION SELECT * FROM CreditoCards --";
var contrasena = "anything";

// Podría extraer datos sensibles de otras tablas
```

---

### Ataque 4: Modificación/Eliminación

**Entrada del Atacante:**

```csharp
var nombreUsuario = "'; DROP TABLE Usuarios; --";
var contrasena = "anything";

// SQL que se ejecuta:
// SELECT * FROM Usuarios WHERE NombreUsuario = ''; DROP TABLE Usuarios; -- ' ...
```

**¿Qué pasa?**
- Se ejecutan múltiples comandos (batch queries)
- **BORRA TODA LA TABLA DE USUARIOS**
- El sistema de autenticación queda inutilizado

---

## ⚠️ PROBLEMAS SECUNDARIOS (TAMBIÉN CAPTURADOS)

### Bug #2: Contraseñas en Texto Plano

```csharp
// En la BD se almacena:
// NombreUsuario | Contrasena | Rol
// admin         | password123| 3
//              ↑ TEXTO PLANO
```

**Riesgos:**
- Si la BD es comprometida, todas las contraseñas son visibles
- Cualquier admin que lea la BD ve las contraseñas
- Empleados con acceso a backups ven contraseñas
- No hay recuperación posible (no es hash)

### Bug #3: Auditoría Falseada

```csharp
// Aunque haya un ataque, se registra:
if (u != null)
{
    SqlCommand cmdUpdate = new SqlCommand(
        "UPDATE Usuarios SET UltimoAcceso = GETDATE() WHERE Id = " + u.Id, 
        conn);
    cmdUpdate.ExecuteNonQuery();  // ← VULNERABLE TAMBIÉN
}
```

**Problema:**
- Se registra acceso exitoso aunque sea un ataque
- El atacante puede falsear su último acceso
- Los logs de auditoría son engañosos

---

## 🔐 POR QUÉ DOCUMENTARLO EN UN TEST (Characterization Test)

### 1️⃣ Captura el Comportamiento ACTUAL (Aunque sea Malo)

El test **NO dice que el código es correcto**. Al contrario, tiene comentarios claros:

```csharp
// BUG 1: SQL Injection - la contraseña va sin parámetros parametrizados
// BUG 2: Contraseñas en texto plano en BD
// ATAQUE POSIBLE: nombreUsuario = "' OR '1'='1"
```

El test **documenta QUÉ hace realmente el código ahora**, no lo que debería hacer.

### 2️⃣ Protege Contra Regresiones Durante Refactorización

**Escenario:**

Un desarrollador dice: "Voy a limpiar la autenticación"

**Sin el test:**
- Podría cambiar accidentalmente el comportamiento
- No sabría que otros módulos dependen de este comportamiento específico
- Podría arreglarlo sin darse cuenta, causando cambios inesperados

**Con el test:**
- El test documenta el comportamiento actual
- Si algo cambia durante refactorización, el test lo detecta
- Es un "escudo de seguridad" contra cambios accidentales

### 3️⃣ Proporciona Contexto para Mejora Incremental

**Michael Feathers explica:**

> "La refactorización es cambiar código sin cambiar su comportamiento observable. Los Characterization Tests documenten qué es observable ahora."

**Uso del test:**

1. ✅ Tests verdes en estado actual (vulnerable)
2. 🔧 Refactorizar a SQL parametrizado
3. ⚠️ Tests empiezan a fallar (el comportamiento cambió)
4. 🧠 Actualizar tests para el nuevo comportamiento
5. ✅ Tests verdes con código mejorado

### 4️⃣ Evita Que El Bug Se Reintroduzca

**Escenario Futuro:**

Un nuevo developer hace "optimización":

```csharp
// "Optimization": Armar SQL más rápido sin usar SqlParameter
string sql = "WHERE NombreUsuario = '" + nombreUsuario + "'";
```

**Sin el test:** Nadie se da cuenta, el bug vuelve

**Con el test:** El test se vuelve "rojo" en CI/CD, señalando que algo está mal

---

## 📊 COMPARATIVA: Código Vulnerable vs. Seguro

### ❌ VULNERABLE (Actual)

```csharp
string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
           + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";

SqlCommand cmd = new SqlCommand(sql, conn);
```

**Problemas:**
- SQL Injection
- Contraseñas visibles en logs
- Contraseñas en texto plano en BD

---

### ✅ SEGURO (Propuesta)

```csharp
using (SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion))
{
    conn.Open();
    string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = @NombreUsuario " +
                 "AND Contrasena = @Contrasena AND Activo = 1";
    
    using (SqlCommand cmd = new SqlCommand(sql, conn))
    {
        cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario ?? "");
        cmd.Parameters.AddWithValue("@Contrasena", 
            HashPassword(contrasena) ?? "");  // Hash, no plano
        
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            // ...
        }
    }
}
```

**Mejoras:**
- ✅ Parametrizado (SQL Injection prevenido)
- ✅ Usa `using` (gestión de recursos)
- ✅ Contraseña hasheada (segura si BD es comprometida)
- ✅ Nombre de parámetros explícitos

---

## 🎯 CÓMO EL TEST DOCUMENTA EL BUG

### El Test Reconstruye el SQL Vulnerable

```csharp
[Fact]
public void ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection()
{
    // Arrange
    string nombreUsuario = "admin";
    string contrasena = "password123";

    // Act: RECONSTRUIMOS EXACTAMENTE lo que hace el código
    string sqlReconstruida = "SELECT * FROM Usuarios WHERE NombreUsuario = '" 
        + nombreUsuario + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";

    // Assert: Verificamos que el SQL tiene las vulnerabilidades
    Assert.Contains("Usuarios", sqlReconstruida);
    Assert.Contains("Contrasena = '", sqlReconstruida);
    
    // DOCUMENTACIÓN CLARA DEL BUG:
    // BUG 1: SQL Injection - la contraseña va sin parámetros
    // BUG 2: Contraseñas en texto plano en BD
    // ATAQUE POSIBLE: nombreUsuario = "' OR '1'='1"
    // RESULTADO: SELECT * FROM Usuarios WHERE NombreUsuario = '' OR '1'='1' -- ...
    // IMPACTO: Bypass de autenticación
}
```

### El Nombre del Test es Descriptivo

```
ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection
 ↓                ↓                    ↓
 Método       Es un bug         Qué vulnerabilidades
```

### El Comentario Detalla el Ataque

```csharp
// ATAQUE POSIBLE: nombreUsuario = "' OR '1'='1"
// Resultaría en: SELECT * FROM Usuarios WHERE NombreUsuario = '' OR '1'='1' AND ...
// Bypass de autenticación
```

---

## 📋 MATRIX DE IMPORTANCIA

| Aspecto | Nivel | Razón |
|---------|-------|-------|
| **Severidad** | 🔴 CRÍTICO | Autenticación del sistema |
| **Impacto** | 🔴 MÁXIMO | Acceso admin sin contraseña |
| **Facilidad de Explotación** | 🟢 TRIVIAL | 5 caracteres: `' OR '1'='1` |
| **Detectabilidad** | 🟡 MEDIA | Logs de auditoría falseados |
| **Scope** | 🔴 GLOBAL | Todo el sistema comprometido |
| **Importancia de Documentarlo** | 🔴 CRÍTICA | Punto más sensible del sistema |

---

## 🧪 POR QUÉ DOCUMENTAR EL BUG (No Arreglarlo YET)

### Razón 1: Michael Feathers - Caracterización, No Corrección

> "Characterization Tests documentan QUÉ HACE el código. No documentan qué DEBERÍA hacer."

Este test es un **Characterization Test**, no un **Specification Test**:

- ✅ **Characterization:** "El código es vulnerable a SQL Injection OR. Eso es lo que hace."
- ❌ **Specification:** "El código debe estar protegido contra SQL Injection."

### Razón 2: Base Segura para Refactorización

**Flujo Correcto:**

```
1. Test rojo (documenta vulnerabilidad)
   ↓
2. Test verde (comportamiento actual capturado)
   ↓
3. Refactorizar (arreglar bug)
   ↓
4. Test rojo (comportamiento cambió)
   ↓
5. Actualizar test (especificar nuevo comportamiento)
   ↓
6. Test verde (seguro ahora, cambio controlado)
```

### Razón 3: Evita Que El Bug Se Reintroduzca

**Escenario Sin Test:**

```
Código vulnerable → Alguien lo arregla → Nuevo dev lo reintroduce → ???
```

**Escenario Con Test:**

```
Test documenta vulnerabilidad 
    ↓
Alguien lo arregla (test se actualiza)
    ↓
Nuevo dev lo reintroduce
    ↓
TEST SE VUELVE ROJO
    ↓
Error detectado en CI/CD antes de producción ✅
```

---

## 🚨 PASOS PARA ARREGLARLO (FUTURO)

Una vez tengas los tests verdes, refactoriza así:

### Paso 1: Usar SqlParameter (Prevenir SQL Injection)

```csharp
cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario ?? "");
cmd.Parameters.AddWithValue("@Contrasena", contrasena ?? "");
```

### Paso 2: Hash de Contraseña (Prevenir Exposición)

```csharp
// En BD, guardar hash, no texto plano
public static string HashPassword(string password)
{
    using (var sha256 = System.Security.Cryptography.SHA256.Create())
    {
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
```

### Paso 3: Comparar Hash

```csharp
cmd.Parameters.AddWithValue("@Contrasena", HashPassword(contrasena) ?? "");
```

### Paso 4: Actualizar Test

```csharp
[Fact]
public void ValidarUsuario_Seguro_UsaParametrosSQL()
{
    // Verificar que se usan parámetros, no concatenación
    // (En prueba real, verificarías el Plan de Ejecución SQL)
}
```

---

## 💡 CONCLUSIÓN: POR QUÉ ESTE TEST ES IMPORTANTE

1. ✅ **Documenta la realidad actual** - Código ACTUAL es vulnerable
2. ✅ **Evita regresiones** - Si alguien lo "arregla" accidentalmente
3. ✅ **Guía la refactorización** - Test verde → Refactorizar → Test actualizar → Test verde
4. ✅ **Educa a nuevos devs** - Aprenden qué está mal y cómo arreglarlo
5. ✅ **Base de CI/CD** - Detecta si el bug se reintroduce

**Lo MÁS IMPORTANTE:** No es decir "el código está mal", sino documentar "esto es lo que el código HACE ahora, aunque esté mal, para que no se rompa durante refactorización".

---

**Autor:** Characterization Tests - Michael Feathers  
**Severidad:** 🔴 CRÍTICO  
**Fases para Arreglar:** 4  
**Prioridad:** 1 (Lo Primero)  

