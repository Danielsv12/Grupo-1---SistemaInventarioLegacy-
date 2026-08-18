namespace SistemaInventario.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Movimiento.
    /// Registra todas las entradas y salidas de stock.
    /// </summary>
    public class Movimiento
    {
        public int Id { get; private set; }
        public int ProductoId { get; private set; }
        public int TipoMovimientoId { get; private set; }
        public int Cantidad { get; private set; }
        public DateTime Fecha { get; private set; }
        public string Usuario { get; private set; }
        public string Observaciones { get; private set; }
        public DateTime CreadoEn { get; private set; }

        // Relaciones
        public Producto Producto { get; private set; }
        public TipoMovimiento TipoMovimiento { get; private set; }

        // Constructor privado para Entity Framework
        private Movimiento() { }

        // Factory method
        public static Movimiento Crear(int productoId, int tipoMovimientoId, int cantidad, string usuario, string observaciones = "")
        {
            if (productoId <= 0)
                throw new ArgumentException("ProductoId inválido.", nameof(productoId));

            if (tipoMovimientoId <= 0)
                throw new ArgumentException("TipoMovimientoId inválido.", nameof(tipoMovimientoId));

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));

            if (string.IsNullOrWhiteSpace(usuario))
                throw new ArgumentException("El usuario es obligatorio.", nameof(usuario));

            return new Movimiento
            {
                ProductoId = productoId,
                TipoMovimientoId = tipoMovimientoId,
                Cantidad = cantidad,
                Usuario = usuario.Trim(),
                Observaciones = observaciones?.Trim() ?? "",
                Fecha = DateTime.UtcNow,
                CreadoEn = DateTime.UtcNow
            };
        }
    }
}
