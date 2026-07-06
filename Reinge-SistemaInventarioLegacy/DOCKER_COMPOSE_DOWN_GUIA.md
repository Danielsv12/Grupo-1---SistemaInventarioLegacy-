# 🛑 DOCKER COMPOSE DOWN - GUÍA COMPLETA

## 🚀 ¿QUÉ ES `docker compose down`?

Comando que **detiene y elimina** todos los servicios definidos en tu `docker-compose.yml`.

```bash
docker-compose down
```

---

## 📊 QUÉ ELIMINA vs QUÉ PRESERVA

### ✅ SE ELIMINA

```
✗ Contenedores (parados)
✗ Red personalizada
✗ Volúmenes anónimos (si se especificó)
```

### ✅ SE PRESERVA (Por defecto)

```
✓ Volúmenes nombrados (.data/sqlserver, etc.)
✓ Imágenes Docker
✓ Código fuente local
✓ Archivos .env
✓ Configuración
```

---

## 🔧 VARIANTES DEL COMANDO

### 1️⃣ Estándar: `docker-compose down`

```bash
docker-compose down
```

**Qué hace:**
- ⏹️ Detiene todos los contenedores
- 🗑️ Elimina contenedores
- 🗑️ Elimina red personalizada
- ✅ **PRESERVA volúmenes** (datos de BD)

**Caso de uso:** Parar desarrollo temporalmente

**Ejemplo:**
```bash
$ docker-compose down

[+] Running 3/3
 ✓ Container reinge-app stopped
 ✓ Container reinge-sqlserver stopped
 ✓ Network reinge-network removed

Removed containers:
- reinge-app
- reinge-sqlserver

Removed networks:
- reinge-network
```

---

### 2️⃣ Con eliminación de volúmenes: `docker-compose down -v`

```bash
docker-compose down -v
```

**Qué hace:**
- ⏹️ Detiene todos los contenedores
- 🗑️ Elimina contenedores
- 🗑️ Elimina red personalizada
- 🗑️ **ELIMINA volúmenes** (incluyendo datos de BD) ⚠️

**Caso de uso:** Reset completo (perder datos de BD)

**Advertencia:** ⚠️ **DESTRUCTIVO - Elimina datos permanentemente**

**Ejemplo:**
```bash
$ docker-compose down -v

[+] Running 4/4
 ✓ Container reinge-app stopped
 ✓ Container reinge-sqlserver stopped
 ✓ Volume reinge-sqlserver-data removed  ← ⚠️ DATOS PERDIDOS
 ✓ Network reinge-network removed

Removed: all containers, volumes, and networks
```

---

### 3️⃣ Con eliminación de imágenes: `docker-compose down --rmi`

```bash
# Eliminar solo imágenes local-built
docker-compose down --rmi local

# Eliminar todas las imágenes (incluso las base)
docker-compose down --rmi all
```

**Opciones:**
- `--rmi local` = Elimina imágenes construidas localmente (reinge/inventario)
- `--rmi all` = Elimina todas (también mcr.microsoft.com/mssql/server)

**Caso de uso:** Limpiar todo el espacio

**Ejemplo:**
```bash
$ docker-compose down --rmi local

[+] Running 5/5
 ✓ Container reinge-app stopped
 ✓ Container reinge-sqlserver stopped
 ✓ Image reinge/inventario:latest removed  ← Imagen app
 ✓ Network reinge-network removed
```

---

### 4️⃣ Combinado: `docker-compose down -v --rmi all`

```bash
docker-compose down -v --rmi all
```

**Qué hace:**
- 🗑️ Elimina contenedores
- 🗑️ Elimina volúmenes (datos)
- 🗑️ Elimina todas las imágenes
- 🗑️ Elimina red

**⚠️ ADVERTENCIA:** Reset **TOTAL Y PERMANENTE**

**Caso de uso:** Limpiar completamente para empezar de cero

---

### 5️⃣ Sin eliminar red: `docker-compose down --no-networks`

```bash
docker-compose down --no-networks
```

**Qué hace:**
- ⏹️ Detiene contenedores
- 🗑️ Elimina contenedores
- ✅ **PRESERVA red personalizada**

**Caso de uso:** Raro, mantener red para otros servicios

---

## 📊 MATRIZ DE OPCIONES

