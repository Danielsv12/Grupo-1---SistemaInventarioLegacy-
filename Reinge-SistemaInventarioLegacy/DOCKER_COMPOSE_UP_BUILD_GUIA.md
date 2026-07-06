# 🚀 DOCKER COMPOSE UP --BUILD - GUÍA PASO A PASO

## 📋 Antes de Ejecutar

### ✅ Prerequisitos

```bash
# 1. Verificar Docker instalado
docker --version
# Debería mostrar: Docker version 24.0.0+

# 2. Verificar Docker Compose
docker-compose --version
# Debería mostrar: Docker Compose version 2.20.0+

# 3. Verificar que Docker daemon está corriendo
docker ps
# Debería mostrar lista de contenedores (vacía si es primera vez)
```

### 📁 Estructura de Archivos

```
Reinge-SistemaInventarioLegacy/
├── Dockerfile.optimizado       ← Script de build
├── docker-compose-produccion.yml  ← Compose (RECOMENDADO)
├── docker-compose.yml          ← Compose alternativo
├── .env.produccion.example     ← Template
├── .env                        ← DEBE EXISTIR (credenciales)
├── .dockerignore               ← Reduce contexto
├── setup_database.sql          ← Script de BD
├── Reinge-SistemaInventarioLegacy.csproj
└── [código fuente]
```

---

## 🔐 PASO 1: Preparar .env

### Opción A: Seguro (RECOMENDADO)

```bash
# 1. Copiar template
cp .env.produccion.example .env

# 2. Generar contraseña fuerte
openssl rand -base64 20 | tr -d "=+/" | cut -c1-20
# Salida: K8mP2xW5jL9nQ4tR7vS1

# 3. Editar .env
nano .env

# Contenido mínimo:
COMPOSE_PROJECT_NAME=reinge
ENVIRONMENT=Production
DB_SA_PASSWORD=K8mP2xW5jL9nQ4tR7vS1   # ← Pegar contraseña
DB_HOSTNAME=db
DB_PORT=1433
DB_NAME=Inventario
APP_PORT=8080
```

### Opción B: Rápido (Para desarrollo local)

```bash
cat > .env << 'EOF'
COMPOSE_PROJECT_NAME=reinge
ENVIRONMENT=Development
DB_SA_PASSWORD=P@ssw0rd2024!Test
DB_HOSTNAME=db
DB_PORT=1433
DB_NAME=Inventario
APP_PORT=8080
EOF
```

### Verificar .env

```bash
# Verificar que existe
ls -la .env

# Verificar contenido (no mostrar credencial completa)
cat .env | head -10
```

---

## 🏗️ PASO 2: Preparar Directorios de Volúmenes

```bash
# Crear directorios para persistencia de datos
mkdir -p .data/sqlserver
mkdir -p .data/sqlserver/logs
mkdir -p .data/app/logs
mkdir -p .backups

# Dar permisos
chmod -R 755 .data

# Verificar
ls -la .data/
```

---

## 🚀 PASO 3: Ejecutar docker compose up --build

### Opción A: Usar docker-compose-produccion.yml (RECOMENDADO)

```bash
# Comando completo
docker-compose \
  -f docker-compose-produccion.yml \
  -p reinge-inventario \
  up \
  --build
```

### Opción B: Uso corto (si renombraste a docker-compose.yml)

```bash
docker-compose up --build
```

### Opción C: En background

```bash
# Iniciar en background (-d)
docker-compose -f docker-compose-produccion.yml up --build -d

# Ver logs
docker-compose -f docker-compose-produccion.yml logs -f
```

---

## 📊 QUÉ ESPERAR DURANTE EL BUILD

### Fase 1: Building Images (2-5 minutos)

```
[+] Building 2.3s (20/20) FINISHED

docker-entrypoint.sh: step 1/20 FROM mcr.microsoft.com/dotnet/sdk:8.0
...
Step 5/20 : COPY . .
...
Step 20/20 : ENTRYPOINT ["dotnet", "Reinge-SistemaInventarioLegacy.dll"]

Successfully tagged reinge/inventario:latest
```

**Qué está pasando:**
- 📦 Descargando imagen base (.NET SDK)
- 📁 Copiar archivos del proyecto
- 🔨 Compilar código
- 📦 Publicar binarios
- 🐳 Crear imagen final

### Fase 2: Creating Services (10-40 segundos)

