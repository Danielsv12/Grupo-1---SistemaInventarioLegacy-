using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Infrastructure.Data
{
    /// <summary>
    /// DbContext de Entity Framework Core.
    /// Centraliza la configuración del modelo de datos y acceso a datos.
    /// </summary>
    public class InventarioDbContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<TipoMovimiento> TiposMovimiento { get; set; }

        public InventarioDbContext(DbContextOptions<InventarioDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar Categoría
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.CreadoEn).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configurar Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Nombre).IsUnique();
                entity.Property(e => e.Precio).HasPrecision(10, 2);
                entity.Property(e => e.Stock).HasDefaultValue(0);
                entity.Property(e => e.StockMinimo).HasDefaultValue(0);
                entity.Property(e => e.CreadoEn).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ActualizadoEn).HasDefaultValueSql("GETUTCDATE()");
                
                // Relación con Categoría
                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Productos)
                    .HasForeignKey(e => e.CategoriaId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Índices
                entity.HasIndex(e => e.Stock);
                entity.HasIndex(e => new { e.Stock, e.StockMinimo });
            });

            // Configurar TipoMovimiento
            modelBuilder.Entity<TipoMovimiento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configurar Movimiento
            modelBuilder.Entity<Movimiento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.Usuario).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                entity.Property(e => e.CreadoEn).HasDefaultValueSql("GETUTCDATE()");

                // Relaciones
                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TipoMovimiento)
                    .WithMany(t => t.Movimientos)
                    .HasForeignKey(e => e.TipoMovimientoId);

                // Índices
                entity.HasIndex(e => e.ProductoId);
                entity.HasIndex(e => e.Fecha);
                entity.HasIndex(e => new { e.ProductoId, e.Fecha });
            });
        }
    }
}
