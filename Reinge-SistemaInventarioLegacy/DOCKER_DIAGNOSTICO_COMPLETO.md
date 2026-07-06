# ✅/❌ DIAGNÓSTICO: ¿docker compose up --build funcionó?

## 🎯 ÁRBOL DE DECISIÓN

```
¿Viste error en la salida?
│
├─ NO → Sección: TODO FUNCIONÓ ✅
│
└─ SÍ → ¿Qué tipo de error?
    │
    ├─ "Port X is already in use" → Ir a: ERROR #1
    ├─ "docker: command not found" → Ir a: ERROR #2
    ├─ "Cannot connect to Docker daemon" → Ir a: ERROR #3
    ├─ ".env file not found" → Ir a: ERROR #4
    ├─ "Service healthcheck failed" → Ir a: ERROR #5
    ├─ "Build failed" → Ir a: ERROR #6
    ├─ "Connection timeout" → Ir a: ERROR #7
    ├─ "Disk space full" → Ir a: ERROR #8
    ├─ "Authentication failed" → Ir a: ERROR #9
    ├─ "SQL Server won't start" → Ir a: ERROR #10
    └─ Otro error → Ir a: DIAGNÓSTICO GENERAL
```

---

# ✅ TODO FUNCIONÓ CORRECTAMENTE

Si ejecutaste `docker compose up --build` y **no viste errores**, verifica esto:

## 🔍 VERIFICACIÓN 1: Ver Estado

```bash
docker-compose ps
```

**Deberías ver:**
```
NAME                   IMAGE                              STATUS              PORTS
reinge-app             reinge/inventario:latest           Up X minutes (healthy)   0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:... Up X minutes (healthy)  0.0.0.0:1433->1433/tcp
```

✅ **Si ves esto:** TODO PERFECTO

❌ **Si ves algo diferente:** Ir a sección ERROR

---

## 🔍 VERIFICACIÓN 2: Probar BD

```bash
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT @@VERSION"
```

**Debería mostrar:**
```
Microsoft SQL Server 2022 (RTM)
```

✅ **Si funciona:** BD lista

❌ **Si falla:** Ir a ERROR #5 (health check)

---

## 🔍 VERIFICACIÓN 3: Probar Aplicación

```bash
curl http://localhost:8080/health
```

**O en el navegador:**
```
http://localhost:8080
```

✅ **Si responde:** Aplicación lista

❌ **Si no responde:** Ir a ERROR #5

---

## 🔍 VERIFICACIÓN 4: Ver Logs

```bash
docker-compose logs --tail=50
```

**Deberías ver:**
```
reinge-sqlserver | SQL Server is now ready to accept connections
reinge-app       | Application started successfully
```

✅ **Si ves esto:** Logs limpios, sin errores

❌ **Si ves errores:** Documenta y continúa a ERROR específico

---

## ✅ CHECKLIST: TODO FUNCIONÓ

- [ ] `docker-compose ps` muestra 2 servicios "healthy"
- [ ] `sqlcmd` devuelve versión de SQL Server
- [ ] `curl` recibe respuesta de API
- [ ] `docker-compose logs` sin errores
- [ ] Puertos 1433 y 8080 accesibles

✅ **Si todo está marcado:** ¡SISTEMA OPERATIVO!

---

# ❌ HUBO ERRORES

Si viste errores, identifica cuál usando el árbol arriba y sigue la solución:

---

## 🔴 ERROR #1: "Port X is already in use"

### Síntoma

```
Error response from daemon: bind: address already in use
ERROR: for reinge-sqlserver  Cannot start service db: driver failed programming external connectivity on endpoint reinge-sqlserver
```

### Causa

Otro servicio está usando el puerto (1433 o 8080)

### Solución A: Ver qué usa el puerto

```bash
# Linux/Mac
lsof -i :1433

# Windows PowerShell
netstat -ano | findstr :1433
```

**Salida típica:**
```
COMMAND    PID    USER     FD   TYPE DEVICE SIZE/OFF NODE NAME
sqlserver  1234   user     5u   IPv6 ...    ...  *:1433
```

### Solución B: Matar el proceso

```bash
# Linux/Mac
kill -9 1234

# Windows
taskkill /PID 1234 /F
```

### Solución C: Cambiar puerto en .env

```env
DB_PORT=5433  # ← Cambiar a puerto libre
```

```bash
docker-compose up --build
```

