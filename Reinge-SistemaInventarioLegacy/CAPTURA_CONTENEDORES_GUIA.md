# 📸 CAPTURA DE CONTENEDORES - GUÍA VISUAL

## 🎯 ¿QUÉ NECESITO VER?

Ejecuta este comando y comparte la salida:

```bash
docker-compose ps
```

---

## 📋 FORMATOS DE CAPTURA

### Opción 1: Captura de Pantalla (Más Fácil)

```bash
# Ejecuta en terminal
docker-compose ps

# Toma captura de pantalla (Ctrl+Print, Cmd+Shift+4, etc.)
# Pega aquí abajo
```

**Pega tu captura aquí:**
```
[AQUÍ VA TU CAPTURA]
```

---

### Opción 2: Copiar Texto (Más Exacto)

```bash
# Ejecuta y copia la salida completa
docker-compose ps

# O guarda en archivo
docker-compose ps > contenedores.txt
cat contenedores.txt
```

**Pega la salida aquí:**
```
[AQUÍ VA TU TEXTO]
```

---

### Opción 3: Salida Formateada

```bash
# Más detalles
docker-compose ps -a
```

---

## 🔍 ¿QUÉ DEBO VER?

### ✅ Estado Esperado

```
NAME                   IMAGE                                      COMMAND             SERVICE      STATUS              PORTS
reinge-app             reinge/inventario:latest                  "dotnet Reinge..."  app          Up 2 minutes (healthy)   0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest "/opt/mssql/b..."  db           Up 3 minutes (healthy)  0.0.0.0:1433->1433/tcp
```

**Indicadores de éxito:**
- ✅ 2 contenedores listados
- ✅ Ambos "Up X minutes"
- ✅ Ambos "(healthy)"
- ✅ Puertos mapeados: 8080 y 1433

---

### ⚠️ Estados Problemáticos

```
NAME                   STATUS                                      
reinge-app             Up 1 minute (unhealthy)    ← ⚠️ No healthy aún
reinge-sqlserver       Up 5 seconds               ← ⏳ Iniciando (normal)
```

---

## 📸 SI TIENES PROBLEMA CAPTURANDO

Si no puedes capturar, ejecuta esto y copia la salida:

```bash
# Información completa del estado
echo "=== ESTADO DE CONTENEDORES ===" && \
docker-compose ps && \
echo "" && \
echo "=== LOGS RECIENTES ===" && \
docker-compose logs --tail=20
```

**Pega todo aquí:**
```
[AQUÍ VA LA SALIDA]
```

---

## 🎯 INFORMACIÓN ADICIONAL ÚTIL

Si quieres proporcionar más información, ejecuta:

### Health Check Detail

```bash
docker-compose ps -a --filter "status=running"
```

### Con versiones de imagen

```bash
docker-compose ps --format "table {{.Names}}\t{{.Image}}\t{{.Status}}"
```

### Información de puertos

```bash
docker-compose ps --format "table {{.Names}}\t{{.Ports}}"
```

### Logs de un servicio

```bash
# Logs de BD
docker-compose logs db --tail=20

# Logs de App
docker-compose logs app --tail=20
```

---

## 📊 EJEMPLOS DE SALIDA

### ✅ ÉXITO (Todo funciona)

```
NAME                   IMAGE                                      COMMAND             SERVICE      STATUS              PORTS
reinge-app             reinge/inventario:latest                  "dotnet Reinge..."  app          Up 2 minutes (healthy)   0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest "/opt/mssql/b..."  db           Up 3 minutes (healthy)  0.0.0.0:1433->1433/tcp
```

**Análisis:**
- ✅ 2 servicios corriendo
- ✅ Ambos "healthy"
- ✅ Puertos accesibles
- ✅ Sistema operativo

---

### ⚠️ INICIANDO (Esperar 40 segundos)

```
NAME                   IMAGE                                      COMMAND             SERVICE      STATUS              PORTS
reinge-app             reinge/inventario:latest                  "dotnet Reinge..."  app          Up 10 seconds            0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest "/opt/mssql/b..."  db           Up 15 seconds            0.0.0.0:1433->1433/tcp
```

**Análisis:**
- ⏳ Acaba de iniciar
- ⏳ Sin "(healthy)" aún es normal
- 💡 Esperar 30-40 segundos más

**Solución:**
```bash
sleep 40
docker-compose ps
```

---

### 🔴 UNHEALTHY (Problema)

```
NAME                   IMAGE                                      COMMAND             SERVICE      STATUS                PORTS
reinge-app             reinge/inventario:latest                  "dotnet Reinge..."  app          Up 2 minutes (unhealthy)  0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest "/opt/mssql/b..."  db           Up 3 minutes (healthy)   0.0.0.0:1433->1433/tcp
```

