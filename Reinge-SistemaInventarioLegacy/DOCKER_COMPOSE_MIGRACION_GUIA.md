# 🔄 GUÍA DE MIGRACIÓN: De tu docker-compose.yml al Optimizado

## 📋 Resumen de Cambios

**De:** Tu `docker-compose.yml` (con problemas de seguridad)  
**A:** `docker-compose-produccion.yml` (seguro y flexible)

**Problemas Resueltos:**
1. ✅ Credenciales hardcodeadas → Variables de entorno
2. ✅ Contraseña débil → Contraseña fuerte
3. ✅ Configuración fija → Flexible (dev/prod)
4. ✅ Sin logging → Logging centralizado
5. ✅ Sin persistencia → Volúmenes persistentes

---

## 🚀 PASO 1: Generar Credenciales Fuertes

### Opción A: Usar OpenSSL (Linux/Mac)

```bash
# Generar contraseña aleatoria segura (20 caracteres)
openssl rand -base64 20 | tr -d "=+/" | cut -c1-20
# Resultado: eK7mN9xQ2vW4pL8rJ5tS
```

### Opción B: Usar PowerShell (Windows)

```powershell
-join ((33..126) | Get-Random -Count 20 | % {[char]$_})
# Resultado: P@Wx9Kl#mN2vQrLsT8uY
```

### Opción C: Usar LastPass Generator

https://www.lastpass.com/features/password-generator