### Solución D: Detener otro SQL Server

```bash
# Si tienes SQL Server instalado localmente
# Windows: Services.msc → SQL Server → Stop
# Mac: Activity Monitor → sqlservr → Force Quit
# Linux: sudo systemctl stop mssql-server
```

### Verificación

```bash
docker-compose ps

# STATUS debe ser: Up X minutes (healthy)
```

---

## 🔴 ERROR #2: "docker: command not found"

### Síntoma

```
bash: docker: command not found
docker-compose: command not found
```

### Causa

Docker no está instalado o no está en PATH

### Solución A: Instalar Docker

**Windows/Mac:**
- Descargar Docker Desktop: https://www.docker.com/products/docker-desktop
- Instalar
- Abrir Docker Desktop

**Linux (Ubuntu/Debian):**
```bash
sudo apt-get update
sudo apt-get install docker.io docker-compose

# Verificar instalación
docker --version
docker-compose --version
```

### Solución B: Agregar a PATH

**Windows:**
- Buscar "Environment Variables"
- Editar PATH
- Agregar: `C:\Program Files\Docker\Docker\resources\bin`
- Reiniciar terminal

**Mac/Linux:**
```bash
# Buscar dónde está docker
which docker

# Debería mostrar algo como: /usr/bin/docker
```

### Verificación

```bash
docker --version
# Docker version 24.0.0+

docker-compose --version
# Docker Compose version 2.20.0+

docker ps
# Debería listar contenedores (puede estar vacío)
```

---

## 🔴 ERROR #3: "Cannot connect to Docker daemon"

### Síntoma

```
Cannot connect to Docker daemon at unix:///var/run/docker.sock
error during connect: this error may indicate the docker daemon is not running
```

### Causa

Docker daemon (servicio) no está corriendo

### Solución A: Iniciar Docker

**Windows/Mac:**
```bash
# Abrir Docker Desktop (la aplicación)
# El daemon se inicia automáticamente
```

**Linux:**
```bash
sudo systemctl start docker

# Verificar estado
sudo systemctl status docker

# Hacer que inicie automáticamente
sudo systemctl enable docker
```

### Solución B: Permisos en Linux

```bash
# Agregar usuario al grupo docker
sudo usermod -aG docker $USER

# Aplicar cambios
newgrp docker

# Verificar
docker ps
# No debería pedir sudo
```

### Verificación

```bash
docker ps

# Debería mostrar lista de contenedores (puede estar vacía)
# NO debería mostrar error de conexión
```

---

## 🔴 ERROR #4: ".env file not found"

### Síntoma

```
ERROR: The Compose file '/path/docker-compose.yml' is invalid
missing required env var: DB_SA_PASSWORD
```

### Causa

Archivo `.env` no existe o no se puede encontrar

### Solución

```bash
# 1. Verificar que exista
ls -la .env

# 2. Si no existe, crear desde template
cp .env.produccion.example .env

# 3. Editar con contraseña
nano .env

# Debe tener al menos:
COMPOSE_PROJECT_NAME=reinge
DB_SA_PASSWORD=P@ssw0rd2024!Test

# 4. Guardar y intentar de nuevo
docker-compose up --build
```

### Verificación

```bash
cat .env | grep DB_SA_PASSWORD

# Debería mostrar: DB_SA_PASSWORD=...
```

---

## 🔴 ERROR #5: "Service healthcheck failed" o "unhealthy"

### Síntoma

```bash
$ docker-compose ps
NAME                   STATUS              
reinge-sqlserver       Up X minutes (unhealthy)
reinge-app             Up X minutes (unhealthy)
```

O en logs:
```
health check failed: unable to connect
```

### Causa

Servicio está corriendo pero health checks fallan (BD no lista, App no responde)

### Solución A: Esperar (SQL Server tarda ~40 segundos)

```bash
# Esperar más tiempo
sleep 60

# Reintentar
docker-compose restart

# Verificar
docker-compose ps
```

### Solución B: Ver logs de error

```bash
# Logs de BD
docker-compose logs db --tail=50

# Logs de App
docker-compose logs app --tail=50

# Buscar errores específicos
docker-compose logs | grep -i error
```

### Solución C: Reiniciar servicios

```bash
# Reiniciar todo
docker-compose restart

# O específico
docker-compose restart db
docker-compose restart app
```

### Solución D: Reset completo

