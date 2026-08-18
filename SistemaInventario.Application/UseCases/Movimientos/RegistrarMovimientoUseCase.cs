using AutoMapper;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Domain.Repositories;

namespace SistemaInventario.Application.UseCases.Movimientos
{
    /// <summary>
    /// UseCase: Registrar un movimiento de stock (entrada o salida).
    /// Coordina la actualización de stock en el producto y el registro de movimiento.
    /// </summary>
    public interface IRegistrarMovimientoUseCase
    {
        Task<MovimientoDTO> EjecutarAsync(RegistrarMovimientoDTO request);
    }

    public class RegistrarMovimientoUseCase : IRegistrarMovimientoUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private const int TIPO_ENTRADA = 1;
        private const int TIPO_SALIDA = 2;

        public RegistrarMovimientoUseCase(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MovimientoDTO> EjecutarAsync(RegistrarMovimientoDTO request)
        {
            // 1. Validar que el producto exista
            var producto = await _unitOfWork.Productos.ObtenerPorIdAsync(request.ProductoId);
            if (producto == null)
                throw new ArgumentException($"El producto con ID {request.ProductoId} no existe.");

            // 2. Validar que el tipo de movimiento sea uno de los soportados (1: Entrada, 2: Salida)
            if (request.TipoMovimientoId != TIPO_ENTRADA && request.TipoMovimientoId != TIPO_SALIDA)
                throw new ArgumentException($"El tipo de movimiento {request.TipoMovimientoId} no es válido. Seleccione 1 (Entrada) o 2 (Salida).");

            // 3. Actualizar stock según la regla de negocio del dominio
            try
            {
                if (request.TipoMovimientoId == TIPO_ENTRADA)
                {
                    producto.RecibirStock(request.Cantidad);
                }
                else if (request.TipoMovimientoId == TIPO_SALIDA)
                {
                    producto.DespacharStock(request.Cantidad);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Error al procesar movimiento: {ex.Message}");
            }

            // 4. Crear entidad de movimiento
            var movimiento = Domain.Entities.Movimiento.Crear(
                request.ProductoId,
                request.TipoMovimientoId,
                request.Cantidad,
                request.Usuario,
                request.Observaciones
            );

            // 5. Guardar cambios en repositorio y confirmar transacción
            var movimientoAgregado = await _unitOfWork.Movimientos.AgregarAsync(movimiento);
            await _unitOfWork.Productos.ActualizarAsync(producto);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MovimientoDTO>(movimientoAgregado);
        }
    }
}