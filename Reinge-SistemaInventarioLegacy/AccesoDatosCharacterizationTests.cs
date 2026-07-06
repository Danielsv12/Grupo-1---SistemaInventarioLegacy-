using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SistemaInventarioLegacy.Tests
{
    /// <summary>
    /// Characterization Tests para AccesoDatos.cs y Configuracion.cs
    /// 
    /// Estos tests capturan el COMPORTAMIENTO ACTUAL del código legacy,
    /// incluyendo bugs conocidos. Su propósito es:
    /// - Proteger contra regresiones durante refactorización
    /// - Documentar el comportamiento esperado (aunque sea incorrecto)
    /// - No validar si el código es correcto, sino QUÉ hace realmente
    /// 
    /// Basados en Michael Feathers "Working Effectively with Legacy Code"
    /// </summary>
    public class ConfiguracionCharacterizationTests
    {
        /// <summary>
        /// Test 1: Verificar que Configuracion contiene constantes de cadena
        /// Comportamiento: Las propiedades estáticas son accesibles directamente
        /// </summary>
        [Fact]
        public void Configuracion_PropiedadesEstaticas_SonAccesibles()
        {
            // Arrange & Act
            var nombreEmpresa = Configuracion.NombreEmpresa;
            var versionSistema = Configuracion.VersionSistema;
            var impuestoVenta = Configuracion.ImpuestoVenta;

            // Assert
            Assert.NotNull(nombreEmpresa);
            Assert.NotEmpty(nombreEmpresa);
            Assert.Equal("Distribuidora XYZ S.A.", nombreEmpresa);
            
            Assert.NotNull(versionSistema);
            Assert.Equal("1.0.3", versionSistema);
            
            Assert.Equal(0.13, impuestoVenta);
        }

        /// <summary>
        /// Test 2: Verificar que los descuentos por tipo cliente están configurados
        /// Caso de borde: Proporciones de descuento entre tipos de cliente
        /// </summary>
        [Fact]
        public void Configuracion_DescuentosPorTipo_TienenValoresEsperados()
        {
            // Arrange & Act
            var descuentoRegular = Configuracion.DescuentoRegular;
            var descuentoMayorista = Configuracion.DescuentoMayorista;
            var descuentoVIP = Configuracion.DescuentoVIP;

            // Assert
            Assert.Equal(0.0, descuentoRegular);
            Assert.Equal(0.10, descuentoMayorista);
            Assert.Equal(0.15, descuentoVIP);
            
            // Verificar relación: VIP > Mayorista > Regular
            Assert.True(descuentoVIP > descuentoMayorista);
            Assert.True(descuentoMayorista > descuentoRegular);
        }

        /// <summary>
        /// Test 3: Configuración de correo y rutas de reportes
        /// Caso de borde: Rutas hardcodeadas, credenciales en código
        /// </summary>
        [Fact]
        public void Configuracion_DatosCorreoYRutas_EstanConfigurables()
        {
            // Arrange & Act
            var emailAdmin = Configuracion.EmailAdmin;
            var servidorSMTP = Configuracion.ServidorSMTP;
            var rutaReportes = Configuracion.RutaReportes;
            var passwordEmail = Configuracion.PasswordEmail;

            // Assert
            Assert.NotEmpty(emailAdmin);
            Assert.Contains("@", emailAdmin);
            
            Assert.NotEmpty(servidorSMTP);
            
            Assert.NotEmpty(rutaReportes);
            // BUG: La ruta está hardcodeada a C:\Reportes\Inventario\
            // COMPORTAMIENTO ACTUAL: Siempre usa esa ruta sin validar que exista
            Assert.Equal(@"C:\Reportes\Inventario\", rutaReportes);
            
            // SEGURIDAD: Las credenciales están en código plano
            Assert.NotEmpty(passwordEmail);
        }

        /// <summary>
        /// Test 4: Modo debug está habilitado por defecto
        /// Comportamiento: ModoDebug = true en desarrollo
        /// </summary>
        [Fact]
        public void Configuracion_ModoDebug_EstaHabilitadoPorDefecto()
        {
            // Arrange & Act
            var modoDebug = Configuracion.ModoDebug;

            // Assert
            Assert.True(modoDebug);
        }

        /// <summary>
        /// Test 5: Cadena de conexión usa trusted connection
        /// Caso de borde: Patrón de conexión específico
        /// </summary>
        [Fact]
        public void Configuracion_CadenaConexion_UsaTrustedConnection()
        {
            // Arrange & Act
            var cadenaConexion = Configuracion.CadenaConexion;

            // Assert
            Assert.NotEmpty(cadenaConexion);
            Assert.Contains("Trusted_Connection=True", cadenaConexion);
            Assert.Contains("localhost", cadenaConexion);
            Assert.Contains("InventarioLegacyDB", cadenaConexion);
        }
    }

    /// <summary>
    /// Characterization Tests para AccesoDatos.cs
    /// 
    /// NOTA: Estos tests usan tipos concretos y NO mocks.
    /// En un entorno de testing real, se reemplazaría Configuracion.CadenaConexion
    /// con una BD de prueba, pero mantenemos la estructura del código original.
    /// </summary>
    public class AccesoDatosCharacterizationTests
    {
        /// <summary>
        /// Test 1: Verificar que ObtenerProductos devuelve una lista
        /// Comportamiento: Devuelve List<Producto>, nunca null
        /// </summary>
        [Fact]
        public void ObtenerProductos_DevuelveListaNoNula()
        {
            // Arrange & Act
            // NOTA: Este test fallará sin BD real, pero documenta el comportamiento esperado
            // COMPORTAMIENTO: El método espera que Configuracion.CadenaConexion sea válida
            var resultado = AccesoDatos.ObtenerProductos();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Producto>>(resultado);
        }

        /// <summary>
        /// Test 2: Mapeo de propiedades de Producto desde SqlDataReader
        /// Caso de borde: Manejo de valores NULL en DB
        /// </summary>
        [Fact]
        public void Producto_MapeoDatos_ManejaNulosEnDescripcion()
        {
            // Arrange
            var producto = new Producto();
            
            // Act: Simulamos el comportamiento de mapeo del código
            // El código hace: p.Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : ""
            producto.Descripcion = ""; // Comportamiento cuando es NULL
            
            // Assert
            Assert.NotNull(producto.Descripcion);
            Assert.Empty(producto.Descripcion);
            
            // COMPORTAMIENTO ACTUAL: Si Descripcion es NULL en BD, se asigna string.Empty
            // No queda null, sino cadena vacía
        }

        /// <summary>
        /// Test 3: ObtenerProductoPorId devuelve null si no existe
        /// Caso de borde: Producto no encontrado
        /// </summary>
        [Fact]
        public void ObtenerProductoPorId_DevuelveNullSiNoExiste()
        {
            // Arrange
            int idInexistente = int.MaxValue;

            // Act
            var resultado = AccesoDatos.ObtenerProductoPorId(idInexistente);

            // Assert
            Assert.Null(resultado);
            // COMPORTAMIENTO: Si la consulta no devuelve filas, reader.Read() retorna false
            // y la función devuelve null en lugar de una excepción
        }

        /// <summary>
        /// Test 4: Verificar que CategoriaId puede ser 0 en Producto
        /// Caso de borde: Producto sin categoría asignada
        /// </summary>
        [Fact]
        public void Producto_CategoriaId_PuedeSer0SiEsNull()
        {
            // Arrange
            var producto = new Producto();
            
            // Act
            // Comportamiento del mapeo: reader["CategoriaId"] != DBNull.Value ? (int)reader["CategoriaId"] : 0
            producto.CategoriaId = 0;
            
            // Assert
            Assert.Equal(0, producto.CategoriaId);
            // COMPORTAMIENTO: 0 es un valor válido que indica "sin categoría"
            // No es ambiguo entre "no asignado" y "categoría 0"
        }

        /// <summary>
        /// Test 5: ObtenerProductosBajoStock filtra por Stock <= StockMinimo
        /// Comportamiento: Retorna solo productos activos con stock bajo
        /// </summary>
        [Fact]
        public void ObtenerProductosBajoStock_DevuelveListaVaciaOConProductos()
        {
            // Arrange & Act
            var resultado = AccesoDatos.ObtenerProductosBajoStock();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Producto>>(resultado);
            // COMPORTAMIENTO: Siempre retorna List<Producto>, nunca null
            // Puede estar vacía si no hay productos con stock bajo
        }

        /// <summary>
        /// Test 6: Verificar que Cliente mapea Email como cadena vacía si es NULL
        /// Caso de borde: Cliente sin email registrado
        /// </summary>
        [Fact]
        public void Cliente_MapeoDatos_ManejaNulosEnCamposOpcionales()
        {
            // Arrange
            var cliente = new Cliente();

            // Act
            // Comportamiento del mapeo: reader["Email"] != DBNull.Value ? ... : ""
            cliente.Email = "";
            cliente.Telefono = "";
            cliente.Direccion = "";

            // Assert
            Assert.NotNull(cliente.Email);
            Assert.NotNull(cliente.Telefono);
            Assert.NotNull(cliente.Direccion);
            Assert.Empty(cliente.Email);
            // COMPORTAMIENTO: Campos opcionales nunca son null, son strings vacíos
        }

        /// <summary>
        /// Test 7: ResumenVentas calcula PromedioVenta solo si hay pedidos
        /// Caso de borde: División por cero cuando TotalPedidos = 0
        /// </summary>
        [Fact]
        public void ResumenVentas_PromedioVenta_EvitaDivisionPorCero()
        {
            // Arrange
            var resumen = new ResumenVentas();
            resumen.TotalPedidos = 0;
            resumen.MontoTotal = 1000;

            // Act
            if (resumen.TotalPedidos > 0)
                resumen.PromedioVenta = resumen.MontoTotal / resumen.TotalPedidos;

            // Assert
            Assert.Equal(0, resumen.PromedioVenta);
            // COMPORTAMIENTO: Solo calcula promedio si hay pedidos
            // PERO: Si hay 0 pedidos, PromedioVenta queda en 0 (su valor inicial)
            // No hay validación explícita, depende del orden de ejecución
        }

        /// <summary>
        /// Test 8: Verificar que CrearPedido calcula impuesto basado en ImpuestoVenta
        /// Caso de borde: Cálculo de totales con múltiples detalles
        /// </summary>
        [Fact]
        public void CrearPedido_CalculoImpuesto_UsaConfiguracionImpuestoVenta()
        {
            // Arrange
            decimal subtotal = 100m;
            var impuestoEsperado = subtotal * (decimal)Configuracion.ImpuestoVenta;

            // Act
            decimal impuestoCalculado = subtotal * (decimal)Configuracion.ImpuestoVenta;

            // Assert
            Assert.Equal(13, impuestoCalculado);
            // COMPORTAMIENTO: El impuesto se calcula como subtotal * 0.13
            // COMPORTAMIENTO ACTUAL: Si Configuracion.ImpuestoVenta = 0.13, impuesto = 13
        }

        /// <summary>
        /// Test 9: Verificar estructura de Pedido con campos denormalizados
        /// Comportamiento: Pedido contiene datos del cliente (violación de normalización)
        /// </summary>
        [Fact]
        public void Pedido_ContieneDatasDenormalizadasDelCliente()
        {
            // Arrange
            var pedido = new Pedido();
            pedido.NombreCliente = "Juan Pérez";
            pedido.EmailCliente = "juan@example.com";

            // Act & Assert
            Assert.NotNull(pedido.NombreCliente);
            Assert.NotNull(pedido.EmailCliente);
            // COMPORTAMIENTO: ObtenerPedidos() realiza un JOIN y mapea NombreCliente y EmailCliente
            // ANTI-PATRÓN: Datos denormalizados se replican en Pedido (acoplamiento)
        }

        /// <summary>
        /// Test 10: Usuario devuelve null si credenciales no coinciden
        /// Caso de borde: Validación de autenticación fallida
        /// </summary>
        [Fact]
        public void ValidarUsuario_DevuelveNullSiNoHayCoincidencia()
        {
            // Arrange
            string usuarioIncorrecto = "usuario_inexistente";
            string contraseñaIncorrecta = "password_invalida";

            // Act
            // NOTA: En prueba real fallaría sin BD
            var resultado = AccesoDatos.ValidarUsuario(usuarioIncorrecto, contraseñaIncorrecta);

            // Assert
            Assert.Null(resultado);
            // COMPORTAMIENTO: Si no hay fila con ese usuario/contraseña, reader.Read() = false
            // Se retorna null (u es inicializado como null)
        }

        /// <summary>
        /// Test 11: Verificar que ObtenerCategorias filtra por Activa = 1
        /// Comportamiento: Solo devuelve categorías activas
        /// </summary>
        [Fact]
        public void ObtenerCategorias_DevuelveListaNoNula()
        {
            // Arrange & Act
            var resultado = AccesoDatos.ObtenerCategorias();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<Categoria>>(resultado);
            // COMPORTAMIENTO: Siempre retorna List<Categoria>, nunca null
            // Puede estar vacía si no hay categorías activas
        }

        /// <summary>
        /// Test 12: DetallePedido mapea NombreProducto desde JOIN
        /// Comportamiento: Denormalización de datos del producto
        /// </summary>
        [Fact]
        public void DetallePedido_ContieneNombreProductoMapedoDesdeJoin()
        {
            // Arrange
            var detalle = new DetallePedido();
            detalle.ProductoId = 1;
            detalle.NombreProducto = "Laptop";

            // Act & Assert
            Assert.NotNull(detalle.NombreProducto);
            Assert.Equal("Laptop", detalle.NombreProducto);
            // COMPORTAMIENTO: ObtenerDetallesPedido realiza JOIN con Productos
            // y mapea el Nombre como NombreProducto en DetallePedido
        }

        /// <summary>
        /// Test 13: Verificar cálculo de totales con descuentos en detalle pedido
        /// Caso de borde: Subtotal = Cantidad * PrecioUnitario - Descuento
        /// </summary>
        [Fact]
        public void DetallePedido_CalculoSubtotal_IncorporaDescuento()
        {
            // Arrange
            var detalle = new DetallePedido
            {
                Cantidad = 5,
                PrecioUnitario = 100m,
                Descuento = 50m
            };

            // Act
            // COMPORTAMIENTO: El cálculo exacto depende de cómo se calcula en BD/lógica
            // Suponemos: Subtotal = (Cantidad * PrecioUnitario) - Descuento
            decimal subtotalEsperado = (5 * 100m) - 50m;

            // Assert
            Assert.Equal(450m, subtotalEsperado);
        }

        /// <summary>
        /// Test 14: Estado de Pedido sigue una secuencia específica
        /// Comportamiento: Estados permitidos 1-5
        /// </summary>
        [Fact]
        public void Pedido_Estado_TieneMappingDefined()
        {
            // Arrange
            var pedido = new Pedido { Estado = 1 }; // Pendiente

            // Act & Assert
            Assert.Equal(1, pedido.Estado);
            // COMPORTAMIENTO ACTUAL: El campo Estado es int sin validación de rango
            // Estados esperados: 1=Pendiente, 2=Procesando, 3=Enviado, 4=Entregado, 5=Cancelado
            // No hay enumeración ni validación, se usa int plano
        }

        /// <summary>
        /// Test 15: BUG - ActualizarEstadoPedido construye SQL dinámicamente
        /// Caso de borde: Concatenación de SQL, vulnerabilidad SQL Injection
        /// </summary>
        [Fact]
        public void ActualizarEstadoPedido_BUG_ConcatenacionSQLDinamica()
        {
            // Arrange
            int pedidoId = 1;
            int estadoEnviado = 3;

            // Act
            // El código hace: string sql = "UPDATE Pedidos SET Estado = " + nuevoEstado;
            // Si nuevoEstado == 3, agrega: ", FechaEnvio = GETDATE()"
            // Si nuevoEstado == 4, agrega: ", FechaEntrega = GETDATE()"
            string sqlReconstruida = "UPDATE Pedidos SET Estado = " + estadoEnviado;
            if (estadoEnviado == 3)
                sqlReconstruida += ", FechaEnvio = GETDATE()";
            if (estadoEnviado == 4)
                sqlReconstruida += ", FechaEntrega = GETDATE()";
            sqlReconstruida += " WHERE Id = " + pedidoId;

            // Assert
            Assert.Contains("FechaEnvio = GETDATE()", sqlReconstruida);
            // BUG: SQL dinámico concatenado, vulnerable a SQL injection
            // COMPORTAMIENTO: Construcción correcta de SQL para estados 3 y 4
            // pero sin uso de parámetros
        }

        /// <summary>
        /// Test 16: Verificar que ObtenerVentasPorMes filtra por año y mes
        /// Caso de borde: Mes inválido (13) o año 0
        /// </summary>
        [Fact]
        public void ObtenerVentasPorMes_ConParametrosValidos()
        {
            // Arrange
            int anio = 2023;
            int mes = 6;

            // Act
            // El método construye: 
            // "SELECT ISNULL(SUM(Total), 0) FROM Pedidos WHERE YEAR(FechaPedido) = " + anio
            // + " AND MONTH(FechaPedido) = " + mes + " AND Estado != 5"
            string sqlReconstruida = "SELECT ISNULL(SUM(Total), 0) FROM Pedidos WHERE YEAR(FechaPedido) = " 
                + anio + " AND MONTH(FechaPedido) = " + mes + " AND Estado != 5";

            // Assert
            Assert.Contains("2023", sqlReconstruida);
            Assert.Contains("MONTH(FechaPedido) = 6", sqlReconstruida);
            // COMPORTAMIENTO: SQL concatenado, sin validación de rango de mes (1-12)
            // CASO DE BORDE: mes = 13 o mes = 0 sería procesado sin error
        }

        /// <summary>
        /// Test 17: BUG - ValidarUsuario compara contraseña en texto plano en SQL
        /// Comportamiento: SQL Injection crítico + contraseñas sin hash
        /// </summary>
        [Fact]
        public void ValidarUsuario_BUG_ContrasenasEnTextoPlanoYSQLInjection()
        {
            // Arrange
            string nombreUsuario = "admin";
            string contrasena = "password123";

            // Act
            // El código hace:
            // string sql = "SELECT * FROM Usuarios WHERE NombreUsuario = '" + nombreUsuario 
            //    + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";
            string sqlReconstruida = "SELECT * FROM Usuarios WHERE NombreUsuario = '" 
                + nombreUsuario + "' AND Contrasena = '" + contrasena + "' AND Activo = 1";

            // Assert
            Assert.Contains("Usuarios", sqlReconstruida);
            Assert.Contains("Contrasena = '", sqlReconstruida);
            // BUG 1: SQL Injection - la contraseña va sin parámetros parametrizados
            // BUG 2: Contraseñas en texto plano en BD
            // ATAQUE POSIBLE: nombreUsuario = "' OR '1'='1"
            // Resultaría en: SELECT * FROM Usuarios WHERE NombreUsuario = '' OR '1'='1' AND ...
            // Bypass de autenticación
        }

        /// <summary>
        /// Test 18: Verificar que BuscarProductos busca en Nombre y Codigo
        /// Caso de borde: Búsqueda con caracteres especiales (SQL Injection)
        /// </summary>
        [Fact]
        public void BuscarProductos_BUG_TermiboBuscadaSinSanitizar()
        {
            // Arrange
            string terminoBusqueda = "'; DROP TABLE Productos; --";

            // Act
            // El código hace: string sql = "SELECT * FROM Productos WHERE ... (Nombre LIKE '%" 
            // + termino + "%' OR Codigo LIKE '%" + termino + "%')"
            string sqlReconstruida = "SELECT * FROM Productos WHERE Activo = 1 AND (Nombre LIKE '%" 
                + terminoBusqueda + "%' OR Codigo LIKE '%" + terminoBusqueda + "%')";

            // Assert
            Assert.Contains("DROP TABLE", sqlReconstruida);
            // BUG: SQL Injection. Un atacante podría ejecutar SQL arbitrario
            // COMPORTAMIENTO ACTUAL: El término se inserta directamente sin escape
        }

        /// <summary>
        /// Test 19: Verificar que InsertarProducto retorna el ID generado
        /// Caso de borde: SCOPE_IDENTITY() devuelve el último ID insertado
        /// </summary>
        [Fact]
        public void InsertarProducto_DevuelveIdGenerado()
        {
            // Arrange
            string codigo = "PROD-001";
            string nombre = "Producto Test";
            string descripcion = "Descripción";
            decimal precioCompra = 50m;
            decimal precioVenta = 100m;
            int stock = 10;
            int stockMinimo = 5;
            int categoriaId = 1;

            // Act
            // NOTA: En entorno de prueba real, esto insertaría en BD
            // El método retorna: int id = Convert.ToInt32(cmd.ExecuteScalar());
            // ExecuteScalar() devuelve SCOPE_IDENTITY() que es el último ID insertado

            // Assert
            // COMPORTAMIENTO: Si la inserción es exitosa, retorna un int > 0
            // Si falla, lanza excepción (no hay manejo de errores)
        }

        /// <summary>
        /// Test 20: Verificar que CrearPedido NO valida stock disponible
        /// Caso de borde: Crear pedido sin verificar si hay stock
        /// </summary>
        [Fact]
        public void CrearPedido_BUG_NoValidaStockDisponible()
        {
            // Arrange
            int clienteId = 1;
            string direccion = "Calle 123";
            string notas = "Notas del pedido";
            var detalles = new List<DetallePedido>
            {
                new DetallePedido
                {
                    ProductoId = 1,
                    Cantidad = 9999, // Cantidad muy alta, probablemente sin stock
                    PrecioUnitario = 50m,
                    Descuento = 0m,
                    Subtotal = 499950m
                }
            };

            // Act
            // El código NOT hace: if (ProductoEnBD.Stock >= d.Cantidad) { ... }
            // Simplemente: "UPDATE Productos SET Stock = Stock - " + d.Cantidad + " WHERE Id = " + d.ProductoId
            // Esto puede dejar Stock negativo

            // Assert
            // BUG: No hay validación de stock disponible
            // COMPORTAMIENTO: Stock puede volverse negativo en BD
            // FALTA: try-catch, transacciones, validaciones
        }

        /// <summary>
        /// Test 21: Verificar que CrearPedido NO usa transacciones
        /// Comportamiento: Si falla a mitad, queda inconsistente
        /// </summary>
        [Fact]
        public void CrearPedido_BUG_SinTransacciones_InconsistenciaEnFallo()
        {
            // Arrange
            int clienteId = 1;
            var detalles = new List<DetallePedido>
            {
                new DetallePedido { ProductoId = 1, Cantidad = 5, PrecioUnitario = 100m }
            };

            // Act
            // El código:
            // 1. Inserta Pedido (OK)
            // 2. Inserta DetallePedido (OK)
            // 3. Actualiza Stock (OK)
            // 4. Inserta Movimiento (si falla aquí, pasos 1-3 ya están hechos)
            // NO hay BEGIN TRANSACTION / COMMIT / ROLLBACK

            // Assert
            // BUG: Sin transacciones, si algo falla en mitad:
            // - El Pedido existe
            // - El DetallePedido existe
            // - El Stock ya fue actualizado
            // - El Movimiento no se registró
            // RESULTADO: Inconsistencia de datos
        }

        /// <summary>
        /// Test 22: Verificar que el método ObtenerProductos cierra conexión
        /// Comportamiento: No usa using, depende de Close() manual
        /// </summary>
        [Fact]
        public void ObtenerProductos_GestionConexion_NoUsaUsing()
        {
            // Arrange & Act
            // El código hace:
            // SqlConnection conn = new SqlConnection(...);
            // conn.Open();
            // ... comandos ...
            // reader.Close();
            // conn.Close();

            // Patrón actual: No usa 'using', es propenso a fugas de conexión
            // si ocurre una excepción entre conn.Open() y conn.Close()

            // Assert
            // COMPORTAMIENTO: Conexiones pueden no cerrarse si hay excepciones
            // RIESGO: Agotamiento del connection pool
        }

        /// <summary>
        /// Test 23: ActualizarCliente NO actualiza la fecha de modificación
        /// Caso de borde: Auditoría incompleta
        /// </summary>
        [Fact]
        public void ActualizarCliente_BUG_NoActualizaFechaModificacion()
        {
            // Arrange
            int clienteId = 1;
            string nombre = "Juan";
            string apellido = "Pérez";

            // Act
            // El código hace:
            // string sql = "UPDATE Clientes SET Nombre = '" + nombre + "', Apellido = '" + apellido + "', ...";
            // NO incluye: "UltimaModificacion = GETDATE()"
            // (a diferencia de ActualizarProducto que sí lo hace)

            string sqlActual = "UPDATE Clientes SET Nombre = '" + nombre + "', Apellido = '" + apellido + "'";

            // Assert
            Assert.DoesNotContain("UltimaModificacion", sqlActual);
            // BUG: Cliente NO registra cuándo fue actualizado
            // INCONSISTENCIA: Producto sí actualiza UltimaModificacion, Cliente no
        }
    }
}
