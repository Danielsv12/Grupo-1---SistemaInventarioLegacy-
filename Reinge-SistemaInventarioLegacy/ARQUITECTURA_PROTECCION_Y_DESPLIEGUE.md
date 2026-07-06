# 🏛️ ARQUITECTURA DE PROTECCIÓN - Reingeniería Segura del Proyecto

## 📋 ÍNDICE

1. [Los 4 Elementos de Protección](#los-4-elementos)
2. [Cómo Protegen la Reingeniería](#cómo-protegen)
3. [Estrategias de Despliegue](#estrategias)
4. [Estrategia Recomendada](#recomendada)
5. [Implementación Paso a Paso](#implementación)
6. [Matriz de Decisión](#matriz)

---

# 🛡️ LOS 4 ELEMENTOS DE PROTECCIÓN

## 1️⃣ CONTENEDORES (Docker)

### ¿QUÉ SON?

```
Contenedor = Caja aislada con:
├─ Código de aplicación
├─ Runtime (.NET, SQL Server)
├─ Dependencias
├─ Configuración
└─ Ambiente reproducible
```

### PROTECCIÓN QUE OFRECE

```
ANTES (Sin contenedores):
┌─────────────────────────────────────┐
│ Dev     Staging    Production       │
│ ✗       ✗          ✗                │
│ "Funciona en mi PC"                 │
│ Errores de ambiente                 │
│ Dependencias incompatibles          │
│ Imposible rollback rápido           │
└─────────────────────────────────────┘

DESPUÉS (Con contenedores):
┌─────────────────────────────────────┐
│ Dev     Staging    Production       │
│ ✅      ✅         ✅               │
│ Mismo ambiente en todos            │
│ Sin "works on my machine"           │
│ Reproducible 100%                  │
│ Rollback instantáneo (cambiar tag) │
└─────────────────────────────────────┘
```

### PROTECCIÓN EN REINGENIERÍA

**Escenario: Refactorizar AccesoDatos.cs**

```
Paso 1: Crear imagen con código legacy
    docker build -t inventario:v1 .
    
Paso 2: Refactorizar (parametrizar SQL)
    Cambiar: string sql = "... + var ..."
    Por:     cmd.Parameters.AddWithValue()
    
Paso 3: Crear imagen con código nuevo
    docker build -t inventario:v2 .
    
Paso 4: Si falla en staging
    docker compose up inventario:v1  ← Rollback instantáneo
    
Paso 5: Si funciona
    docker compose up inventario:v2  ← Deploy en prod
```

**Ventajas:**
- ✅ Rollback en segundos (no horas)
- ✅ Ambientes idénticos (dev = prod)
- ✅ Aislar errores de refactorización
- ✅ Versionar exactamente qué se deployó

---

## 2️⃣ CI/CD (Pipeline Automatizado)

### ¿QUÉ ES?

```
CI/CD = Cadena automatizada:
├─ Build (compilar)
├─ Test (ejecutar tests)
├─ Scan (seguridad)
├─ Deploy (desplegar)
└─ Monitor (vigilar)
```

### PROTECCIÓN QUE OFRECE

```
MANUAL (Sin CI/CD):
Developer:
  ├─ Compilar localmente
  ├─ Ejecutar tests en su PC
  ├─ Deployar manualmente
  ├─ Si falla, qué pasó?
  └─ ¿Está todo en prod?

AUTOMATIZADO (Con CI/CD):
git push → Pipeline:
  ├─ Build automático
  ├─ Tests automáticos (todos)
  ├─ Scan de seguridad
  ├─ Calidad de código
  ├─ Deploy automático
  └─ Notificación si falla
```

### PROTECCIÓN EN REINGENIERÍA

**Escenario: Refactorizar ValidarUsuario.cs (SQL Injection)**

```
Developer hace git push con cambio:

ANTES (manual):
    git push
    → Espera 10 minutos?
    → ¿Alguien va a testear?
    → Deploy manual
    → Oops, se olvidó un parámetro
    → SQL Injection sigue ahí
    
DESPUÉS (CI/CD):
    git push
    → 30 segundos: Build automático
    → 1 minuto: Tests automáticos TODOS
    → 15 segundos: Scan de seguridad
    → Detecta: "SQL Injection aún presente"
    → ❌ BLOQUEA el push automáticamente
    → Notifica al developer
    → Developer arregla
    → Próximo push: ✅ Pasa todo
```

**Ventajas:**
- ✅ Detecta regresiones inmediatamente
- ✅ Tests de caracterización fallan si reintroduzco bugs
- ✅ Garantiza que código deployado pasó todos los tests
- ✅ Auditoría de quién deployó qué y cuándo

---

## 3️⃣ TESTS DE CARACTERIZACIÓN

### ¿QUÉ SON?

```
Tests de Caracterización = Especificación del comportamiento actual
├─ Capturan QUÉ HACE el código ahora
├─ Incluyendo bugs conocidos
├─ Documentan SQL Injection
├─ Documentan falta de validación
└─ Protegen contra regresiones
```

### PROTECCIÓN QUE OFRECE

```
ANTES (Sin tests):
    Developer refactoriza
    → Cambia SQL concatenación a parámetros
    → "Creo que está bien"
    → Deploy
    → SQL Injection está arreglado (OK)
    → Pero cambió el comportamiento
    → Otros módulos rompen

DESPUÉS (Con tests):
    Developer refactoriza
    → Cambia SQL concatenación
    → Ejecuta: dotnet test
    → 70 tests pasan
    → Test: "SqlInjectionExistsInValidarUsuario" falla
    → "¡Espera, ese comportamiento cambió!"
    → Developer revisa qué pasó
    → Es el arreglo, es seguro
    → Actualiza el test
    → Tests pasan
    → Deploy seguro
```

### PROTECCIÓN EN REINGENIERÍA

**Los 70+ tests de caracterización protegen porque:**

```
✅ Capturan estado actual (incluyendo bugs)
   → Si accidentalmente "arreglas" un bug sin propósito
   → Test falla inmediatamente

✅ Documentan comportamiento esperado
   → SQL Injection existe → Test lo documenta
   → Si intentas remover inyección sin refactorizar
   → Test falla (queremos refactorizar pero con control)

✅ Detectan cambios no intencionales
   → Refactorizo AccesoDatos.cs
   → Accidentalmente cambio lógica de negocio
   → Tests de MapeoProducto fallan
   → Detecto el problema ANTES de producción

✅ Permiten refactorización segura
   → Versión actual: Tests verdes (con bugs)
   → Refactorizo: Tests rojo (comportamiento cambió)
   → Arreglo tests: Tests verdes (comportamiento preservado)
   → Cambio es transparente, seguro, auditable
```

---

## 4️⃣ MIGRACIÓN CON ROLLBACK

### ¿QUÉ ES?

```
Migración con Rollback = Despliegue reversible:
├─ Deploy versión nueva
├─ Monitorear
├─ Si algo falla: rollback inmediato
├─ Volver a versión anterior en segundos
└─ Cero pérdida de datos
```

### PROTECCIÓN QUE OFRECE

```
TRADICIONAL (Sin rollback rápido):
    Deploy en producción
    ├─ Si falla: "Oh no, ¿y ahora?"
    ├─ Buscar repo anterior: 30 minutos
    ├─ Compilar: 5 minutos
    ├─ Deploy manual: 10 minutos
    ├─ Usuarios vieron error durante 45 minutos
    └─ Cliente enojado

MODERNO (Con rollback):
    Deploy en producción
    ├─ Health checks fallan en 10 segundos
    ├─ Rollback automático o manual
    ├─ Revert a v1: 5 segundos
    ├─ Usuarios reciben error durante 15 segundos
    └─ RTO = 15 segundos, RPO = 0 datos perdidos
```

### PROTECCIÓN EN REINGENIERÍA

**Escenario: Migración de AccesoDatos.cs a parámetros**

```
v1 (Legacy): SQL Injection
    docker run inventario:v1
    
v2 (Nueva): SQL parametrizado
    docker build -t inventario:v2 .
    docker run inventario:v2
    
Monitoreo:
    ├─ Si v2 funciona → Excelente
    ├─ Si v2 falla → Rollback automático en 5 segundos
    └─ Usuarios no ven downtime
```

**Con Caracterización Tests:**

```
v1 → deploy → tests pasan (incluyendo SQL Injection)
v2 → deploy → tests pasan (parametrizado, sin injection)
    → Si hubiera regresión, tests fallarían
    → Rollback automático

Garantía: Código deployado siempre pasó tests
```

---

# 🛡️ CÓMO PROTEGEN LA REINGENIERÍA

## MATRIZ DE PROTECCIÓN

```
                    Contenedor  CI/CD  Tests  Rollback
Ambiente diferente     ✅        ✅      ✅      ✅
Código roto           ❌        ✅      ✅      ✅
Downtime              ❌        ❌      ❌      ✅
Regresiones           ❌        ✅      ✅      ❌
SQL Injection         ❌        ❌      ✅      ❌
Auditoría            ✅        ✅      ✅      ✅
Versiones            ✅        ✅      ❌      ✅
```

---

## ESCENARIO REAL: Refactorizar ValidarUsuario (SQL Injection)

### Sin Protección (Riesgo Máximo)

```
Developer:
  └─ Edita ValidarUsuario.cs
     └─ Cambia: string sql = "... + password" 
        a: cmd.Parameters.AddWithValue()
     
     └─ Prueba localmente: "Parece que funciona"
     
     └─ Deploy manual a producción
     
     └─ 3 horas después: 
        ├─ Descubren que cambió comportamiento
        ├─ Otros módulos rompen
        ├─ Clientes enojados
        ├─ Rollback manual: 1 hora más
        └─ 4 horas de downtime
```

### CON PROTECCIÓN (Riesgo Mínimo)

```
Developer:
  ├─ Edita ValidarUsuario.cs
  │  └─ Cambia SQL Injection → Parámetros
  │
  ├─ git push
  │  └─ CI/CD Pipeline:
  │     ├─ Build automático ✅
  │     ├─ 70 tests caracterizan: 
  │     │  ├─ ValidarUsuario_BUG_SQLInjection
  │     │  │  └─ Falla (comportamiento cambió) ← Detectado
  │     │  ├─ ObtenerProductos_Mapping
  │     │  └─ ... 68 tests más
  │     │
  │     ├─ Scan de seguridad:
  │     │  └─ SQL Injection: No detectada ✅ (Arreglada)
  │     │
  │     ├─ Análisis de código:
  │     │  └─ Usa SqlParameter ✅
  │     │
  │     └─ Bloquea merge: "Actualiza tests"
  │
  ├─ Developer:
  │  ├─ Ve que test falló porque comportamiento cambió (INTENCIONAL)
  │  ├─ Lee comentario del test
  │  ├─ Entiende que SQL Injection fue arreglado
  │  ├─ Actualiza test para nuevo comportamiento
  │  ├─ git push nuevamente
  │
  ├─ CI/CD Pipeline (intento 2):
  │  ├─ Tests: ✅ Todos pasan
  │  ├─ Seguridad: ✅ Sin SQL Injection
  │  ├─ Calidad: ✅ Parámetros usados
  │  └─ Aprueba automáticamente
  │
  ├─ Deploy en staging:
  │  ├─ Contenedor v2 con código nuevo
  │  ├─ Tests de integración automáticos
  │  └─ ✅ Validación manual
  │
  ├─ Deploy en producción (Blue-Green):
  │  ├─ Ambiente BLUE (v1, viejo) sigue sirviendo
  │  ├─ Ambiente GREEN (v2, nuevo) inicia
  │  ├─ Health checks GREEN: ✅
  │  ├─ Cambio router: BLUE → GREEN
  │  └─ Rollback instantáneo disponible si es necesario
  │
  └─ Monitoreo:
     ├─ 0 downtime
     ├─ Transición sin errores
     ├─ Auditoria: quién, qué, cuándo, por qué
     └─ ✅ Refactorización segura completada
```

---

## PROTECCIÓN POR CAPAS

```
┌─────────────────────────────────────────────────┐
│ CAPA 1: Tests de Caracterización               │
│ ├─ Detectan cambios no intencionales            │
│ ├─ Documentan bugs conocidos                   │
│ └─ Especifican comportamiento esperado         │
├─────────────────────────────────────────────────┤
│ CAPA 2: CI/CD Pipeline                         │
│ ├─ Valida que tests pasen                      │
│ ├─ Ejecuta security scan                       │
│ ├─ Verifica calidad de código                  │
│ └─ Bloquea merge si algo falla                 │
├─────────────────────────────────────────────────┤
│ CAPA 3: Contenedores                           │
│ ├─ Empaquetan código + dependencias            │
│ ├─ Versiona exactamente qué se deployó        │
│ ├─ Permite rollback instantáneo                │
│ └─ Reproducible 100% en todos lados            │
├─────────────────────────────────────────────────┤
│ CAPA 4: Estrategia de Despliegue               │
│ ├─ Blue-Green: cero downtime                   │
│ ├─ Canary: monitoreo gradual                   │
│ ├─ Rolling: actualización sin parar            │
│ └─ Rollback automático si algo falla           │
└─────────────────────────────────────────────────┘
```

---

# 🚀 ESTRATEGIAS DE DESPLIEGUE

## ESTRATEGIA #1: BLUE-GREEN

### ¿QUÉ ES?

```
Dos ambientes idénticos:

AZUL (Actual)        VERDE (Nuevo)
├─ v1 (Legacy)       ├─ v2 (Refactorizado)
├─ Sirviendo 100%    ├─ Pruebas exhaustivas
├─ Tráfico 100%      ├─ Cero tráfico
└─ Estable           └─ En espera

Cambio de router:
AZUL → 100% → 0%
VERDE → 0% → 100%

Tiempo: 5 segundos
Rollback: 5 segundos (cambio router inverso)
```

### DIAGRAMA

```
Usuario → [Load Balancer] → AZUL (v1)  ← Tráfico 100%
                         ↘ VERDE (v2) ← Esperando

Después del cambio:
Usuario → [Load Balancer] → AZUL (v1)  ← Esperando
                         ↘ VERDE (v2) ← Tráfico 100%

Rollback (si falla):
Usuario → [Load Balancer] → AZUL (v1)  ← Tráfico 100%
                         ↘ VERDE (v2) ← Esperando
```

### VENTAJAS

```
✅ Cero downtime durante cambio
✅ Rollback instantáneo (5 segundos)
✅ Pruebas exhaustivas en VERDE antes de cambiar
✅ Estado consistente (no hay medio punto)
✅ Fácil de entender y implementar
```

### DESVENTAJAS

```
❌ Requiere 2x recursos (2 ambientes completos)
❌ Necesita manejo de BD (migraciones)
❌ Rollback no undo cambios de estado
❌ Caro en infrastructure
```

### IMPLEMENTACIÓN CON TU PROYECTO

```yaml
# docker-compose-blue.yml (v1 - Actual)
services:
  app:
    image: inventario:v1
    ports:
      - "8080:8080"
  db:
    image: mssql:2022
    ports:
      - "1433:1433"

# docker-compose-green.yml (v2 - Nuevo)
services:
  app:
    image: inventario:v2
    ports:
      - "8081:8080"  ← Puerto diferente
  db:
    image: mssql:2022
    ports:
      - "1434:1433"  ← Puerto diferente

# nginx.conf (Router)
upstream blue {
  server localhost:8080;
}

upstream green {
  server localhost:8081;
}

server {
  listen 80;
  
  # Cambiar entre azul/verde editando esto:
  location / {
    proxy_pass http://blue;  # ← Cambiar a http://green
  }
}
```

### PASOS DE DESPLIEGUE

```
1. PREPARAR VERDE (ambiente nuevo)
   docker-compose -f docker-compose-green.yml up -d
   
2. PRUEBAS EXHAUSTIVAS EN VERDE
   curl http://localhost:8081/health
   sqlcmd -S localhost:1434 -U sa -Q "SELECT 1"
   Ejecutar tests: dotnet test
   
3. SI TODO OK
   Cambiar nginx.conf: blue → green
   nginx reload
   
4. MONITOREAR
   Logs de VERDE
   Health checks
   
5. SI ALGO FALLA
   nginx.conf: green → blue
   nginx reload
   
   VERDE sigue corriendo para debug
   AZUL nuevamente sirviendo (rollback 5 segundos)
```

---

## ESTRATEGIA #2: CANARY

### ¿QUÉ ES?

```
Despliegue gradual:

Usuarios:
├─ 95% → v1 (Actual)
├─ 5% → v2 (Nuevo) ← Grupo de prueba
└─ Monitorear errores en v2

Si v2 está bien:
├─ 80% → v1
├─ 20% → v2
└─ ...

Hasta:
├─ 0% → v1
└─ 100% → v2
```

### DIAGRAMA

```
Etapa 1 (5% canario):
Usuarios 100% → [Router] → v1 (95%)
                         ↘ v2 (5%) ← Monitorear

Etapa 2 (20%):
Usuarios 100% → [Router] → v1 (80%)
                         ↘ v2 (20%)

Etapa 3 (100%):
Usuarios 100% → [Router] → v2 (100%)

Si falla en Etapa 1:
Revert → v1 (100%)
Rollback: 10 segundos, solo 5% afectado
```

### VENTAJAS

```
✅ Riesgo mínimo (solo 5% afectado inicialmente)
✅ Monitoreo real de usuarios reales
✅ Rollback rápido si falla
✅ Menos recursos que Blue-Green
✅ Detecta problemas con BD real (no mocks)
```

### DESVENTAJAS

```
❌ Más lento (despliegue gradual 1-2 horas)
❌ Monitoreo complejo (análisis de métricas)
❌ Usuarios ven posibles errores (aunque pocos)
❌ Requiere herramientas de observabilidad
❌ Rollback puede afectar datos (sesiones incompletas)
```

### IMPLEMENTACIÓN CON TU PROYECTO

```yaml
# docker-compose.yml
services:
  app-v1:
    image: inventario:v1
    ports:
      - "8080:8080"
    environment:
      VERSION: v1
      
  app-v2:
    image: inventario:v2
    ports:
      - "8081:8080"
    environment:
      VERSION: v2

# nginx.conf (Router con Canary)
upstream v1 {
  server localhost:8080 weight=95;  # 95% tráfico
}

upstream v2 {
  server localhost:8081 weight=5;   # 5% tráfico
}

server {
  listen 80;
  
  location / {
    proxy_pass http://v1;
    # Etapa 1: 95% v1, 5% v2
    
    # Etapa 2: cambiar a
    # proxy_pass http://v2; weight=20
    # ...
    
    # Etapa 3: cambiar a
    # proxy_pass http://v2; weight=100
  }
}
```

### PASOS DE DESPLIEGUE

```
Etapa 1 (5% Canario):
  1. Desplegar v2 en paralelo
     docker-compose up -d app-v2
  
  2. Configurar nginx: 5% → v2
     nginx reload
  
  3. Monitorear (5 minutos):
     - Logs de v2
     - Error rate en v2
     - Database queries
     - ¿Errores de SQL?
  
  4. Si OK → Etapa 2
     Si falla → Rollback (nginx 0% v2)

Etapa 2 (20%):
  nginx.conf: weight=20
  nginx reload
  Monitorear (10 minutos)
  ...

Etapa 3 (100%):
  nginx.conf: weight=100
  nginx reload
  v1 puede bajar después
```

---

## ESTRATEGIA #3: ROLLING

### ¿QUÉ ES?

```
Actualización gradual de instancias:

Instancias: 5 × app
├─ app-1: v1 → Stop → v2 ← (1/5 actualizada)
├─ app-2: v1 (4/5 todavía sirviendo)
├─ app-3: v1
├─ app-4: v1
└─ app-5: v1

Repeats:
├─ app-2: v1 → Stop → v2 ← (2/5 actualizada)
├─ app-3: v1 → Stop → v2 ← (3/5 actualizada)
...

Hasta: Todas v2
```

### DIAGRAMA

```
Antes:
[v1] [v1] [v1] [v1] [v1]  ← 5 instancias v1

Paso 1:
[v2] [v1] [v1] [v1] [v1]  ← 4 instancias sirviendo

Paso 2:
[v2] [v2] [v1] [v1] [v1]  ← 3 instancias sirviendo

Paso 3:
[v2] [v2] [v2] [v1] [v1]  ← 2 instancias sirviendo

Paso 4:
[v2] [v2] [v2] [v2] [v1]  ← 1 instancia sirviendo

Paso 5:
[v2] [v2] [v2] [v2] [v2]  ← Todas v2
```

### VENTAJAS

```
✅ Cero downtime (siempre hay instancias sirviendo)
✅ Rollback más fácil (baja app-n y vuelve v1)
✅ Eficiente en recursos (no necesita 2x)
✅ Fácil de automatizar con Kubernetes
✅ Compatible con load balancing automático
```

### DESVENTAJAS

```
❌ Requiere múltiples instancias (infraestructura)
❌ Compatible solo con aplicaciones stateless
❌ Despliegue más lento (secuencial)
❌ Complejidad si hay afinidad de sesión
❌ Rollback puede ser más lento
```

### IMPLEMENTACIÓN CON TU PROYECTO

```yaml
# docker-compose-rolling.yml
version: '3.8'

services:
  app-1:
    image: inventario:v1
    container_name: app-1
    ports:
      - "8001:8080"
  
  app-2:
    image: inventario:v1
    container_name: app-2
    ports:
      - "8002:8080"
  
  app-3:
    image: inventario:v1
    container_name: app-3
    ports:
      - "8003:8080"
  
  app-4:
    image: inventario:v1
    container_name: app-4
    ports:
      - "8004:8080"
  
  app-5:
    image: inventario:v1
    container_name: app-5
    ports:
      - "8005:8080"
  
  nginx:
    image: nginx
    ports:
      - "80:80"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
    depends_on:
      - app-1
      - app-2
      - app-3
      - app-4
      - app-5

# nginx.conf
upstream apps {
  server app-1:8080;
  server app-2:8080;
  server app-3:8080;
  server app-4:8080;
  server app-5:8080;
}

server {
  listen 80;
  location / {
    proxy_pass http://apps;
  }
}
```

### PASOS DE DESPLIEGUE

```
Etapa 1: Actualizar app-1
  docker-compose up -d --force-recreate -d \
    --build -d \
    --no-deps app-1
  
  # Cambiar imagen en compose a v2
  sed -i 's/inventario:v1/inventario:v2/g' docker-compose.yml
  
  # Re-crear solo app-1 con v2
  docker-compose up -d --force-recreate app-1
  
  # Esperar a que arranque
  sleep 30
  
  # Verificar health
  curl http://localhost:8001/health

Etapa 2: Actualizar app-2
  docker-compose up -d --force-recreate app-2
  sleep 30
  curl http://localhost:8002/health
  
... repetir para app-3, app-4, app-5

Resultado: Todas las instancias en v2
```

---

## ESTRATEGIA #4: FEATURE FLAGS

### ¿QUÉ ES?

```
Desactivar/activar features sin redeploy:

if (FeatureFlagService.IsEnabled("UseParameterizedSQL"))
{
  // Nuevo código (parametrizado, sin SQL Injection)
}
else
{
  // Código legacy (SQL Injection)
}
```

### DIAGRAMA

```
Base de Datos (Feature Flags):
Feature: "UseParameterizedSQL"
├─ Habilitado para: 5% usuarios
├─ Habilitado para: 20% usuarios (Etapa 2)
└─ Habilitado para: 100% usuarios (Final)

En tiempo de ejecución:
Usuario login
  ├─ GetFeatureFlag("UseParameterizedSQL")
  ├─ Si = true → UseParameterizedSQL()
  └─ Si = false → UseLegacySQL()
```

### VENTAJAS

```
✅ Cero redeploy (cambio de BD solo)
✅ Rollback instantáneo (cambiar BD)
✅ A/B testing built-in
✅ Gradual roll-out sin containers nuevos
✅ Fallback automático a legacy
✅ No afecta producción mientras desarrollas
```

### DESVENTAJAS

```
❌ Código "sucio" (if-else en todas partes)
❌ Mantener dos caminos de código
❌ Gestión de BD de flags es overhead
❌ Testing más complejo (2 caminos)
❌ Técnico deuda si no limpias flags después
```

### IMPLEMENTACIÓN CON TU PROYECTO

```csharp
// FeatureFlagService.cs
public class FeatureFlagService
{
  private readonly IDatabase _db;
  
  public bool IsEnabled(string featureName, int userId = 0)
  {
    // Consultar BD de feature flags
    var flag = _db.GetFeatureFlag(featureName);
    
    if (flag == null) return false;
    if (flag.Percentage == 100) return true;
    
    // Rollout gradual: 5% usuarios
    return (userId % 100) < flag.Percentage;
  }
}

// Usar en AccesoDatos.cs (refactorización)
public static Usuario ValidarUsuario(string nombreUsuario, string contrasena)
{
  SqlConnection conn = new SqlConnection(Configuracion.CadenaConexion);
  conn.Open();
  
  if (FeatureFlagService.IsEnabled("UseParameterizedSQL"))
  {
    // ✅ NUEVO: Parametrizado (sin SQL Injection)
    string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = @usuario AND Contrasena = @pass";
    SqlCommand cmd = new SqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@usuario", nombreUsuario);
    cmd.Parameters.AddWithValue("@pass", contrasena);
    
    SqlDataReader reader = cmd.ExecuteReader();
    // ... mapeo ...
  }
  else
  {
    // ❌ LEGACY: Concatenación (SQL Injection)
    string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
               + "' AND Contrasena = '" + contrasena + "'";
    SqlCommand cmd = new SqlCommand(sql, conn);
    // ... mapeo ...
  }
}

// Base de Datos de Feature Flags
/*
INSERT INTO FeatureFlags (Name, Percentage, Enabled) VALUES
('UseParameterizedSQL', 0, 1);     -- 0% al inicio

-- Etapa 1
UPDATE FeatureFlags SET Percentage = 5 WHERE Name = 'UseParameterizedSQL';

-- Etapa 2
UPDATE FeatureFlags SET Percentage = 20 WHERE Name = 'UseParameterizedSQL';

-- Etapa 3 (100%)
UPDATE FeatureFlags SET Percentage = 100 WHERE Name = 'UseParameterizedSQL';

-- Rollback si falla
UPDATE FeatureFlags SET Percentage = 0 WHERE Name = 'UseParameterizedSQL';
*/
```

### PASOS DE DESPLIEGUE

```
Etapa 1 (5% usuarios):
  1. Código con Feature Flag ya en producción
  2. Feature Flag OFF (0%)
  3. Actualizar BD: Percentage = 5
  4. Monitorear 5% usuarios con nuevo código
  5. Si OK → siguiente etapa

Etapa 2 (20% usuarios):
  1. Update BD: Percentage = 20
  2. Monitorear
  3. Si OK → siguiente etapa

Etapa 3 (100% usuarios):
  1. Update BD: Percentage = 100
  2. Monitorear

Rollback si falla:
  1. Update BD: Percentage = 0
  2. Instantáneo, sin redeploy
```

---

# ✅ ESTRATEGIA RECOMENDADA

## 🏆 ELEGIR: BLUE-GREEN + FEATURE FLAGS

### ¿POR QUÉ?

```
Proyecto Reinge:
├─ Legacy code con bugs conocidos (SQL Injection)
├─ Refactorización es crítica
├─ Downtime = clientes enojados
├─ Tests de caracterización protegen
├─ CI/CD Pipeline valida cambios
└─ Necesita máxima confianza

Mejor estrategia: BLUE-GREEN + FEATURE FLAGS
├─ Blue-Green: Cero downtime, rollback instantáneo
├─ Feature Flags: Control granular, rollout gradual
├─ Combinadas: Máxima seguridad
```

### DIAGRAMA COMBINADO

```
┌─────────────────────────────────────────────┐
│ CONTENEDORES (Blue-Green)                   │
├─────────────────────────────────────────────┤
│ AZUL (v1)              VERDE (v2)           │
│ - Código legacy        - Código refactorizado
│ - SQL Injection        - SQL Parametrizado │
│ - Tráfico 100%         - Tráfico 0%         │
│ - Monitoreo OK         - Pruebas exhaustivas
└─────────────────────────────────────────────┘
                ↓ Cambio de router
┌─────────────────────────────────────────────┐
│ FEATURE FLAGS (Gradual Rollout)             │
├─────────────────────────────────────────────┤
│ UseParameterizedSQL:                        │
│ - Etapa 1: 5% usuarios (VERDE)              │
│ - Etapa 2: 20% usuarios                     │
│ - Etapa 3: 100% usuarios                    │
│ - Rollback: 0% usuarios (AZUL)              │
└─────────────────────────────────────────────┘
                ↓ BD Feature Flags
┌─────────────────────────────────────────────┐
│ CI/CD PIPELINE                              │
├─────────────────────────────────────────────┤
│ - 70+ Tests de Caracterización PASAN        │
│ - SQL Injection eliminado ✅                │
│ - Comportamiento preservado ✅              │
│ - Security scan ✅                          │
│ - Code quality ✅                           │
└─────────────────────────────────────────────┘
                ↓ Auditoría completa
┌─────────────────────────────────────────────┐
│ ROLLBACK (si algo falla)                    │
├─────────────────────────────────────────────┤
│ - Opción 1: Cambio router (5 segundos)      │
│ - Opción 2: Feature Flag OFF (1 segundo)    │
│ - Opción 3: Ambas (máxima seguridad)        │
└─────────────────────────────────────────────┘
```

---

## PROTECCIÓN EN 4 CAPAS

```
Capa 4: FEATURE FLAGS
  ├─ Rollout gradual 5%→20%→100%
  ├─ Monitoreo de usuarios reales
  └─ Rollback en 1 segundo

        ↓

Capa 3: BLUE-GREEN
  ├─ Cero downtime durante cambio
  ├─ Cambio router en 5 segundos
  └─ Versiones completas aisladas

        ↓

Capa 2: CI/CD PIPELINE
  ├─ 70+ tests caracterizan el cambio
  ├─ Detectan regresiones
  ├─ Security scan automático
  └─ Bloquea si algo falla

        ↓

Capa 1: CONTENEDORES
  ├─ Ambiente reproducible
  ├─ Versión exacta deployada
  └─ Rollback a versión anterior
```

---

## IMPLEMENTACIÓN PASO A PASO

### Fase 1: Preparación (1 semana)

```
1. Código con Feature Flag compilado
   ├─ UseParameterizedSQL
   ├─ IF flag → Nuevo código
   ├─ ELSE → Legacy código
   └─ Ambos paths funcionan

2. Tests actualizados
   ├─ Compatibles con ambos caminos
   ├─ Verifican comportamiento preservado
   └─ 70+ tests PASAN

3. CI/CD validando
   ├─ Build exitoso
   ├─ Tests OK
   ├─ Security scan OK
   └─ Listo para desplegar

4. Contenedores preparados
   ├─ inventario:v2 construida
   ├─ docker-compose-blue.yml (actual)
   ├─ docker-compose-green.yml (nuevo)
   └─ nginx.conf con routing
```

### Fase 2: Despliegue en Staging (1 día)

```
1. Desplegar VERDE en staging
   docker-compose -f docker-compose-green.yml up -d

2. Feature Flag: 0% (desactivada)
   UPDATE FeatureFlags SET Percentage = 0
   
3. Pruebas exhaustivas
   ├─ Código legacy activado (FeatureFlag=OFF)
   ├─ Debe funcionar igual que AZUL
   ├─ Validar BD, API, tests
   └─ 24 horas de monitoreo

4. Activar Feature Flag: 5%
   UPDATE FeatureFlags SET Percentage = 5
   
5. Monitorear 5% usuarios
   ├─ Error rate
   ├─ Response time
   ├─ SQL queries
   └─ Logs

6. Si OK → 20%, luego 100%

7. Si FALLA → Rollback a 0%
```

### Fase 3: Despliegue en Producción (1 día)

```
1. Cambio router: AZUL → VERDE
   nginx: blue → green
   Esto SIRVE v2, pero Feature Flag=0% (legacy)
   
2. Monitoreo (5 minutos)
   ├─ Health checks VERDE
   ├─ Error rate: 0%
   ├─ Usuarios no ven cambio (legacy code)
   └─ Si OK, continuar

3. Feature Flag: 5%
   UPDATE FeatureFlags SET Percentage = 5
   
4. Monitorear (5 minutos)
   ├─ 5% usuarios ven nuevo código
   ├─ Error rate, response time
   ├─ ¿SQL Injection detectada?
   └─ Si OK, continuar

5. Feature Flag: 20%
   Monitorear (5 minutos)
   
6. Feature Flag: 100%
   ├─ Todos usuarios: código nuevo
   ├─ Monitoreo exhaustivo (1 hora)
   └─ Si OK, limpiar AZUL

7. Refactorización completada ✅
   ├─ Código parametrizado
   ├─ SQL Injection eliminado
   ├─ Comportamiento preservado
   ├─ Tests pasan
   ├─ Cero downtime
   └─ Auditoría completa
```

### Rollback (Si algo falla)

**Opción 1: Rollback vía Feature Flag (1 segundo)**
```sql
UPDATE FeatureFlags SET Percentage = 0
-- Instantáneo, todos usuarios: código legacy
```

**Opción 2: Rollback vía Blue-Green (5 segundos)**
```bash
# nginx.conf: green → blue
# nginx reload
# Tráfico vuelve a AZUL v1
```

**Opción 3: Ambos (máxima seguridad)**
```
1. Feature Flag → 0% (usuarios ven legacy)
2. Router → AZUL (tráfico a v1)
3. Downtime: 0 segundos
4. Data: Preservada 100%
```

---

# 📊 MATRIZ DE DECISIÓN

## ¿Cuándo usar cada estrategia?

```
                    Blue-Green  Canary  Rolling  Flags
Downtime            0 seg       0 seg   0 seg    0 seg
Rollback            5 seg       10 seg  30 seg   1 seg
Recursos            2x          1x      1x       1x
Complejidad         Media       Alta    Media    Alta
Riesgo              Bajo        Bajo    Medio    Medio
Refactorización     ✅          ✅      ⚠️       ✅
SQL Injection       ✅          ✅      ⚠️       ✅
Tests Críticos      ✅          ✅      ✅       ✅
Contenedores        ✅          ✅      ✅       ✅
```

---

# 🎯 CONCLUSIÓN

## Para Reinge (Refactorización Legacy con SQL Injection)

### Recomendación: **BLUE-GREEN + FEATURE FLAGS**

**Por qué:**
1. **Máxima seguridad:** Dos despliegues independientes
2. **Cero downtime:** Cambio instantáneo con rollback
3. **Control granular:** Rollout 5%→20%→100%
4. **Protección en capas:** Contenedores + CI/CD + Tests + Despliegue
5. **Auditoría completa:** Quién, qué, cuándo, por qué
6. **Reversibilidad:** Rollback en 1-5 segundos si algo falla

**Ventajas específicas para tu proyecto:**
- ✅ Refactorizar SQL Injection sin downtime
- ✅ Monitorear con usuarios reales
- ✅ Tests de caracterización validan cambios
- ✅ Rollback automático si falla
- ✅ Costo de error: mínimo
- ✅ Confianza máxima en producción

---

**Generado:** Arquitectura de Protección + Estrategias de Despliegue  
**Para:** Refactorización de Código Legacy  
**Status:** Listo para implementación  

