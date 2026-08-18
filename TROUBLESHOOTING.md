# 🚨 TROUBLESHOOTING - Problemas Comunes y Soluciones

## ❌ Problema 1: Docker Compose - "SA_PASSWORD is missing a value"

### Síntoma
```
error while interpolating services.inventario-app.environment.[]: required variable SA_PASSWORD is missing a value: error
```

### Solución ✅

**Opción A: Usar archivo .env (Recomendado)**
```bash
# El archivo .env ya está creado en C:\SistemaInventarioRefactorizado\.env
# Contiene:
# SA_PASSWORD=YourPassword123!
# ACCEPT_EULA=Y

# Ejecutar desde la carpeta del proyecto:
cd C:\SistemaInventarioRefactorizado
docker-compose up --env-file .env
```

**Opción B: Establecer variable de entorno**
```powershell
# En PowerShell (Windows):
$env:SA_PASSWORD = "YourPassword123!"
$env:ACCEPT_EULA = "Y"
docker-compose up

# En CMD (Windows):
set SA_PASSWORD=YourPassword123!
set ACCEPT_EULA=Y
docker-compose up
```

**Opción C: Editar docker-compose.yml manualmente**
```yaml
environment:
  ACCEPT_EULA: "Y"
  SA_PASSWORD: "YourPassword123!"  # Hardcode (no recomendado para producción)
```

---

## ❌ Problema 2: Tests Fallando - "Login failed for user"

### Síntoma
```
System.Data.SqlClient.SqlException : Login failed for user 'AzureAD\naryery.salazar@misalud.go.cr'.
```

### Causa
El proyecto **legacy** está tratando de conectarse a SQL Server en el sistema actual, pero:
1. No tienes SQL Server instalado localmente
2. Las credenciales son diferentes
3. El proyecto intenta conectarse con autenticación de Windows

### Soluciones ✅

**Opción 1: Instalar SQL Server Express (Recomendado para desarrollo)**
```bash
# Descargar desde: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
# Instalar SQL Server 2022 Express
# Luego cambiar la connection string en el proyecto legacy
```

**Opción 2: Usar Docker con SQL Server**
```bash
# Iniciar SQL Server en Docker
docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=YourPassword123! -p 1433:1433 mcr.microsoft.com/mssql/server:latest

# Actualizar connection string en el proyecto legacy:
# Server=localhost;Database=InventarioDb;User Id=sa;Password=YourPassword123!;Encrypt=false;
```

**Opción 3: Saltarse los tests que requieren BD**
```bash
# Ejecutar solo tests unitarios sin BD
dotnet test --filter "FullyQualifiedName~Unit"

# O saltarlos completamente
dotnet test --filter "FullyQualifiedName~CharacterizationTests" --no-build
```

---

## ❌ Problema 3: "La ruta de acceso al archivo proporcionada no existe"

### Síntoma
```
dotnet run -p SistemaInventario.Presentation
error: La ruta de acceso al archivo proporcionada no existe: SistemaInventario.Presentation.
```

### Causa
Hay **dos proyectos diferentes**:
1. **Proyecto Legacy Original** (la carpeta actual donde estás)
2. **Proyecto Refactorizado** (en `C:\SistemaInventarioRefactorizado`)

### Solución ✅

**Para el Proyecto Refactorizado (Clean Architecture - RECOMENDADO):**
```bash
cd C:\SistemaInventarioRefactorizado

# Compilar
dotnet build

# Ejecutar
dotnet run --project SistemaInventario.Presentation/SistemaInventario.Presentation.csproj

# O simplemente
dotnet run
```

**Para el Proyecto Legacy Original:**
```bash
cd C:\Users\NaryerySalazarDíaz\Downloads\Grupo-1---SistemaInventarioLegacy--main\Reinge-SistemaInventarioLegacy

# Compilar
dotnet build

# Ejecutar
dotnet run

# O si tiene tests:
dotnet test
```

---

## 📊 COMPARATIVA: Legacy vs Refactorizado

| Aspecto | Legacy Original | Refactorizado (Clean Arch) |
|---------|---|---|
| **Ubicación** | `Downloads/Grupo-1/.../Reinge-SistemaInventarioLegacy` | `C:\SistemaInventarioRefactorizado` |
| **Arquitectura** | Monolítica (1 archivo ~800 líneas) | 5 capas desacopladas |
| **Tests** | ❌ Fallan (BD requerida) | ✅ 10 tests verdes (sin BD) |
| **Seguridad** | ❌ SQL Injection presente | ✅ 100% parametrizado |
| **Recomendación** | Para análisis/diagnóstico | **Para producción** |

---

## 🎯 RECOMENDACIÓN: Usa el Proyecto Refactorizado

```bash
# 1. Ir a la carpeta correcta
cd C:\SistemaInventarioRefactorizado

# 2. Compilar (sin BD requerida)
dotnet build

# 3. Ejecutar tests (sin BD requerida)
dotnet test

# 4. Ejecutar con Docker (incluye BD automáticamente)
docker-compose up --env-file .env

# 5. O ejecutar sin Docker (con BD local)
# Primero instalar SQL Server o Docker + SQL Server
dotnet run --project SistemaInventario.Presentation
```

---

## 🔧 Verificar Que Todo Funciona

```bash
# 1. Verificar compilación
dotnet build
# Resultado esperado: ✅ Compilación correcta

# 2. Verificar tests (Clean Architecture)
cd C:\SistemaInventarioRefactorizado
dotnet test
# Resultado esperado: ✅ 10/10 tests pasados

# 3. Verificar Docker
docker-compose --version
docker --version

# 4. Verificar SQL Server en Docker
docker ps
# Resultado esperado: Ver contenedor con "mssql" en el nombre
```

---

## 📞 Soporte Rápido

| Problema | Comando Rápido |
|---------|---|
| "Port already in use" | `docker-compose down` luego `docker-compose up` |
| "Connection refused" | Esperar 30s después de `docker-compose up` (healthcheck) |
| "Build error" | `dotnet clean` luego `dotnet build` |
| "Test error (BD)" | `dotnet test --filter "FullyQualifiedName~Domain"` |
| "Docker error" | `docker system prune` (limpia caché) |

---

## ✅ Checklist Final

- [ ] Tengo dos proyectos diferentes (Legacy vs Refactorizado)
- [ ] Estoy en la carpeta correcta: `C:\SistemaInventarioRefactorizado`
- [ ] Tengo `.env` con `SA_PASSWORD` configurado
- [ ] Docker está instalado y corriendo
- [ ] `dotnet build` compila sin errores
- [ ] `dotnet test` pasa todos los tests
- [ ] `docker-compose up` inicia sin error de SA_PASSWORD

---

**¿Todo funciona? ✅ Procede a:**
```bash
docker-compose up
# La app se iniciará automáticamente en http://localhost:5000
```

**¿Aún hay problemas? 🆘 Verifica:**
1. El archivo `.env` existe en la carpeta del proyecto
2. Ejecutas comandos desde `C:\SistemaInventarioRefactorizado`
3. Docker Desktop está corriendo
4. No tienes otro contenedor usando puerto 1433 o 5000
