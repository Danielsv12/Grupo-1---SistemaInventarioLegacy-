using AutoMapper;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Repositories;

namespace SistemaInventario.Application.UseCases.Productos
{
    /// <summary>
    /// UseCase: Buscar productos por nombre.
    /// </summary>
    public interface IBuscarProductoUseCase
    {
        Task<IEnumerable<ProductoDTO>> EjecutarAsync(string termino);
    }

    public class BuscarProductoUseCase : IBuscarProductoUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BuscarProductoUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductoDTO>> EjecutarAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return new List<ProductoDTO>();

            var productos = await _unitOfWork.Productos.BuscarPorNombreAsync(termino);
            return _mapper.Map<IEnumerable<ProductoDTO>>(productos);
        }
    }
}