| Comando | Contenedores | Volúmenes | Imágenes | Red |
|---------|--------------|-----------|----------|-----|
| `down` | ❌ Elimina | ✅ Preserva | ✅ Preserva | ❌ Elimina |
| `down -v` | ❌ Elimina | ❌ **Elimina** | ✅ Preserva | ❌ Elimina |
| `down --rmi local` | ❌ Elimina | ✅ Preserva | ❌ Elimina local | ❌ Elimina |
| `down --rmi all` | ❌ Elimina | ✅ Preserva | ❌ **Elimina todas** | ❌ Elimina |
| `down -v --rmi all` | ❌ Elimina | ❌ **Elimina** | ❌ **Elimina** | ❌ Elimina |

---

## 🧪 EJEMPLOS PRÁCTICOS

### Escenario 1: Pausa de Desarrollo (Sin perder datos)

```bash
# Detener para fin del día
docker-compose down

# A la mañana siguiente
docker-compose up -d
# ✅ BD sigue intacta, datos preservados
```

---

### Escenario 2: Reset de BD (Perder datos)

```bash
# Eliminar todo
docker-compose down -v

# Reconstruir
docker-compose up --build
# ✅ BD vacía, nuevo setup
```

---

### Escenario 3: Limpiar Todo el Proyecto

```bash
# Nuclear option
docker-compose down -v --rmi all

# Reiniciar desde cero
docker-compose build
docker-compose up -d
# ✅ Proyecto completamente limpio
```

---

### Escenario 4: Cambiar de Configuración

```bash
# Detener
docker-compose down

# Editar .env
nano .env

# Reiniciar con nueva config
docker-compose up --build
```

---

## 🔍 QUÉ VAS A VER

### Ejecución Típica

```bash
$ docker-compose down

[+] Running 4/4
 ✓ Container reinge-app Stopped
 ✓ Container reinge-sqlserver Stopped
 ✓ Network reinge-network removed
```

**Línea por línea:**
- `Running 4/4` = 4 tareas en progreso
- `Container reinge-app Stopped` = App detenida
- `Container reinge-sqlserver Stopped` = BD detenida
- `Network reinge-network removed` = Red eliminada

---

## 🆚 COMPARATIVA: DOWN vs OTRAS OPCIONES

### `docker-compose pause`

```bash
docker-compose pause
```

- ⏸️ Pausa contenedores (sin eliminar)
- ✅ Rápido reanudar
- ✅ Preserva todo

**Uso:** Pausa temporal

---

### `docker-compose stop`

```bash
docker-compose stop
```

- ⏹️ Detiene contenedores (sin eliminar)
- ✅ Puedes hacer `up` de nuevo
- ✅ Preserva configuración

**Uso:** Parada ordenada

---

### `docker-compose down`

```bash
docker-compose down
```

- 🗑️ Detiene y elimina contenedores
- 🗑️ Elimina red
- ✅ Preserva volúmenes (datos)

**Uso:** Parada completa, pero con datos

---

### `docker-compose down -v`

```bash
docker-compose down -v
```

- 🗑️ Detiene y elimina contenedores
- 🗑️ Elimina volúmenes (DATOS)
- 🗑️ Elimina red

**Uso:** Reset completo

---

## 📋 WORKFLOW TÍPICO

### Inicio de Día

```bash
# Iniciar servicios
docker-compose up -d

# Verificar
docker-compose ps
```

### Durante el Día

```bash
# Trabajo normal
# Ver logs si hay problema
docker-compose logs -f

# Reiniciar servicio específico
docker-compose restart db
```

### Fin de Día

```bash
# Parar servicios (preservar datos)
docker-compose down

# Verificar que paró
docker-compose ps
# (Debería estar vacío)
```

### Día Siguiente

```bash
# Reiniciar (datos intactos)
docker-compose up -d

# Continuar trabajo
```

---

## ⚠️ CUIDADO: DATOS PERDIDOS

### Antes de `docker-compose down -v`

```bash
# BACKUP (si necesitas preservar datos)
docker-compose exec db sqlcmd -S localhost -U sa -Q "
  BACKUP DATABASE [Inventario]
  TO DISK = '/var/opt/mssql/backup/backup.bak'
"

# Luego sí
docker-compose down -v
```

---

## 🧹 LIMPIAR DESPUÉS DE DOWN

### Ver qué quedó

