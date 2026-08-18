namespace SistemaInventario.Domain.Entities
{
    /// <summary>
    /// Entidad de dominio: Categoría.
    /// Normalización de la tabla de categorías.
    /// </summary>
    public class Categoria
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public string Descripcion { get; private set; }
        public bool Activo { get; private set; }
        public DateTime CreadoEn { get; private set; }

        // Relación inversa con Productos
        public ICollection<Producto> Productos { get; private set; } = new List<Producto>();

        // Constructor privado para Entity Framework
        private Categoria() { }

        // Factory method
        public static Categoria Crear(string nombre, string descripcion = "")
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombre));

            return new Categoria
            {
                Nombre = nombre.Trim(),
                Descripcion = descripcion?.Trim() ?? "",
                Activo = true,
                CreadoEn = DateTime.UtcNow
            };
        }

        public void Desactivar() => Activo = false;
        public void Activar() => Activo = true;
    }
}
