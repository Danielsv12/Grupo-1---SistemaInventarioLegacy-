# Prompt óptimo — Refactoring con SOLID y patrones de diseño

> Plantilla lista para copiar, pegar y adaptar. Cada sección tiene una razón de ser explicada debajo.

---

## Plantilla completa

```
Contexto del sistema:
- Lenguaje: C# / .NET 10
- Tipo de proyecto: [aplicación de consola / API / servicio]
- Restricciones conocidas: [sin DI container externo / sin NuGet / etc.]

Objetivo:
Refactoriza la clase [NombreClase] aplicando los siguientes criterios, en este orden de prioridad:
1. Extract Method para separar [validación / cálculo / notificación / persistencia]
2. Corregir code smells del catálogo de Fowler que encuentres
3. Aplicar los principios SOLID que apliquen, priorizando SRP y DIP
4. Usar los patrones de diseño más simples que resuelvan el problema (no sobre-ingeniería)

Código fuente completo a refactorizar:
[PEGAR CÓDIGO AQUÍ]

Entregables esperados:
1. Código C# compilable y completo (no pseudocódigo, no fragmentos)
2. Tabla: code smell → línea original → refactorización aplicada
3. Para cada principio SOLID aplicado: una frase explicando QUÉ cambió y POR QUÉ
4. Ejemplo de composición (cómo instanciar la clase refactorizada desde Program.cs)
5. No agregar funcionalidad nueva; solo restructurar lo existente

Restricciones de la respuesta:
- Los comentarios en el código solo cuando el POR QUÉ no es obvio
- No usar frameworks de DI externos (Autofac, Microsoft.Extensions.DI), solo constructor injection
- Mantener el mismo namespace del archivo original
```

---

## Por qué cada sección funciona

### `Contexto del sistema`
Sin esto Claude puede generar código Java, Python o C# antiguo. Especificar lenguaje y versión evita respuestas genéricas y activa reglas del compilador correctas (switch expressions, records, nullable reference types).

**ROI:** Elimina una ronda de "esto no compila en .NET 6".

---

### `Objetivo con orden de prioridad`
El orden le indica a Claude qué defender cuando hay tensión entre principios. "Extract Method primero" garantiza que la técnica pedida aparezca explícitamente, no diluida en una refactorización más ambiciosa.

**ROI:** Evita que el modelo optimice por lo que le parece más elegante en lugar de lo que pediste.

---

### `Código fuente completo`
Pegar el código real en lugar de describirlo elimina la ambigüedad. Claude no infiere nombres de métodos, tipos de parámetros ni lógica de negocio que no vio.

**ROI:** Elimina el 80% de los errores de interpretación.

---

### `Entregables numerados`
Sin lista explícita, el modelo decide qué incluir. Con la lista, cada punto actúa como un contrato: si falta alguno, es fácil pedirlo de forma quirúrgica en el siguiente turno.

**ROI:** Una sola respuesta completa en lugar de 3-4 turnos de seguimiento.

---

### `Restricciones de la respuesta`
Controlan el scope de salida. "No agregar funcionalidad nueva" previene el comportamiento por defecto del modelo de mejorar el código más allá de lo pedido.

**ROI:** El código resultante es revisable en minutos, no en horas.

---

## Variantes por caso de uso

### Para análisis rápido (sin código)
```
Tengo esta clase C#: [CÓDIGO]. 
Lista los code smells de Fowler que encuentres, en formato tabla: 
smell | línea aproximada | refactorización recomendada. 
Solo el análisis, sin código aún.
```

### Para aplicar un solo patrón
```
Aplica el patrón Strategy a la lógica de descuento de esta clase C#: [CÓDIGO].
Mantén todo lo demás igual. Muestra solo las clases nuevas y el constructor modificado.
```

### Para revisión de SOLID post-refactoring
```
Revisa si este código C# cumple SRP y DIP: [CÓDIGO].
Para cada violación: línea exacta, principio violado, cambio mínimo para corregirlo.
No reescribas todo, solo señala los puntos de mejora.
```

### Para generar tests después del refactoring
```
Tengo esta clase refactorizada con interfaces inyectadas: [CÓDIGO].
Genera tests unitarios en xUnit para GestorVentas usando Moq para mockear las interfaces.
Cubre: venta exitosa, pedido vacío, tipo de cliente inválido.
```

---

## Principios de prompt engineering aplicados aquí

| Principio | Aplicación concreta |
|-----------|---------------------|
| **Anclar en estándares** | "catálogo de Fowler", "principios SOLID" → respuesta verificable |
| **Pedir el por qué** | "explicando QUÉ cambió y POR QUÉ" → aprendizaje, no solo código |
| **Contexto mínimo suficiente** | Lenguaje + restricciones + código real; sin historia del proyecto |
| **Entregables como contrato** | Lista numerada → fácil auditar qué falta |
| **Restricciones explícitas** | "no sobre-ingeniería", "no funcionalidad nueva" → scope controlado |
| **Formato de salida definido** | "tabla: smell → línea → refactorización" → parseable, no prosa |

---

## Anti-patrones de prompt a evitar

| Anti-patrón | Por qué falla | Alternativa |
|-------------|---------------|-------------|
| "Mejora este código" | Sin criterio → el modelo optimiza lo que le parece | Listar los smells a corregir |
| "Aplica buenas prácticas" | Demasiado vago → cada modelo tiene opinión diferente | Nombrar principios específicos (SOLID, Fowler) |
| "¿Cómo refactorizarías esto?" | Genera opciones, no código | "Refactoriza aplicando X, entrega código compilable" |
| Describir el código en prosa | El modelo infiere mal → código incorrecto | Pegar el código fuente real |
| Pedir todo en un prompt | Respuesta larga e incompleta | Separar análisis / implementación / tests |
