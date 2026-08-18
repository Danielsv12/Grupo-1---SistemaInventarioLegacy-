using Microsoft.Extensions.DependencyInjection;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Application.UseCases.Movimientos;

namespace SistemaInventario.Presentation.ConsoleUI
{
    /// <summary>
    /// Menú para gestión de movimientos (entradas/salidas).
    /// </summary>
    public class MenuMovimientos
    {
        private readonly IServiceProvider _serviceProvider;

        public MenuMovimientos(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task EjecutarAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║    Registrar Movimiento de Stock      ║");
                Console.WriteLine("╚════════════════════════════════════════╝\n");

                Console.Write("ID del Producto: ");
                if (!int.TryParse(Console.ReadLine(), out int productoId))
                {
                    Console.WriteLine("ID inválido.");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("\nTipo de Movimiento:");
                Console.WriteLine("1. Entrada (Recepción)");
                Console.WriteLine("2. Salida (Despacho)");
                Console.Write("Seleccione: ");

                if (!int.TryParse(Console.ReadLine(), out int tipoMov))
                {
                    Console.WriteLine("Tipo inválido.");
                    Console.ReadKey();
                    return;
                }

                Console.Write("\nCantidad: ");
                if (!int.TryParse(Console.ReadLine(), out int cantidad))
                {
                    Console.WriteLine("Cantidad inválida.");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Usuario: ");
                string usuario = Console.ReadLine();

                Console.Write("Observaciones (opcional): ");
                string observaciones = Console.ReadLine();

                var useCase = _serviceProvider.GetRequiredService<IRegistrarMovimientoUseCase>();
                var request = new RegistrarMovimientoDTO
                {
                    ProductoId = productoId,
                    TipoMovimientoId = tipoMov,
                    Cantidad = cantidad,
                    Usuario = usuario,
                    Observaciones = observaciones
                };

                var resultado = await useCase.EjecutarAsync(request);
                Console.WriteLine($"\n✓ Movimiento registrado exitosamente. ID: {resultado.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
