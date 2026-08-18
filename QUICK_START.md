# ⚡ CÓMO EJECUTAR - INSTRUCCIONES RÁPIDAS

## 🎯 Paso 1: Asegúrate de estar en la carpeta correcta

```bash
cd C:\SistemaInventarioRefactorizado
```

## 🎯 Paso 2: Compilar y testear

```bash
dotnet build
dotnet test
```

**Resultado esperado**:
```
✅ Compilación correcta
✅ 10/10 tests pasados
```

## 🎯 Paso 3: Ejecutar con Docker (SIN necesidad de SQL Server local)

```bash
# El archivo .env ya tiene las variables configuradas
docker-compose up

# Resultado esperado:
# - SQL Server iniciándose
# - App conectándose a BD
# - Menú CLI apareciendo
```

Si obtienes error de `SA_PASSWORD`, ejecuta:
```bash
docker-compose up --env-file .env
```

## 🎯 Paso 4: Usar la aplicación

Una vez que docker-compose esté ejecutándose, verás el menú en la consola:

```
╔════════════════════════════════════════╗
║   Sistema de Gestión de Inventario    ║
║        PYME - Clean Architecture       ║
╚════════════════════════════════════════╝

1. Gestionar Productos
2. Registrar Movimientos
3. Salir

Seleccione una opción:
```

---

## 🛑 PROBLEMAS COMUNES

### ❌ "SA_PASSWORD is missing"
**Solución**:
```bash
docker-compose up --env-file .env
```

### ❌ "Connection refused"
**Solución**: Esperar 30 segundos (SQL Server necesita tiempo para iniciar)

### ❌ "Port 1433 already in use"
**Solución**:
```bash
docker-compose down
docker-compose up
```

---

## 📊 VERIFICAR QUE TODO FUNCIONA

```bash
# 1. Verificar compilación
dotnet build
# Debe terminar sin errores

# 2. Verificar tests
dotnet test
# Debe mostrar: ✅ 10/10 tests pasados

# 3. Verificar Docker
docker ps
# Debe mostrar 2 contenedores: sqlserver + app

# 4. Verificar BD
docker logs inventario-app | grep "Database migrated"
# Debe mostrar que las migraciones se ejecutaron
```

---

## 🎯 ALTERNATIVA: Ejecutar SIN Docker (requiere SQL Server local)

Si tienes SQL Server instalado localmente:

```bash
# 1. Cambiar connection string en appsettings.json
# 2. Ejecutar
dotnet run --project SistemaInventario.Presentation
```

---

**¿Lista la app? ✅ Listo para producción**
