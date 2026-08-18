using Microsoft.EntityFrameworkCore.Storage;
using SistemaInventario.Domain.Repositories;
using SistemaInventario.Infrastructure.Data;

namespace SistemaInventario.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del Unit of Work.
    /// Coordina transacciones y acceso a múltiples repositorios.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InventarioDbContext _context;
        private IProductoRepository _productoRepository;
        private IMovimientoRepository _movimientoRepository;
        private ICategoriaRepository _categoriaRepository;
        private IDbContextTransaction _transaction;

        public UnitOfWork(InventarioDbContext context)
        {
            _context = context;
        }

        public IProductoRepository Productos
            => _productoRepository ??= new ProductoRepository(_context);

        public IMovimientoRepository Movimientos
            => _movimientoRepository ??= new MovimientoRepository(_context);

        public ICategoriaRepository Categorias
            => _categoriaRepository ??= new CategoriaRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction?.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                await _transaction?.RollbackAsync();
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
