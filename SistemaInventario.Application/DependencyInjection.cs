using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using SistemaInventario.Application.Mappers;
using SistemaInventario.Application.UseCases.Productos;
using SistemaInventario.Application.UseCases.Movimientos;

namespace SistemaInventario.Application
{
    /// <summary>
    /// Extensión para inyección de dependencias de la capa Application.
    /// Registra todos los UseCases y mappers.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registrar AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));

            // Registrar UseCases
            services.AddScoped<IRegistrarProductoUseCase, RegistrarProductoUseCase>();
            services.AddScoped<IBuscarProductoUseCase, BuscarProductoUseCase>();
            services.AddScoped<IRegistrarMovimientoUseCase, RegistrarMovimientoUseCase>();

            return services;
        }
    }
}