```bash
# Volúmenes orfos
docker volume ls
docker volume ls -f dangling=true

# Imágenes
docker images

# Networks
docker network ls
```

### Limpiar orfos

```bash
# Volúmenes orfos
docker volume prune

# Imágenes no usadas
docker image prune

# Networks no usadas
docker network prune

# Todo junto
docker system prune -a --volumes
```

---

## 🔧 TROUBLESHOOTING

### Error: "docker-compose.yml not found"

```bash
# Asegúrate de estar en el directorio correcto
cd /path/to/Reinge-SistemaInventarioLegacy
docker-compose down
```

---

### Error: "Cannot connect to Docker daemon"

```bash
# Docker no está corriendo
# Windows/Mac: Abre Docker Desktop
# Linux: sudo systemctl start docker
```

---

### Error: "permission denied"

```bash
# Linux: Necesitas permisos
sudo docker-compose down

# O agregar usuario a grupo
sudo usermod -aG docker $USER
newgrp docker
```

---

### Pregunta: ¿Se perdieron mis datos?

```bash
# Si usaste: docker-compose down -v

# ❌ Los volúmenes fueron eliminados
# ✅ Pero si tienes backup:
docker restore < backup.sql

# Si NO tienes backup:
# 😞 Datos están perdidos
```

---

## 🎯 RECETA RÁPIDA

### Para "pausar" el proyecto (preservar datos)

```bash
docker-compose down
```

### Para "resetear" (perder datos)

```bash
docker-compose down -v
```

### Para "limpiar todo" (empezar de cero)

```bash
docker-compose down -v --rmi all
```

---

## 📊 ANTES Y DESPUÉS

### Antes de `docker-compose down`

```bash
$ docker-compose ps

NAME                   STATUS
reinge-app             Up 2 hours (healthy)
reinge-sqlserver       Up 2 hours (healthy)

$ ls .data/
sqlserver/  app/
```

### Después de `docker-compose down`

```bash
$ docker-compose ps
# (Vacío, sin contenedores)

$ ls .data/
# Volúmenes aún existen (datos preservados)
sqlserver/  app/
```

### Después de `docker-compose down -v`

```bash
$ docker-compose ps
# (Vacío, sin contenedores)

$ ls .data/
# Volúmenes eliminados (datos perdidos)
# (Directorio vacío o no existe)
```

---

## 💡 TIPS

### Verificar antes de down -v

```bash
# Ver qué volúmenes se eliminarán
docker volume ls | grep reinge

# Si lo vas a lamentar, hacer backup:
docker-compose exec db sqlcmd -S localhost -U sa -Q "BACKUP DATABASE ..."
```

### Comando con confirmación

```bash
#!/bin/bash
read -p "¿Eliminar volúmenes? (s/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Ss]$ ]]; then
  docker-compose down -v
else
  docker-compose down
fi
```

### Ver lo que va a eliminar

```bash
# Listar lo que eliminaría
docker-compose config | grep -A 20 "volumes:"

# Luego ejecutar
docker-compose down -v
```

---

## 🆘 EMERGENCIAS

### Accidental `docker-compose down -v` (datos perdidos)

```bash
# ❌ Contenedores y volúmenes eliminados
# ✅ Pero imágenes aún existen

# Recuperar (si tienes backup)
docker-compose up -d

# Restaurar datos desde backup
docker-compose exec db sqlcmd ... RESTORE DATABASE ...

# Si NO tienes backup: 😞 Lamentable
```

### Olvidé el estado de un servicio

```bash
# Ver historial
docker ps -a --format "table {{.Names}}\t{{.Status}}"

# Ver logs pasados
docker-compose logs --tail=1000
```

---

## 📚 RESUMEN

| Acción | Comando | Datos | Imágenes | Contenedores |
|--------|---------|-------|----------|--------------|
| Pausa temporal | `down` | ✅ Preserva | ✅ Preserva | ❌ Elimina |
| Reset con datos | `down -v` | ❌ Elimina | ✅ Preserva | ❌ Elimina |
| Limpieza total | `down -v --rmi all` | ❌ Elimina | ❌ Elimina | ❌ Elimina |

---

**Generado:** Guía de `docker-compose down`  
**Uso:** Detener y eliminar servicios  
**Cuidado:** `-v` es destructivo con datos  

