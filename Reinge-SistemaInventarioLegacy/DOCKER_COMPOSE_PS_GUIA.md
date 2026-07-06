# 📊 DOCKER COMPOSE PS - GUÍA COMPLETA

## 🚀 ¿QUÉ ES `docker compose ps`?

Comando que muestra el **estado actual de todos los servicios** en tu docker-compose.

```bash
docker-compose ps
```

---

## 📋 SALIDA TÍPICA

### Estado Ideal ✅

```
NAME                     IMAGE                                      COMMAND             SERVICE      STATUS              PORTS
reinge-app               reinge/inventario:latest                  "dotnet Reinge..."  app          Up 1 minute (healthy)   0.0.0.0:8080->8080/tcp
reinge-sqlserver         mcr.microsoft.com/mssql/server:2022-latest "/opt/mssql/b..."  db           Up 2 minutes (healthy)  0.0.0.0:1433->1433/tcp
```

**Interpretación:**
```
✅ 2 servicios corriendo
✅ Ambos "Up" (iniciados)
✅ Ambos "(healthy)" (health checks pasando)
✅ Puertos mapeados correctamente
```

---

## 🔍 EXPLICACIÓN DE CADA COLUMNA

### 1. NAME (Nombre del Contenedor)

```
reinge-app
reinge-sqlserver
```

**Qué es:** Nombre único del contenedor  
**Generado por:** `${COMPOSE_PROJECT_NAME}-${SERVICE_NAME}`  
**En tu .env:** `COMPOSE_PROJECT_NAME=reinge`

---

### 2. IMAGE (Imagen Docker)

```
reinge/inventario:latest
mcr.microsoft.com/mssql/server:2022-latest
```

**reinge/inventario:latest**
- `reinge` = Namespace/organización
- `inventario` = Nombre de imagen
- `latest` = Tag (versión)

**mcr.microsoft.com/mssql/server:2022-latest**
- `mcr.microsoft.com` = Microsoft Container Registry
- `mssql/server` = SQL Server
- `2022-latest` = Versión 2022, última compilación

---

### 3. COMMAND (Comando que ejecuta)

```
"dotnet Reinge..."
"/opt/mssql/b..."
```

**Para app (.NET):**
```
dotnet Reinge-SistemaInventarioLegacy.dll
```

**Para BD (SQL Server):**
```
/opt/mssql/bin/sqlservr
```

**Qué significa:** El comando que corre cuando el contenedor inicia

---

### 4. SERVICE (Servicio del compose)

```
app
db
```

**Nombres de los servicios en docker-compose.yml:**
```yaml
services:
  app:        ← Service name
    ...
  db:         ← Service name
    ...
```

---

### 5. STATUS (Estado)

```
Up 1 minute (healthy)
Up 2 minutes (healthy)
```

**Partes:**
- `Up` = Contenedor corriendo
- `1 minute` = Tiempo desde que inició
- `(healthy)` = Health check pasó

**Otros estados posibles:**
```
Up 5 seconds              ← Acaba de iniciar, sin health check aún
Up 1 minute (unhealthy)   ← Corriendo pero health check falla
Exited (0)                ← Terminó correctamente
Exited (1)                ← Terminó con error
Restarting                ← Reiniciando automáticamente
Paused                    ← Pausado
```

---

### 6. PORTS (Puertos Mapeados)

```
0.0.0.0:8080->8080/tcp
0.0.0.0:1433->1433/tcp
```

**Formato:** `{HOST_IP}:{HOST_PORT}->{CONTAINER_PORT}/{PROTOCOL}`

**Ejemplo: `0.0.0.0:8080->8080/tcp`**
- `0.0.0.0` = Accesible desde cualquier IP
- `8080` = Puerto en tu máquina (HOST)
- `8080` = Puerto dentro del contenedor (CONTAINER)
- `tcp` = Protocolo

**Significado práctico:**
```
localhost:8080  →  (viaja a través de red)  →  Contenedor puerto 8080
```

---

## 🧪 EJEMPLOS DE SALIDA

### ✅ Estado Perfecto

```bash
$ docker-compose ps

NAME                   IMAGE                                    STATUS              PORTS
reinge-app             reinge/inventario:latest                 Up 2 minutes (healthy)   0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest  Up 3 minutes (healthy)   0.0.0.0:1433->1433/tcp
```

**Indica:**
- ✅ Ambos servicios corriendo
- ✅ Ambos health checks pasando
- ✅ Puertos accesibles
- ✅ Sistema operativo

---

### ⚠️ Iniciando (Health check no pasó aún)

```bash
$ docker-compose ps

NAME                   IMAGE                                    STATUS              PORTS
reinge-app             reinge/inventario:latest                 Up 5 seconds        0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest  Up 10 seconds       0.0.0.0:1433->1433/tcp
```

**Indica:**
- ⏳ Acaba de iniciar
- ⏳ Health checks aún no han corrido
- 💡 Esperar 20-40 segundos

---

### 🔴 Unhealthy (Health check falla)

```bash
$ docker-compose ps

NAME                   IMAGE                                    STATUS                PORTS
reinge-app             reinge/inventario:latest                 Up 2 minutes (unhealthy)  0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest  Up 3 minutes (healthy)    0.0.0.0:1433->1433/tcp
```

**Indica:**
- 🔴 App no responde a health check
- 💡 Ver logs: `docker-compose logs app`
- 💡 Reiniciar: `docker-compose restart app`

---

### 🛑 Exited (Terminado)