**Análisis:**
- 🔴 App no está healthy
- ✅ BD está healthy

**Ver qué está mal:**
```bash
docker-compose logs app --tail=50
```

---

### 🛑 EXITED (No corriendo)

```
NAME                   IMAGE                                      COMMAND             SERVICE      STATUS              PORTS
reinge-app             reinge/inventario:latest                  "dotnet Reinge..."  app          Exited (1)
reinge-sqlserver       mcr.microsoft.com/mssql/server:2022-latest "/opt/mssql/b..."  db           Exited (1)
```

**Análisis:**
- 🛑 Servicios no están corriendo
- Exited (1) = Error

**Ver qué pasó:**
```bash
docker-compose logs

# Reiniciar
docker-compose up -d
```

---

### 📭 VACÍO (No iniciados)

```
NAME    IMAGE    COMMAND    SERVICE    STATUS    PORTS
```

**Análisis:**
- ❌ Compose no ha iniciado servicios

**Solucionar:**
```bash
docker-compose up -d
```

---

## 🎥 CÓMO CAPTURAR PANTALLA

### Windows
```
Shift + Impr. Pant. (Captura la pantalla)
O: Windows + Shift + S (Captura región)
```

### Mac
```
Cmd + Shift + 4 (Captura región)
Cmd + Shift + 3 (Captura pantalla completa)
```

### Linux
```
Print (Captura pantalla)
O: gnome-screenshot (en GNOME)
```

---

## 📋 FORMATO RECOMENDADO PARA COMPARTIR

**Pega así:**

```
=== SALIDA DE docker-compose ps ===

[AQUÍ LA CAPTURA O TEXTO]

=== SALIDA DE docker-compose logs (últimas 50 líneas) ===

[AQUÍ LOS LOGS]

=== SALIDA DE docker-compose ps -a (con todos) ===

[AQUÍ ESTADO COMPLETO]
```

---

## 🎯 ANÁLISIS AUTOMÁTICO

**Después de capturar, responde:**

1. ¿Hay 2 contenedores listados?
   - Sí / No

2. ¿Ambos dicen "Up"?
   - Sí / No

3. ¿Ambos dicen "(healthy)"?
   - Sí / No

4. ¿Hay error visible?
   - Sí / No
   - Si sí, ¿cuál?

5. ¿Los puertos están mapeados (8080, 1433)?
   - Sí / No

---

## 🔧 COMANDOS PARA DIFERENTES ESCENARIOS

### Si ves "unhealthy"

```bash
docker-compose logs --tail=50
docker-compose restart
sleep 40
docker-compose ps
```

### Si ves "Exited"

```bash
docker-compose logs
docker-compose up -d
```

### Si ves vacío

```bash
docker-compose up --build
sleep 60
docker-compose ps
```

### Si quieres más detalles

```bash
docker-compose exec db sqlcmd -S localhost -U sa -Q "SELECT @@VERSION"
curl http://localhost:8080/health
```

---

## 📤 CÓMO COMPARTIR CONMIGO

**Proporciona:**

1. **Captura de `docker-compose ps`**
   - Screenshot o texto copiado

2. **Captura de `docker-compose logs` (últimas 20 líneas)**
   ```bash
   docker-compose logs --tail=20
   ```

3. **Tu respuesta a preguntas arriba**
   - ¿2 contenedores? Sí/No
   - ¿Ambos "Up"? Sí/No
   - ¿Ambos "(healthy)"? Sí/No

4. **Cualquier error visible**
   - Copia el mensaje exacto

---

## 📸 EJEMPLO COMPLETO

**Aquí va tu información:**

```
=== CAPTURA DE docker-compose ps ===

NAME                   IMAGE                              STATUS              PORTS
reinge-app             reinge/inventario:latest           Up 2 minutes (healthy)   0.0.0.0:8080->8080/tcp
reinge-sqlserver       mcr.microsoft.com/mssql/server:... Up 3 minutes (healthy)  0.0.0.0:1433->1433/tcp

=== ANÁLISIS ===
✅ 2 contenedores: Sí
✅ Ambos "Up": Sí
✅ Ambos "(healthy)": Sí
✅ Errores: No
✅ Puertos: Sí

CONCLUSIÓN: TODO FUNCIONA ✅
```

---

## 🎯 PRÓXIMO PASO

1. **Ejecuta:** `docker-compose ps`
2. **Captura o copia** la salida
3. **Pega aquí abajo** en tu próximo mensaje
4. **Indicaré** si todo está bien o qué corregir

---

**Generado:** Guía de captura de contenedores  
**Uso:** Documentar estado de Docker Compose  
**Status:** Listo para análisis visual  