```bash
# Parar todo
docker-compose down

# Esperar 5 segundos
sleep 5

# Reiniciar
docker-compose up --build

# Esperar 60 segundos
sleep 60

# Verificar
docker-compose ps
```

### Verificación

```bash
# Health checks pasando
docker-compose ps | grep "healthy"

# O probar manualmente
docker-compose exec db sqlcmd -S localhost -U sa -Q "SELECT 1"

curl http://localhost:8080/health
```

---

## 🔴 ERROR #6: "Build failed"

### Síntoma

```
ERROR: failed to download resource
ERROR: Service 'app' failed to build
Step 5/20 : RUN dotnet restore ... exited with code 1
```

### Causa

Problema durante compilación (falta NuGet, error en código, etc.)

### Solución A: Build sin cache

```bash
docker-compose build --no-cache
```

### Solución B: Ver logs detallados

```bash
docker-compose build --no-cache 2>&1 | tail -100

# Buscar error específico
docker-compose build 2>&1 | grep -i "error"
```

### Solución C: Verificar .csproj

```bash
# Ver si hay errores en proyecto
cat Reinge-SistemaInventarioLegacy/Reinge-SistemaInventarioLegacy.csproj | head -20

# Compilar localmente (si tienes .NET)
cd Reinge-SistemaInventarioLegacy
dotnet build -c Release

# Si falla localmente, fijar error local primero
```

### Solución D: Limpiar y reintentar

```bash
# Eliminar imágenes anteriores
docker image rm reinge/inventario:latest

# Intentar build de nuevo
docker-compose build --no-cache

# Luego up
docker-compose up -d
```

### Verificación

```bash
# Build completó sin errores
docker-compose build

# Imagen creada
docker images | grep reinge

# Servicios corriendo
docker-compose ps
```

---

## 🔴 ERROR #7: "Connection timeout"

### Síntoma

```
timeout: timed out waiting for SQL Server to start
Connection refused
Unable to connect to server
```

### Causa

BD tarda demasiado en iniciar o puerto incorrecto

### Solución A: Aumentar timeout

```bash
# Esperar 2 minutos
sleep 120

# Verificar
docker-compose ps
```

### Solución B: Verificar puerto

```bash
# En .env, verificar
cat .env | grep DB_PORT

# Debería ser 1433 o similar

# Probar conexión
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT 1"
```

### Solución C: Ver logs de BD

```bash
docker-compose logs db --tail=100

# Buscar mensaje "ready to accept connections"
docker-compose logs db | grep ready
```

### Solución D: Reiniciar BD

```bash
docker-compose restart db

# Esperar
sleep 40

# Probar
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT 1"
```

### Verificación

```bash
# Health check healthy
docker-compose ps | grep db | grep healthy

# Conexión exitosa
docker-compose exec db sqlcmd -S localhost -U sa -Q "SELECT @@VERSION"
```

---

## 🔴 ERROR #8: "Disk space full"

### Síntoma

```
no space left on device
Error response from daemon: write /var/lib/docker/...
```

### Causa

Disco lleno con imágenes/volúmenes/contenedores anteriores

### Solución A: Limpiar Docker

```bash
# Ver uso de espacio
docker system df

# Limpiar no usados
docker system prune -a --volumes

# ⚠️ Esto elimina TODO lo que no uses
```

### Solución B: Limpiar seleccionando

```bash
# Solo imágenes
docker image prune -a

# Solo volúmenes
docker volume prune

# Solo contenedores
docker container prune
```

### Solución C: Eliminar proyecto anterior

```bash
# Si tienes proyecto viejo
docker-compose down -v --rmi all

# Libera espacio
```

### Verificación

```bash
# Espacio disponible
df -h

# Espacio Docker
docker system df

# Reintentar build
docker-compose up --build
```

---

## 🔴 ERROR #9: "Authentication failed" / "Access denied"

### Síntoma

```
Access denied: sa password incorrect
Login failed for user 'sa'
Authentication failed
```

### Causa

Contraseña incorrecta en .env o en BD

### Solución A: Verificar .env

```bash
# Ver contraseña (sin mostrar completa)
cat .env | grep DB_SA_PASSWORD

# Debe coincidir con la BD
```

### Solución B: Reset de contraseña

```bash
# Eliminar volumen (⚠️ Pierde datos)
docker-compose down -v

# Editar .env con nueva contraseña
nano .env
# DB_SA_PASSWORD=NuevaContraseña123!

# Reiniciar
docker-compose up --build
```

