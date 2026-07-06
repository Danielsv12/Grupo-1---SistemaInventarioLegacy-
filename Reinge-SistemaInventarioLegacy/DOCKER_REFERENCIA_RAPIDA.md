# ⚡ DOCKER COMPOSE - HOJA DE REFERENCIA RÁPIDA

## 🚀 COMANDO PRINCIPAL

```bash
# ← ESTO ES LO QUE EJECUTASTE
docker-compose up --build
```

| Flag | Significado |
|------|-------------|
| `up` | Iniciar servicios |
| `--build` | Reconstruir imágenes antes de iniciar |

---

## 📋 PREPARACIÓN (1 minuto)

```bash
# 1. Crear .env
cp .env.produccion.example .env

# 2. Editar credenciales (si quieres cambiarlas)
nano .env

# 3. Crear directorios de volúmenes
mkdir -p .data/{sqlserver,app/logs}
```

---

## 🚀 EJECUCIÓN (5-10 minutos)

### Opción A: Versión Completa (RECOMENDADA)

```bash
docker-compose -f docker-compose-produccion.yml up --build
```

### Opción B: Versión Corta (si usas docker-compose.yml)

```bash
docker-compose up --build
```

### Opción C: En Background

```bash
docker-compose up --build -d
docker-compose logs -f  # Ver logs en otra terminal
```

---

## ✅ VERIFICACIÓN (1 minuto)

```bash
# Estado de servicios
docker-compose ps

# Debería mostrar:
# ✓ reinge-sqlserver  Up X minutes (healthy)
# ✓ reinge-app        Up X minutes (healthy)

# Si alguno NO está healthy, esperar 30 segundos y reintentar
```

---

## 🔍 QUÉ VAS A VER

### Build (2-5 minutos)

```
[+] Building 2.3s (20/20) FINISHED
Successfully tagged reinge/inventario:latest
```

### Starting (20-40 segundos)

```
reinge-sqlserver  | SQL Server is now ready to accept connections
reinge-app        | Application started successfully
```

### Completo ✅

```
CONTAINER ID        STATUS
reinge-sqlserver    Up 2 minutes (healthy)
reinge-app          Up 1 minute (healthy)
```

---

## 🧪 COMANDOS DURANTE EJECUCIÓN

| Comando | Qué Hace |
|---------|----------|
| `Ctrl+C` | Detener (pausar servicios) |
| `docker-compose logs -f` | Ver logs en vivo |
| `docker-compose ps` | Ver estado |
| `docker-compose exec db sqlcmd -S localhost -U sa -Q "SELECT 1"` | Probar BD |
| `curl http://localhost:8080/health` | Probar API |

---

## 🛑 DETENER

```bash
# Opción 1: Pause (suspender)
docker-compose pause

# Opción 2: Stop (detener)
docker-compose stop

# Opción 3: Down (detener y eliminar contenedores)
docker-compose down

# Opción 4: Down + eliminar datos (⚠️ Cuidado)
docker-compose down -v
```

---

## 🔄 REINICIAR

```bash
# Reiniciar todo
docker-compose restart

# Reiniciar un servicio
docker-compose restart db

# Reconstruir y reiniciar
docker-compose up --build --force-recreate
```

---

## 🚨 PROBLEMAS COMUNES

### Puerto en uso

```bash
# Cambiar en .env
DB_PORT=5433

# Reintentar
docker-compose up --build
```

### Docker daemon no corre

```bash
# Windows/Mac: Abrir Docker Desktop
# Linux:
sudo systemctl start docker
```

### Conexión timeout a BD

```bash
# Esperar más (SQL Server tarda ~40s)
sleep 60
docker-compose restart
```

### Disco lleno

```bash
docker system prune -a --volumes
```

---

## 📊 COMANDOS ÚTILES

```bash
# Ver todo
docker-compose ps -a

# Ver logs
docker-compose logs --tail=50

# Acceder a BD
docker-compose exec db sqlcmd -S localhost -U sa -Q "SELECT @@VERSION"

# Acceder a app
docker-compose exec app bash

# Ver recursos
docker stats

# Validar config
docker-compose config
```

---

## 📈 PROCESO TÍPICO

```
1. Preparar (.env)
   ↓
2. docker-compose up --build
   ↓
3. Esperar 40 segundos
   ↓
4. Verificar: docker-compose ps
   ↓
5. Usar aplicación
   ↓
6. docker-compose down (cuando terminares)
```

---

## 🎯 RESUMEN

```
✅ Antes: cp .env.produccion.example .env
✅ Comando: docker-compose up --build
✅ Esperar: 5-10 minutos
✅ Verificar: docker-compose ps
✅ Usar: localhost:1433 (BD), localhost:8080 (App)
✅ Parar: docker-compose down
```

---

**Tiempo total:** 10-15 minutos  
**Complejidad:** ⭐ Fácil  
**Status:** ✅ Listo  