```
[+] Running 2/2
 ✓ Network reinge-network Created
 ✓ Volume "reinge-sqlserver-data" Created
 ✓ Container reinge-sqlserver Created
 ✓ Container reinge-app Created
```

**Qué está pasando:**
- 🌐 Crear red interna
- 💾 Crear volúmenes
- 🐳 Crear contenedores

### Fase 3: Starting Services (40-60 segundos)

```
reinge-sqlserver  | SQL Server 2022 CTP3.2 on Linux
reinge-sqlserver  | (c) Microsoft Corporation. All rights reserved.
reinge-sqlserver  | Starting SQL Server Agent
reinge-sqlserver  | SQL Server is now ready to accept connections
reinge-app        | Application started successfully
```

**Qué está pasando:**
- 🗄️ SQL Server inicializando BD
- ✅ Health checks pasando
- 🚀 Aplicación arrancando

---

## ✅ PASO 4: Verificar que Todo Funciona

### Verificación 1: Ver estado de contenedores

```bash
docker-compose ps

# Esperado:
NAME                 STATUS              PORTS
reinge-sqlserver     Up 2 minutes (healthy)
reinge-app           Up 1 minute (healthy)
```

### Verificación 2: Ver logs

```bash
# Logs de todo
docker-compose logs

# Logs en vivo (útil para debugging)
docker-compose logs -f

# Logs específicos
docker-compose logs db      # BD
docker-compose logs app     # Aplicación

# Últimas 50 líneas
docker-compose logs --tail=50
```

### Verificación 3: Probar BD

```bash
# Acceder a BD desde host
sqlcmd -S localhost,1433 -U sa -P "P@ssw0rd2024!Test" -Q "SELECT @@VERSION"

# O desde contenedor
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT DB_NAME()"
```

### Verificación 4: Probar aplicación

```bash
# Si es API
curl http://localhost:8080/health

# Ver respuesta
# Esperado: {"status":"healthy"}
```

### Verificación 5: Ver volúmenes persistentes

```bash
# Verificar que datos existen
ls -la .data/sqlserver/
ls -la .data/app/logs/

# Debería contener archivos de BD y logs
```

---

## 🔧 DURANTE LA EJECUCIÓN: MONITOREO

### Terminal 1: Ver logs en vivo

```bash
docker-compose logs -f
```

### Terminal 2: Ver estado

```bash
# Cada 2 segundos
watch -n 2 "docker-compose ps"

# O manual
docker-compose ps
```

### Terminal 3: Ejecutar comandos

```bash
# Probar BD
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT 1"

# Ejecutar test
docker-compose exec app dotnet test
```

---

## 🚨 PROBLEMAS COMUNES Y SOLUCIONES

### ❌ Error: "Port 1433 is already in use"

```
Error response from daemon: bind: address already in use
```

**Solución A: Cambiar puerto**
```env
# En .env
DB_PORT=5433  # ← Cambiar a otro puerto libre
```

```bash
# Reintentar
docker-compose up --build
```

**Solución B: Detener otro SQL Server**
```bash
# Ver qué está usando el puerto
lsof -i :1433  # (Linux/Mac)
netstat -ano | findstr :1433  # (Windows)

# Matar proceso
kill -9 <PID>
```

**Solución C: Usar puerto interno solamente**
```yaml
# En docker-compose.yml, comentar ports
# ports:
#   - "1433:1433"
```

---

### ❌ Error: "docker: command not found"

```
bash: docker: command not found
```

**Solución:**
- Windows: Instalar Docker Desktop
- Mac: `brew install docker` o Docker Desktop
- Linux: `sudo apt-get install docker.io docker-compose`

---

### ❌ Error: "Cannot connect to Docker daemon"

```
Cannot connect to Docker daemon at unix:///var/run/docker.sock
```

**Solución:**
```bash
# Iniciar Docker daemon
# Windows/Mac: Abrir Docker Desktop

# Linux:
sudo systemctl start docker

# Dar permisos
sudo usermod -aG docker $USER
newgrp docker
```

---

### ❌ Error: ".env file not found"

```
ERROR: The Compose file '/path/docker-compose.yml' is invalid
missing required env var: DB_SA_PASSWORD
```

**Solución:**
```bash
# Verificar .env existe
ls -la .env

# Si no existe, crear
cp .env.produccion.example .env
```

---

### ❌ Error: "Service healthcheck failed"

```
reinge-sqlserver  | Error: unable to connect to master instance
```

