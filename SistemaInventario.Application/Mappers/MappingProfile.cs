using AutoMapper;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;

namespace SistemaInventario.Application.Mappers
{
    /// <summary>
    /// Perfil de AutoMapper: mapeo automático entre Entidades y DTOs.
    /// Implementa la transformación de datos entre capas sin acoplamiento.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Producto → ProductoDTO
            CreateMap<Producto, ProductoDTO>()
                .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria.Nombre))
                .ForMember(dest => dest.BajoStock, opt => opt.MapFrom(src => src.EsBajoStock()));

            // CrearProductoDTO → Producto (mediante factory method)
            CreateMap<CrearProductoDTO, Producto>()
                .ConvertUsing(src => Producto.Crear(src.Nombre, src.CategoriaId, src.Precio, src.Stock, src.StockMinimo));

            // Movimiento → MovimientoDTO
            CreateMap<Movimiento, MovimientoDTO>()
                .ForMember(dest => dest.ProductoNombre, opt => opt.MapFrom(src => src.Producto.Nombre))
                .ForMember(dest => dest.TipoMovimiento, opt => opt.MapFrom(src => src.TipoMovimiento.Nombre));

            // RegistrarMovimientoDTO → Movimiento
            CreateMap<RegistrarMovimientoDTO, Movimiento>()
                .ConvertUsing(src => Movimiento.Crear(src.ProductoId, src.TipoMovimientoId, src.Cantidad, src.Usuario, src.Observaciones));

            // Categoría → CategoriaDTO
            CreateMap<Categoria, CategoriaDTO>();
        }
    }
}
