namespace SistemaInventario.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object para Producto.
    /// Desacoplado de la entidad de dominio, seguro para transferencia entre capas.
    /// </summary>
    public class ProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public bool Activo { get; set; }
        public bool BajoStock { get; set; }
        public DateTime CreadoEn { get; set; }
        public DateTime ActualizadoEn { get; set; }
    }

    public class CrearProductoDTO
    {
        public string Nombre { get; set; }
        public int CategoriaId { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
    }

    public class ActualizarProductoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int StockMinimo { get; set; }
    }

    /// <summary>
    /// DTO para Movimiento.
    /// </summary>
    public class MovimientoDTO
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int TipoMovimientoId { get; set; }
        public string TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public string Observaciones { get; set; }
    }

    public class RegistrarMovimientoDTO
    {
        public int ProductoId { get; set; }
        public int TipoMovimientoId { get; set; }
        public int Cantidad { get; set; }
        public string Usuario { get; set; }
        public string Observaciones { get; set; }
    }

    /// <summary>
    /// DTO para Categoría.
    /// </summary>
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
