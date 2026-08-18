using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Domain.Repositories
{
    /// <summary>
    /// Interfaz de repositorio para Producto.
    /// Define el contrato entre la aplicación y la persistencia.
    /// Inversión de dependencias: la aplicación depende de abstracciones, no de implementaciones.
    /// </summary>
    public interface IProductoRepository
    {
        Task<Producto> ObtenerPorIdAsync(int id);
        Task<Producto> ObtenerPorNombreAsync(string nombre);
        Task<IEnumerable<Producto>> ObtenerTodosAsync();
        Task<IEnumerable<Producto>> ObtenerBajoStockAsync();
        Task<IEnumerable<Producto>> BuscarPorNombreAsync(string termino);
        Task<Producto> AgregarAsync(Producto producto);
        Task<Producto> ActualizarAsync(Producto producto);
        Task<bool> EliminarAsync(int id);
        Task<bool> ExisteAsync(int id);
    }

    /// <summary>
    /// Interfaz de repositorio para Movimiento.
    /// </summary>
    public interface IMovimientoRepository
    {
        Task<Movimiento> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Movimiento>> ObtenerPorProductoAsync(int productoId);
        Task<IEnumerable<Movimiento>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta);
        Task<Movimiento> AgregarAsync(Movimiento movimiento);
        Task<IEnumerable<Movimiento>> ObtenerTodosAsync();
    }

    /// <summary>
    /// Interfaz de repositorio para Categoría.
    /// </summary>
    public interface ICategoriaRepository
    {
        Task<Categoria> ObtenerPorIdAsync(int id);
        Task<Categoria> ObtenerPorNombreAsync(string nombre);
        Task<IEnumerable<Categoria>> ObtenerTodosAsync();
        Task<Categoria> AgregarAsync(Categoria categoria);
        Task<Categoria> ActualizarAsync(Categoria categoria);
        Task<bool> ExisteAsync(int id);
    }

    /// <summary>
    /// Interfaz Unit of Work para coordinar transacciones.
    /// </summary>
    public interface IUnitOfWork
    {
        IProductoRepository Productos { get; }
        IMovimientoRepository Movimientos { get; }
        ICategoriaRepository Categorias { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