### Solución C: Verificar credenciales en BD

```bash
# Conectar como sa
docker-compose exec db sqlcmd -S localhost -U sa -P "" -Q "SELECT 1"

# Si pide contraseña, está mal configurado
```

### Verificación

```bash
# Conexión exitosa
docker-compose exec db sqlcmd -S localhost -U sa -Q "SELECT @@VERSION"

# Sin errores de autenticación
```

---

## 🔴 ERROR #10: "SQL Server won't start"

### Síntoma

```
/opt/mssql/bin/sqlservr: error while loading shared libraries
Cannot start SQL Server
```

### Causa

Problema con imagen SQL Server o recursos insuficientes

### Solución A: Aumentar recursos

En `docker-compose-produccion.yml`:
```yaml
deploy:
  resources:
    limits:
      memory: 4G  # Aumentar si es posible
    reservations:
      memory: 2G
```

### Solución B: Usar otra imagen

```yaml
db:
  image: mcr.microsoft.com/mssql/server:2019-latest  # Versión anterior
```

### Solución C: Reset completo

```bash
# Eliminar todo
docker-compose down -v --rmi all

# Reiniciar
docker-compose build
docker-compose up --build
```

### Verificación

```bash
# BD responde
docker-compose exec db sqlcmd -S localhost -U sa -Q "SELECT @@VERSION"

# Logs sin errores
docker-compose logs db | grep "ready"
```

---

# 🔍 DIAGNÓSTICO GENERAL

Si tu error no está arriba, sigue estos pasos:

## Paso 1: Recolectar información

```bash
# Versiones
docker --version
docker-compose --version

# Estado
docker-compose ps

# Logs completos
docker-compose logs > /tmp/compose-logs.txt

# Config (sanitizado)
docker-compose config | grep -v PASSWORD
```

## Paso 2: Identificar error

```bash
# Último error
docker-compose logs -f | head -100

# Buscar palabra "error"
docker-compose logs 2>&1 | grep -i "error\|failed\|exception"
```

## Paso 3: Buscar solución

Toma el error exacto y busca en:
- GitHub Issues: https://github.com/docker/compose/issues
- Stack Overflow: https://stackoverflow.com/questions/tagged/docker-compose
- Docker Docs: https://docs.docker.com/

## Paso 4: Si nada funciona

Reset nuclear:
```bash
# Parar todo
docker-compose down -v

# Limpiar todo
docker system prune -a --volumes

# Empezar de cero
docker-compose build
docker-compose up --build
```

---

# 📊 MATRIZ DE SOLUCIONES RÁPIDAS

| Error | Solución Rápida |
|-------|-----------------|
| Port in use | `docker-compose down` + cambiar puerto en .env |
| docker not found | Instalar Docker Desktop |
| Cannot connect daemon | Iniciar Docker (abrir Docker Desktop) |
| .env not found | `cp .env.produccion.example .env` |
| Health check failed | `sleep 60 && docker-compose restart` |
| Build failed | `docker-compose build --no-cache` |
| Connection timeout | `sleep 120 && docker-compose ps` |
| Disk full | `docker system prune -a --volumes` |
| Auth failed | Reset BD con `docker-compose down -v` |
| SQL Server won't start | `docker-compose down -v --rmi all` |

---

# ✅ SI TODO FUNCIONÓ

```bash
# Ver estado
docker-compose ps

# Deberías ver:
✓ reinge-app       Up X minutes (healthy)
✓ reinge-sqlserver Up X minutes (healthy)

# Acceder a BD
sqlcmd -S localhost,1433 -U sa -P "contraseña" -Q "SELECT @@VERSION"

# Acceder a API
curl http://localhost:8080

# Listo para usar! 🎉
```

---

# 📞 RESUMEN

**✅ Todo funcionó?**
→ Ir a: VERIFICACIÓN 1-4

**❌ Tuvo errores?**
→ Identificar cuál (ERROR #1-#10)
→ Seguir solución
→ Volver a verificar

**¿Error no está arriba?**
→ Ir a: DIAGNÓSTICO GENERAL

---

**Generado:** Guía de diagnóstico Docker Compose  
**Uso:** Después de ejecutar `docker-compose up --build`  
**Status:** Soluciona 10+ errores comunes  

