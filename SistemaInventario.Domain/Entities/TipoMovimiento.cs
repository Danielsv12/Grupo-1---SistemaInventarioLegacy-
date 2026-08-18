namespace SistemaInventario.Domain.Entities
{
    /// <summary>
    /// Entidad de referencia: TipoMovimiento.
    /// Define los tipos de movimientos permitidos (Entrada / Salida).
    /// </summary>
    public class TipoMovimiento
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; } // "Entrada" / "Salida"
        public string Descripcion { get; private set; }

        // Relación inversa
        public ICollection<Movimiento> Movimientos { get; private set; } = new List<Movimiento>();

        private TipoMovimiento() { }

        public static TipoMovimiento Crear(string nombre, string descripcion = "")
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del tipo de movimiento es obligatorio.", nameof(nombre));

            return new TipoMovimiento
            {
                Nombre = nombre.Trim(),
                Descripcion = descripcion?.Trim() ?? ""
            };
        }
    }
}