```bash
$ docker-compose ps

NAME                   IMAGE                                    STATUS              PORTS
reinge-app             reinge/inventario:latest                 Exited (0)
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest  Exited (1)
```

**Indica:**
- 🛑 Servicios no están corriendo
- `Exited (0)` = Terminó correctamente
- `Exited (1)` = Terminó con error

**Solución:**
```bash
docker-compose up
```

---

### 📭 Sin servicios (Compose no inició)

```bash
$ docker-compose ps

NAME    IMAGE    COMMAND    SERVICE    STATUS    PORTS
```

**Indica:**
- ❌ Compose no está corriendo
- ❌ Servicios no iniciados

**Solución:**
```bash
docker-compose up -d
```

---

## 🔧 COMANDOS RELACIONADOS

### Ver todo (incluso detenidos)

```bash
docker-compose ps -a

# Muestra TODOS los contenedores, incluso los que fueron parados
```

### Ver solo nombres

```bash
docker-compose ps --quiet

# Salida:
# abc123def456
# xyz789uvw123
```

### Ver en formato JSON

```bash
docker-compose ps --format json

# Útil para scripts/parsing
```

### Ver un servicio específico

```bash
docker-compose ps db

# Solo muestra la BD
```

---

## 📊 MATRIZ DE ESTADOS

| Estado | Significado | Acción |
|--------|-------------|--------|
| `Up X (healthy)` | ✅ Todo bien | Nada |
| `Up X (unhealthy)` | ⚠️ Problema | Ver logs, reiniciar |
| `Up X` (sin health) | ⏳ Iniciando | Esperar |
| `Exited (0)` | ✅ Parado normal | `docker-compose up` |
| `Exited (1)` | 🔴 Error | `docker-compose logs` |
| `Restarting` | 🔄 Reiniciando | Esperar o revisar logs |
| `Paused` | ⏸️ Pausado | `docker-compose unpause` |

---

## 🚀 CASOS DE USO COMUNES

### 1. Verificar que todo inició

```bash
docker-compose ps

# Buscar:
# - Ambos servicios "Up"
# - Ambos "(healthy)"
# - Puertos mapeados
```

### 2. Monitorear en tiempo real

```bash
watch -n 2 "docker-compose ps"

# Actualiza cada 2 segundos (Linux/Mac)
```

En Windows (PowerShell):
```powershell
while($true) { 
  docker-compose ps
  Start-Sleep -Seconds 2
  Clear-Host
}
```

### 3. Obtener ID de un contenedor

```bash
docker-compose ps --quiet db

# Salida: abc123def456
# Luego usar: docker logs abc123def456
```

### 4. Verificar puertos

```bash
docker-compose ps

# Buscar columna PORTS
# Verificar que están mapeados correctamente
```

---

## 🧪 VERIFICACIÓN DESPUÉS DE `docker-compose up --build`

### Paso 1: Ver estado

```bash
docker-compose ps
```

### Paso 2: Interpretar resultado

```
NAME                   STATUS                PORTS
reinge-app             Up X minutes (healthy)    8080->8080
reinge-sqlserver       Up X minutes (healthy)    1433->1433

✅ Si ves esto → TODO BIEN
```

### Paso 3: Si hay problema

```bash
# Ver logs
docker-compose logs -f

# Reiniciar
docker-compose restart

# Parar y reintentar
docker-compose down
docker-compose up --build
```

---

## 💡 TIPS

### Health check demora

```
Up 5 seconds  ← Aún sin (healthy)

Esperar 20-40 segundos según servicio
```

### Puerto en uso

```
Si ves puerto vacío en PORTS, algo está mal

docker-compose logs  ← Ver por qué falla
```

### Necesitas IP exacta

```bash
docker inspect $(docker-compose ps -q app) | grep IPAddress

# Muestra IP interna del contenedor
```

### Script para verificar todo

```bash
#!/bin/bash

echo "=== Docker Compose Status ==="
docker-compose ps

echo ""
echo "=== Health Check Details ==="
docker-compose exec db sqlcmd -S localhost -U sa -Q "SELECT @@VERSION" 2>/dev/null && echo "DB: ✅" || echo "DB: ❌"
curl -s http://localhost:8080/health > /dev/null && echo "App: ✅" || echo "App: ❌"
```

---

## 🎯 RESUMEN RÁPIDO

| Necesitas... | Comando |
|-------------|---------|
| Ver estado actual | `docker-compose ps` |
| Ver todo (incluso parados) | `docker-compose ps -a` |
| Ver logs en vivo | `docker-compose logs -f` |
| Monitorear cambios | `watch "docker-compose ps"` |
| Ver un servicio | `docker-compose ps db` |
| Ver IPs internas | `docker inspect ...` |
| Verificar puertos | `docker-compose ps` (ver PORTS) |

---

## 🆘 TROUBLESHOOTING

### Problema: "No such file or directory"

```bash
# Asegúrate de estar en el directorio correcto
cd /path/to/Reinge-SistemaInventarioLegacy
docker-compose ps
```

### Problema: "Cannot connect to Docker daemon"

```bash
# Docker no está corriendo
# Windows/Mac: Abre Docker Desktop
# Linux: sudo systemctl start docker
```

### Problema: "No containers to show"

```bash
# Los servicios no fueron iniciados
docker-compose up -d
docker-compose ps
```

---

**Generado:** Guía de `docker-compose ps`  
**Uso:** Ver estado de servicios  
**Frecuencia:** Después de cada `up`, para verificar  

