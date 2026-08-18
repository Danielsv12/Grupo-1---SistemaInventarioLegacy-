using Microsoft.EntityFrameworkCore;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Repositories;
using SistemaInventario.Infrastructure.Data;

namespace SistemaInventario.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Producto.
    /// Encapsula la lógica de acceso a datos usando Entity Framework Core.
    /// No hay SQL concatenado ni inyección SQL: solo queries parametrizadas.
    /// </summary>
    public class ProductoRepository : IProductoRepository
    {
        private readonly InventarioDbContext _context;

        public ProductoRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<Producto> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Producto> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Nombre == nombre);
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> ObtenerBajoStockAsync()
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo && p.Stock <= p.StockMinimo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> BuscarPorNombreAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return new List<Producto>();

            var terminoBusqueda = termino.ToLower();
            return await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo && p.Nombre.ToLower().Contains(terminoBusqueda))
                .ToListAsync();
        }

        public async Task<Producto> AgregarAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto> ActualizarAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var producto = await ObtenerPorIdAsync(id);
            if (producto == null)
                return false;

            producto.Desactivar();
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Productos.AnyAsync(p => p.Id == id);
        }
    }

    /// <summary>
    /// Implementación del repositorio de Movimiento.
    /// </summary>
    public class MovimientoRepository : IMovimientoRepository
    {
        private readonly InventarioDbContext _context;

        public MovimientoRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<Movimiento> ObtenerPorIdAsync(int id)
        {
            return await _context.Movimientos
                .Include(m => m.Producto)
                .Include(m => m.TipoMovimiento)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Movimiento>> ObtenerPorProductoAsync(int productoId)
        {
            return await _context.Movimientos
                .Include(m => m.Producto)
                .Include(m => m.TipoMovimiento)
                .Where(m => m.ProductoId == productoId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimiento>> ObtenerPorFechaAsync(DateTime desde, DateTime hasta)
        {
            return await _context.Movimientos
                .Include(m => m.Producto)
                .Include(m => m.TipoMovimiento)
                .Where(m => m.Fecha >= desde && m.Fecha <= hasta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<Movimiento> AgregarAsync(Movimiento movimiento)
        {
            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();
            return movimiento;
        }

        public async Task<IEnumerable<Movimiento>> ObtenerTodosAsync()
        {
            return await _context.Movimientos
                .Include(m => m.Producto)
                .Include(m => m.TipoMovimiento)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }
    }

    /// <summary>
    /// Implementación del repositorio de Categoría.
    /// </summary>
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly InventarioDbContext _context;

        public CategoriaRepository(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<Categoria> ObtenerPorIdAsync(int id)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Categoria> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.Nombre == nombre);
        }

        public async Task<IEnumerable<Categoria>> ObtenerTodosAsync()
        {
            return await _context.Categorias.Where(c => c.Activo).ToListAsync();
        }

        public async Task<Categoria> AgregarAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<Categoria> ActualizarAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Categorias.AnyAsync(c => c.Id == id);
        }
    }
}
