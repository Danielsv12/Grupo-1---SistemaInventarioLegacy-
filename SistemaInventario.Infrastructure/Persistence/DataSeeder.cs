using SistemaInventario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaInventario.Infrastructure.Persistence
{
    /// <summary>
    /// DataSeeder: Inicialización de datos de referencia (categorías, tipos de movimiento).
    /// Se ejecuta después de las migraciones, para evitar problemas con factory methods.
    /// </summary>
    public class DataSeeder
    {
        private readonly InventarioDbContext _context;

        public DataSeeder(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Esperar a que las migraciones se apliquen
                await _context.Database.MigrateAsync();

                // Seed Tipos de Movimiento
                if (!await _context.TiposMovimiento.AnyAsync())
                {
                    _context.TiposMovimiento.AddRange(
                        Domain.Entities.TipoMovimiento.Crear("Entrada", "Entrada de stock"),
                        Domain.Entities.TipoMovimiento.Crear("Salida", "Salida de stock")
                    );
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✓ Tipos de movimiento seeded");
                }

                // Seed Categorías
                if (!await _context.Categorias.AnyAsync())
                {
                    _context.Categorias.AddRange(
                        Domain.Entities.Categoria.Crear("Electrónica", "Productos electrónicos"),
                        Domain.Entities.Categoria.Crear("Ropa", "Prendas de vestir"),
                        Domain.Entities.Categoria.Crear("Alimentos", "Productos alimenticios")
                    );
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✓ Categorías seeded");
                }

                Console.WriteLine("✓ Database ready!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error durante seeding: {ex.Message}");
                throw;
            }
        }
    }
}