**Requisitos mínimos:**
- ✅ 20+ caracteres (producción)
- ✅ Mayúsculas, minúsculas, números
- ✅ Símbolos especiales (excepto $, ", ', \)
- ✅ NO patrones predecibles

**Tu contraseña actual es débil:**
```
Reing2026$Fuerte
└─ 16 caracteres: No es suficiente
└─ Patrón año: 2026 (predecible)
└─ Palabra española: Legible
```

**Contraseña fuerte recomendada:**
```
K8mP2xW5jL9nQ4tR7vS1
└─ 20 caracteres: Seguro
└─ Random: No predecible
└─ Alfanumérico: Fuerte
```

---

## 🔐 PASO 2: Crear Archivo .env

### Copiar Template

```bash
cp .env.produccion.example .env
```

### Editar .env

```bash
nano .env  # Linux/Mac
# o
notepad .env  # Windows
```

### Contenido

```env
# COMPOSE
COMPOSE_PROJECT_NAME=reinge
ENVIRONMENT=Production

# SQL SERVER
DB_SA_PASSWORD=K8mP2xW5jL9nQ4tR7vS1  # ← Tu nueva contraseña
DB_HOSTNAME=db
DB_PORT=1433
DB_NAME=Inventario
DB_MEMORY_MB=2048
DB_DATA_PATH=./.data/sqlserver
DB_LOG_PATH=./.data/sqlserver/logs

# APLICACIÓN
APP_VERSION=latest
APP_PORT=8080
APP_LOG_PATH=./.data/app/logs
LOG_LEVEL=Information
```

---

## 🚫 PASO 3: Proteger .env (NO commitear)

### Agregar a .gitignore

```bash
# Editar o crear .gitignore
echo ".env" >> .gitignore
echo ".env.*.local" >> .gitignore
echo ".data/" >> .gitignore
```

### Verificar

```bash
git status
# Deberías NO ver .env aquí

git check-ignore .env
# Debería imprimir: .env
```

---

## 🗑️ PASO 4: Remover Credenciales del Historial Git (Crítico)

### Opción A: Si nunca pushaste a main

```bash
# Solo en rama local, no en origin
git rm --cached docker-compose.yml
echo "docker-compose.yml" >> .gitignore
git add .gitignore
git commit -m "Remove hardcoded credentials"
```

### Opción B: Si ya pushiste a main (⚠️ Crítico)

**Las credenciales están comprometidas.** Debes:

1. Cambiar contraseña en BD inmediatamente
2. Remover del historial git:

```bash
# Instalar git-filter-repo (alternativa a BFG)
pip install git-filter-repo

# Remover docker-compose.yml del historial
git filter-repo --path docker-compose.yml

# Force push (⚠️ Cuidado: reescribe historial)
git push origin --force-with-lease
```

**ADVERTENCIA:** Notificar a todo el equipo:
- Todos deben re-clonar el repo
- Cambiar credenciales expuestas
- Auditoría de logs

---

## 📋 PASO 5: Usar docker-compose-produccion.yml

### Opción A: Renombrar (Recomendado)

```bash
# Respaldar original
cp docker-compose.yml docker-compose.yml.backup

# Usar nueva versión
cp docker-compose-produccion.yml docker-compose.yml
```

### Opción B: Usar -f explícito

```bash
docker-compose -f docker-compose-produccion.yml up -d
```

### Opción C: Tener ambas

```bash
# Desarrollo
docker-compose -f docker-compose.dev.yml up -d

# Producción
docker-compose -f docker-compose-produccion.yml up -d
```

---

## 🧪 PASO 6: Probar Migración (Sin BD existente)

### Preparar

```bash
# Crear directorios para volúmenes
mkdir -p .data/sqlserver
mkdir -p .data/sqlserver/logs
mkdir -p .data/app/logs
mkdir -p .backups
```

### Iniciar

```bash
# Usar nueva versión con .env
docker-compose -f docker-compose-produccion.yml up -d
```

### Monitorear Startup

```bash
# Ver logs en vivo
docker-compose logs -f

# Esperar a que BD esté lista (~40 segundos)
# Deberías ver:
# ✅ "SQL Server 2022 ready to accept connections"
# ✅ "Application started successfully"
```

### Verificar Conectividad

```bash
# BD
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT @@VERSION"

# Aplicación
docker-compose exec app curl http://localhost:8080/health

# Logs
docker-compose logs db | grep "ready"
docker-compose logs app | grep "Started"
```

### Health Checks

```bash
# Ver estado
docker-compose ps

# Deberías ver:
# NAME                 STATUS
# reinge-sqlserver     Up 2 minutes (healthy)
# reinge-app           Up 1 minute (healthy)
```

---

## 🔄 PASO 7: Restaurar BD Existente (Opcional)

### Si tienes backup

```bash
# Copiar archivo .bak al directorio compartido
cp tu-backup.bak .backups/

# Entrar en BD
docker-compose exec db bash

# Restaurar
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -Q "
  RESTORE DATABASE [Inventario]
  FROM DISK = '/var/opt/mssql/backup/tu-backup.bak'
"
```

### Generar Script SQL

```bash
# Entrar en BD
docker-compose exec db bash

# Ejecutar script de setup
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${MSSQL_SA_PASSWORD}" -i /docker-entrypoint-initdb.d/01-setup.sql
```

---

## 🧹 PASO 8: Limpiar Versión Anterior (Opcional)

### Detener servicios viejos (si están corriendo)

```bash
# Si aún estaban corriendo
docker stop reinge-sqlserver reinge-app
docker rm reinge-sqlserver reinge-app
docker volume rm reinge_sqlserver-data
```

### Limpiar imágenes antiguas

```bash
docker image prune

# O específicamente
docker rmi inventario-app:latest
```

---

## ✅ PASO 9: Verificación Final

### Checklist

- [ ] `.env` creado y NO commiteado
- [ ] Credenciales removidas del git history
- [ ] `.gitignore` actualizado
- [ ] `docker-compose-produccion.yml` en uso
- [ ] `docker-compose ps` muestra 2 healthy
- [ ] BD responde a queries
- [ ] Aplicación responde en http://localhost:8080
- [ ] Logs visibles sin errores
- [ ] Volúmenes persistentes creados

### Comandos de Verificación

```bash
# Estado general
docker-compose ps
docker-compose logs -f

# BD
docker-compose exec db sqlcmd -S localhost,1433 -U sa -Q "SELECT DB_NAME()"

# Aplicación
curl http://localhost:8080/health

# Volúmenes
ls -la .data/

# Variables de entorno cargadas
docker-compose config | grep DB_SA_PASSWORD
# Debería mostrar: DB_SA_PASSWORD: K8mP2xW5jL9nQ4tR7vS1
```

---

## 🔧 TROUBLESHOOTING COMÚN

### "Error: Missing required environment variable: DB_SA_PASSWORD"

**Causa:** .env no existe o no está siendo leído

**Solución:**
```bash
# Verificar que existe
ls -la .env

# Verificar que es readable
cat .env | grep DB_SA_PASSWORD

# Si no está, crear
cp .env.produccion.example .env
```

---

### "Connection timeout"

**Causa:** BD aún no está lista

**Solución:**
```bash
# Esperar más
sleep 40
docker-compose up -d

# Ver health check
docker-compose ps db
# STATUS debe decir "healthy"

# Ver logs de BD
docker-compose logs db | tail -20
```

---

### "Access denied: sa password"

**Causa:** Contraseña en .env no coincide con BD

**Solución:**
```bash
# Verificar .env
cat .env | grep DB_SA_PASSWORD

# Resetear todo
docker-compose down -v
docker-compose up -d
```

---

## 📊 COMPARATIVA: Antes vs Después

### Antes (Tu versión)

```yaml
db:
  environment:
    MSSQL_SA_PASSWORD: "Reing2026$Fuerte"  # ⚠️  Visible en repo
```

```bash
$ git log --oneline
abc123 Added docker compose
# Credencial visible en historial
```

### Después (Optimizado)

```yaml
db:
  environment:
    MSSQL_SA_PASSWORD: ${DB_SA_PASSWORD:?error}  # ✅ Variable
```

```bash
$ cat .env
DB_SA_PASSWORD=K8mP2xW5jL9nQ4tR7vS1  # ✅ Local, no en repo

$ git log --oneline
def456 Use environment variables
abc123 Remove hardcoded credentials
# Credencial removida del historial
```

---

## 🎯 PRÓXIMOS PASOS

### En Desarrollo
1. ✅ Seguir esta guía (hoy)
2. ✅ Testear en local
3. ✅ Notificar al equipo

### En Staging
1. ✅ Usar nuevas credenciales
2. ✅ Ejecutar tests de integración
3. ✅ Verificar performance

### En Producción
1. ✅ Audit de cambios
2. ✅ Backup antes de cambiar
3. ✅ Deploy en off-peak hours
4. ✅ Monitoreo por 24h

---

## 🆘 SOPORTE

Si algo falla:

1. Ver logs: `docker-compose logs -f`
2. Revisar .env: `cat .env`
3. Reiniciar: `docker-compose restart`
4. Reset total: `docker-compose down -v && docker-compose up -d`

---

**Tiempo estimado:** 15-30 minutos  
**Complejidad:** ⭐⭐ (Fácil a Moderado)  
**Riesgo:** 🟢 Bajo (si sigues los pasos)  
**Impacto:** 🔴 Alto (Seguridad crítica)  

