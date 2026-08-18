# 📚 GUÍA DE USO - Sistema de Gestión de Inventario

## 🚀 Acceder a la Consola de la Aplicación

### Opción 1: Ver logs en tiempo real
```bash
docker logs -f inventario-app
```

### Opción 2: Entrar en el contenedor y ver la consola
```bash
# Entrar al contenedor
docker exec -it inventario-app bash

# Dentro del contenedor, ejecutar la app
dotnet SistemaInventario.Presentation.dll
```

### Opción 3: Ver menú principal (después de `docker-compose up`)
Si ejecutaste `docker-compose up` (sin -d), la consola debería mostrar el menú automáticamente.

---

## 📋 EJEMPLOS DE USO

### Ejemplo 1: Registrar un Producto

```
╔════════════════════════════════════════╗
║   Sistema de Gestión de Inventario    ║
║        PYME - Clean Architecture       ║
╚════════════════════════════════════════╝

1. Gestionar Productos
2. Registrar Movimientos
3. Salir

Seleccione una opción: 1

╔════════════════════════════════════════╗
║         Gestión de Productos          ║
╚════════════════════════════════════════╝

1. Registrar Producto
2. Buscar Producto
3. Volver

Seleccione una opción: 1

╔════════════════════════════════════════╗
║       Registrar Nuevo Producto        ║
╚════════════════════════════════════════╝

Nombre del producto: Laptop Dell XPS 13
ID Categoría (1=Electrónica, 2=Ropa, 3=Alimentos): 1
Precio: 1299.99
Stock inicial: 5
Stock mínimo: 2

✓ Producto registrado exitosamente. ID: 1
```

---

### Ejemplo 2: Buscar un Producto

```
Seleccione una opción: 2

╔════════════════════════════════════════╗
║         Buscar Producto               ║
╚════════════════════════════════════════╝

Ingrese término de búsqueda: Laptop

Resultados:

ID: 1 | Nombre: Laptop Dell XPS 13
  Categoría: Electrónica | Precio: $1299.99
  Stock: 5 | Mínimo: 2
  Bajo Stock: NO
```

---

### Ejemplo 3: Registrar Movimiento (Entrada de Stock)

```
Seleccione una opción: 2

╔════════════════════════════════════════╗
║    Registrar Movimiento de Stock      ║
╚════════════════════════════════════════╝

ID del Producto: 1

Tipo de Movimiento:
1. Entrada (Recepción)
2. Salida (Despacho)
Seleccione: 1

Cantidad: 10
Usuario: admin
Observaciones (opcional): Compra a proveedor XYZ

✓ Movimiento registrado exitosamente. ID: 1
```

---

### Ejemplo 4: Registrar Movimiento (Salida de Stock)

```
ID del Producto: 1

Tipo de Movimiento:
1. Entrada (Recepción)
2. Salida (Despacho)
Seleccione: 2

Cantidad: 3
Usuario: vendedor1
Observaciones (opcional): Venta a cliente ABC

✓ Movimiento registrado exitosamente. ID: 2
```

---

## 🎯 CASOS DE USO PASO A PASO

### Caso 1: PYME recibe mercancía nueva

```
1. Menú Principal → "Gestionar Productos"
2. "Registrar Producto"
   - Nombre: Monitor Samsung 27"
   - Categoría: 1 (Electrónica)
   - Precio: 299.99
   - Stock: 0 (no hay aún)
   - Stock mínimo: 1
3. Menú Principal → "Registrar Movimientos"
   - Producto ID: 2
   - Tipo: Entrada
   - Cantidad: 20 (llega del proveedor)
   - Usuario: bodeguero1
   - Observación: "Recepción de compra PO#12345"
```

**Resultado**: BD ahora tiene 20 monitores en stock.

---

### Caso 2: Cliente compra un producto

```
1. Menú Principal → "Registrar Movimientos"
   - Producto ID: 1 (Laptop)
   - Tipo: Salida
   - Cantidad: 2
   - Usuario: vendedor2
   - Observación: "Venta a cliente corporativo"
```

**Resultado**: Stock de laptops pasa de 15 a 13.

---

### Caso 3: Verificar productos con bajo stock

```
1. Menú Principal → "Gestionar Productos"
2. "Buscar Producto"
   - Búsqueda: "" (dejar vacío para ver todos)
   
Resultados mostrarán:
- Bajo Stock: NO (para productos arriba del mínimo)
- Bajo Stock: SÍ (para productos en crítico)
```

---

## 📊 DATOS DE PRUEBA INICIALES

La BD se inicializa con:

**Categorías**:
```
ID: 1 → Electrónica
ID: 2 → Ropa
ID: 3 → Alimentos
```

**Tipos de Movimiento**:
```
ID: 1 → Entrada
ID: 2 → Salida
```

---

## 🔄 FLUJO TÍPICO DE UNA PYME

```
Mañana:
├─ Recepción de mercancía (Entrada)
├─ Registrar nuevos productos
└─ Verificar stock

Tarde:
├─ Procesar ventas (Salida)
├─ Buscar productos disponibles
└─ Generar reportes

Noche:
└─ Verificar alertas de bajo stock
```

---

## 💾 DATOS PERSISTEN EN

Los datos se guardan en:
- **Base de datos**: `InventarioDb` (en SQL Server)
- **Volumen Docker**: `sqlserver_data`

Incluso si detienes `docker-compose down`, los datos persisten.
Para limpiar: `docker-compose down -v` (elimina datos).

---

## 🆘 SI ALGO NO FUNCIONA

### Error: "Conectando a: localhost"
```
❌ Significa que la app no encontró la BD en Docker
✅ Solución: Espera 30-60 segundos, la BD tarda en iniciar
```

### Error: "Login failed for user 'sa'"
```
❌ Contraseña incorrecta o BD no lista
✅ La app reintentará automáticamente (restart: on-failure)
```

### No veo la consola
```
✅ Ejecuta: docker logs -f inventario-app
```

---

## 📞 RESUMEN RÁPIDO

```bash
# 1. Iniciar todo
docker-compose up

# 2. En otra terminal, ver logs
docker logs -f inventario-app

# 3. Cuando veas el menú, ¡ya está listo!
# 4. Usa 1, 2, 3 para navegar
# 5. Sigue los prompts
```

---

**¡Listo para usar!** 🚀
