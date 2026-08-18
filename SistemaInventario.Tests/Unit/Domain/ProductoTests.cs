using Xunit;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Tests.Unit.Domain
{
    /// <summary>
    /// Pruebas unitarias para la entidad Producto.
    /// Valida la lógica de dominio sin dependencias externas.
    /// </summary>
    public class ProductoTests
    {
        [Fact]
        public void Crear_ConDatosValidos_DebeCrearProductoExitosamente()
        {
            // Arrange
            string nombre = "Laptop";
            int categoriaId = 1;
            decimal precio = 999.99m;
            int stock = 10;
            int stockMinimo = 2;

            // Act
            var producto = Producto.Crear(nombre, categoriaId, precio, stock, stockMinimo);

            // Assert
            Assert.NotNull(producto);
            Assert.Equal(nombre, producto.Nombre);
            Assert.Equal(precio, producto.Precio);
            Assert.Equal(stock, producto.Stock);
            Assert.Equal(stockMinimo, producto.StockMinimo);
            Assert.True(producto.Activo);
        }

        [Fact]
        public void Crear_ConNombreVacio_DebeLanzarExcepcion()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Producto.Crear("", 1, 100, 10, 2)
            );
        }

        [Fact]
        public void Crear_ConPrecioNegativo_DebeLanzarExcepcion()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Producto.Crear("Laptop", 1, -100, 10, 2)
            );
        }

        [Fact]
        public void RecibirStock_DebeAumentarElStock()
        {
            // Arrange
            var producto = Producto.Crear("Laptop", 1, 999.99m, 10, 2);
            int cantidadRecibida = 5;

            // Act
            producto.RecibirStock(cantidadRecibida);

            // Assert
            Assert.Equal(15, producto.Stock);
        }

        [Fact]
        public void DespacharStock_ConStockSuficiente_DebeDisminuirElStock()
        {
            // Arrange
            var producto = Producto.Crear("Laptop", 1, 999.99m, 10, 2);
            int cantidadDespachada = 3;

            // Act
            producto.DespacharStock(cantidadDespachada);

            // Assert
            Assert.Equal(7, producto.Stock);
        }

        [Fact]
        public void DespacharStock_ConStockInsuficiente_DebeLanzarExcepcion()
        {
            // Arrange
            var producto = Producto.Crear("Laptop", 1, 999.99m, 10, 2);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                producto.DespacharStock(20)
            );
        }

        [Fact]
        public void EsBajoStock_CuandoStockMenorAlMinimo_DebeRetornarTrue()
        {
            // Arrange
            var producto = Producto.Crear("Laptop", 1, 999.99m, 1, 5);

            // Act
            var resultado = producto.EsBajoStock();

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void EsBajoStock_CuandoStockMayorAlMinimo_DebeRetornarFalse()
        {
            // Arrange
            var producto = Producto.Crear("Laptop", 1, 999.99m, 10, 2);

            // Act
            var resultado = producto.EsBajoStock();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void Desactivar_DebeSetearActivoEnFalso()
        {
            // Arrange
            var producto = Producto.Crear("Laptop", 1, 999.99m, 10, 2);

            // Act
            producto.Desactivar();

            // Assert
            Assert.False(producto.Activo);
        }
    }
}
