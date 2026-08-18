namespace SistemaInventario.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Producto.
    /// Encapsula la lógica de negocio relacionada con productos.
    /// No es un modelo anémico: contiene comportamiento y validaciones.
    /// </summary>
    public class Producto
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public int CategoriaId { get; private set; }
        public decimal Precio { get; private set; }
        public int Stock { get; private set; }
        public int StockMinimo { get; private set; }
        public bool Activo { get; private set; }
        public DateTime CreadoEn { get; private set; }
        public DateTime ActualizadoEn { get; private set; }

        // Relación con Categoría (lazy loading si es necesario)
        public Categoria Categoria { get; private set; }

        // Constructor privado para Entity Framework
        private Producto() { }

        // Factory method para crear un Producto válido
        public static Producto Crear(string nombre, int categoriaId, decimal precio, int stock, int stockMinimo)
        {
            // Validaciones de dominio (pueden lanzar DomainException)
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del producto es obligatorio.", nameof(nombre));

            if (precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero.", nameof(precio));

            if (stock < 0)
                throw new ArgumentException("El stock no puede ser negativo.", nameof(stock));

            if (stockMinimo < 0)
                throw new ArgumentException("El stock mínimo no puede ser negativo.", nameof(stockMinimo));

            if (categoriaId <= 0)
                throw new ArgumentException("Categoría inválida.", nameof(categoriaId));

            return new Producto
            {
                Nombre = nombre.Trim(),
                CategoriaId = categoriaId,
                Precio = precio,
                Stock = stock,
                StockMinimo = stockMinimo,
                Activo = true,
                CreadoEn = DateTime.UtcNow,
                ActualizadoEn = DateTime.UtcNow
            };
        }

        // Métodos de negocio
        public void ActualizarDatos(string nombre, decimal precio, int stockMinimo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del producto es obligatorio.", nameof(nombre));

            if (precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero.", nameof(precio));

            if (stockMinimo < 0)
                throw new ArgumentException("El stock mínimo no puede ser negativo.", nameof(stockMinimo));

            Nombre = nombre.Trim();
            Precio = precio;
            StockMinimo = stockMinimo;
            ActualizadoEn = DateTime.UtcNow;
        }

        public void RecibirStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));

            Stock += cantidad;
            ActualizadoEn = DateTime.UtcNow;
        }

        public void DespacharStock(int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));

            if (Stock < cantidad)
                throw new InvalidOperationException($"Stock insuficiente. Disponible: {Stock}, Requerido: {cantidad}");

            Stock -= cantidad;
            ActualizadoEn = DateTime.UtcNow;
        }

        public bool EsBajoStock() => Stock <= StockMinimo;

        public void Desactivar()
        {
            Activo = false;
            ActualizadoEn = DateTime.UtcNow;
        }

        public void Activar()
        {
            Activo = true;
            ActualizadoEn = DateTime.UtcNow;
        }
    }
}
