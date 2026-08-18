using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Infrastructure.Data;
using SistemaInventario.Domain.Repositories;
using SistemaInventario.Infrastructure.Repositories;

namespace SistemaInventario.Infrastructure
{
    /// <summary>
    /// Extensión para inyección de dependencias de la capa Infrastructure.
    /// Centraliza la configuración de servicios y conexiones a BD.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {
            // Registrar DbContext
            services.AddDbContext<InventarioDbContext>(options =>
                options.UseSqlServer(connectionString)
                       .EnableSensitiveDataLogging()); // Solo para desarrollo

            // Registrar Unit of Work y repositorios
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IMovimientoRepository, MovimientoRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            return services;
        }

        /// <summary>
        /// Migrar BD automáticamente al iniciar (solo para desarrollo).
        /// </summary>
       public static async Task<IServiceProvider> MigrateAndSeedAsync(this IServiceProvider services)
        {
        using (var scope = services.CreateScope())
        {
        var context = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();
        
        // EnsureCreatedAsync garantiza que las tablas definidas en los DbContext/Entities 
        // existan físicamente en SQL Server antes de ejecutar el seeding.
        await context.Database.EnsureCreatedAsync();
        
        // Seed datos de referencia
        var seeder = new Persistence.DataSeeder(context);
        await seeder.SeedAsync();

        return services;
        }
        }
    }
}