**Solución:**
```bash
# Esperar más (SQL Server tarda ~40 segundos)
sleep 60

# Reiniciar
docker-compose restart

# Ver logs de BD
docker-compose logs db --tail=50
```

---

### ❌ Error: "Build failed: connection timeout"

```
failed to download
```

**Solución A: Mejorar conexión**
- Verificar internet
- Cambiar DNS a 8.8.8.8

**Solución B: Reintentar**
```bash
docker-compose build --no-cache
```

**Solución C: Usar proxy**
```bash
docker build --network host --build-arg http_proxy=http://proxy:8080
```

---

### ❌ Error: "Disk space full"

```
no space left on device
```

**Solución:**
```bash
# Limpiar imágenes no usadas
docker image prune -a

# Limpiar volúmenes
docker volume prune

# Limpiar todo
docker system prune -a --volumes
```

---

## 🛑 DETENER SERVICIOS

### Opción A: Pause (suspender, no eliminar)

```bash
docker-compose pause
```

### Opción B: Stop (detener, preservar datos)

```bash
docker-compose stop
```

### Opción C: Down (detener y eliminar contenedores, pero no volúmenes)

```bash
docker-compose down
```

### Opción D: Down con eliminación de volúmenes (⚠️ Elimina datos)

```bash
docker-compose down -v
```

### Opción E: Kill (terminar inmediatamente)

```bash
docker-compose kill
```

---

## 🔄 REINICIAR

```bash
# Reiniciar servicio específico
docker-compose restart db

# Reiniciar todo
docker-compose restart

# Reiniciar y reconstruir
docker-compose up --build --force-recreate
```

---

## 📊 COMANDOS ÚTILES DURANTE EJECUCIÓN

```bash
# Ver logs en tiempo real
docker-compose logs -f

# Ver logs de un servicio
docker-compose logs -f db

# Últimas 100 líneas
docker-compose logs --tail=100

# Ver estado
docker-compose ps

# Ejecutar comando en contenedor
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT @@VERSION"

# Entrar en bash
docker-compose exec app bash

# Ver recursos usados
docker stats

# Ver configuración renderizada
docker-compose config

# Validar configuración
docker-compose config --quiet
```

---

## ✅ CHECKLIST DE VERIFICACIÓN

Después de `docker compose up --build`, verificar:

- [ ] `docker-compose ps` muestra 2 servicios en "Up"
- [ ] Ambos servicios tienen status "healthy"
- [ ] `docker-compose logs` sin errores
- [ ] Puerto 1433 mapeado (verificar: `netstat -tulpn | grep 1433`)
- [ ] Puerto 8080 mapeado (verificar: `curl http://localhost:8080`)
- [ ] `.data/` directorio contiene archivos
- [ ] `docker-compose logs db | grep ready` tiene resultado
- [ ] `docker-compose exec app dotnet --info` funciona
- [ ] `docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT 1"` funciona

---

## 📋 CICLO COMPLETO

```bash
# 1. Preparar
cp .env.produccion.example .env
nano .env  # Editar credenciales

# 2. Build e iniciar
docker-compose -f docker-compose-produccion.yml up --build

# 3. En otra terminal, monitorear
docker-compose logs -f

# 4. Verificar
docker-compose ps
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT @@VERSION"

# 5. Usar
# - Acceder a BD: localhost:1433
# - Acceder a App: localhost:8080

# 6. Detener
docker-compose down
```

---

## 🎯 PRÓXIMOS PASOS

### Si todo funciona:

1. ✅ Documentar comandos que usas frecuentemente
2. ✅ Crear script bash/bat para startup
3. ✅ Agregar CI/CD pipeline
4. ✅ Implementar monitoreo

### Si hay errores:

1. 📖 Revisar troubleshooting arriba
2. 💬 Revisar logs: `docker-compose logs -f`
3. 🔧 Reintentar: `docker-compose up --build`
4. 🗑️ Reset total: `docker-compose down -v && docker-compose up --build`

---

## 📞 AYUDA RÁPIDA

```bash
# Si algo falla, ejecuta esto:
docker-compose down -v
docker-compose up --build --no-cache

# Ver logs completos
docker-compose logs | tail -100

# Diagnosticar
docker-compose config
docker-compose ps -a
docker images
docker volume ls
```

---

**Tiempo estimado:** 5-10 minutos (si todo está bien)  
**Complejidad:** ⭐ (Fácil)  
**Riesgo:** 🟢 (Bajo)  

¿Necesitas que ejecute paso a paso contigo?

