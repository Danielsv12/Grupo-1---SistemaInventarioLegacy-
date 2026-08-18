namespace SistemaInventario.Domain.Exceptions
{
    /// <summary>
    /// Excepción base para errores de dominio.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ProductoNoEncontradoException : DomainException
    {
        public ProductoNoEncontradoException(int productoId)
            : base($"El producto con ID {productoId} no fue encontrado.") { }
    }

    public class StockInsuficienteException : DomainException
    {
        public StockInsuficienteException(int productoId, int disponible, int requerido)
            : base($"Stock insuficiente para el producto {productoId}. Disponible: {disponible}, Requerido: {requerido}") { }
    }

    public class CategoriaNoEncontradaException : DomainException
    {
        public CategoriaNoEncontradaException(int categoriaId)
            : base($"La categoría con ID {categoriaId} no fue encontrada.") { }
    }
}
