using AutoMapper;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Repositories;

namespace SistemaInventario.Application.UseCases.Productos
{
    /// <summary>
    /// UseCase: Registrar un nuevo producto.
    /// Encapsula la lógica de aplicación para crear productos.
    /// Independiente de la interfaz (consola, API, etc).
    /// </summary>
    public interface IRegistrarProductoUseCase
    {
        Task<ProductoDTO> EjecutarAsync(CrearProductoDTO request);
    }

    public class RegistrarProductoUseCase : IRegistrarProductoUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RegistrarProductoUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductoDTO> EjecutarAsync(CrearProductoDTO request)
        {
            // Validar que la categoría exista
            var categoriaExiste = await _unitOfWork.Categorias.ExisteAsync(request.CategoriaId);
            if (!categoriaExiste)
                throw new ArgumentException($"La categoría con ID {request.CategoriaId} no existe.");

            // Validar que el nombre sea único
            var productoExistente = await _unitOfWork.Productos.ObtenerPorNombreAsync(request.Nombre);
            if (productoExistente != null)
                throw new ArgumentException($"Ya existe un producto con el nombre '{request.Nombre}'.");

            // Crear la entidad (validación de dominio)
            var producto = Producto.Crear(
                request.Nombre,
                request.CategoriaId,
                request.Precio,
                request.Stock,
                request.StockMinimo
            );

            // Persistir
            var productoAgregado = await _unitOfWork.Productos.AgregarAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            // Retornar DTO (recargar para tener la categoría)
            var productoConCategoria = await _unitOfWork.Productos.ObtenerPorIdAsync(productoAgregado.Id);
            
            return _mapper.Map<ProductoDTO>(productoConCategoria);
        }
    }
}
