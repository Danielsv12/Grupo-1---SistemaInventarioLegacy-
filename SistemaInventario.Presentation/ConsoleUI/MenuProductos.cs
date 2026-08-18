using Microsoft.Extensions.DependencyInjection;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Application.UseCases.Productos;

namespace SistemaInventario.Presentation.ConsoleUI
{
    /// <summary>
    /// Menú para gestión de productos.
    /// Delega toda lógica de negocio a los UseCases.
    /// </summary>
    public class MenuProductos
    {
        private readonly IServiceProvider _serviceProvider;

        public MenuProductos(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task EjecutarAsync()
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║         Gestión de Productos          ║");
                Console.WriteLine("╚════════════════════════════════════════╝\n");
                Console.WriteLine("1. Registrar Producto");
                Console.WriteLine("2. Buscar Producto");
                Console.WriteLine("3. Volver\n");
                Console.Write("Seleccione una opción: ");

                if (int.TryParse(Console.ReadLine(), out int opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            await RegistrarProducto();
                            break;
                        case 2:
                            await BuscarProducto();
                            break;
                        case 3:
                            salir = true;
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Presione cualquier tecla...");
                            Console.ReadKey();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Entrada inválida. Presione cualquier tecla...");
                    Console.ReadKey();
                }
            }
        }

        private async Task RegistrarProducto()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║       Registrar Nuevo Producto        ║");
                Console.WriteLine("╚════════════════════════════════════════╝\n");

                Console.Write("Nombre del producto: ");
                string nombre = Console.ReadLine();

                Console.Write("ID Categoría (1=Electrónica, 2=Ropa, 3=Alimentos): ");
                if (!int.TryParse(Console.ReadLine(), out int categoriaId))
                {
                    Console.WriteLine("Categoría inválida.");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Precio: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal precio))
                {
                    Console.WriteLine("Precio inválido.");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Stock inicial: ");
                if (!int.TryParse(Console.ReadLine(), out int stock))
                {
                    Console.WriteLine("Stock inválido.");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Stock mínimo: ");
                if (!int.TryParse(Console.ReadLine(), out int stockMin))
                {
                    Console.WriteLine("Stock mínimo inválido.");
                    Console.ReadKey();
                    return;
                }

                var useCase = _serviceProvider.GetRequiredService<IRegistrarProductoUseCase>();
                var request = new CrearProductoDTO
                {
                    Nombre = nombre,
                    CategoriaId = categoriaId,
                    Precio = precio,
                    Stock = stock,
                    StockMinimo = stockMin
                };

                var resultado = await useCase.EjecutarAsync(request);
                Console.WriteLine($"\n✓ Producto registrado exitosamente. ID: {resultado.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }

            Console.ReadKey();
        }

        private async Task BuscarProducto()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║         Buscar Producto               ║");
                Console.WriteLine("╚════════════════════════════════════════╝\n");

                Console.Write("Ingrese término de búsqueda: ");
                string termino = Console.ReadLine();

                var useCase = _serviceProvider.GetRequiredService<IBuscarProductoUseCase>();
                var resultados = await useCase.EjecutarAsync(termino);

                if (!resultados.Any())
                {
                    Console.WriteLine("\nNo se encontraron productos.");
                }
                else
                {
                    Console.WriteLine("\nResultados:\n");
                    foreach (var producto in resultados)
                    {
                        Console.WriteLine($"ID: {producto.Id} | Nombre: {producto.Nombre}");
                        Console.WriteLine($"  Categoría: {producto.CategoriaNombre} | Precio: ${producto.Precio}");
                        Console.WriteLine($"  Stock: {producto.Stock} | Mínimo: {producto.StockMinimo}");
                        Console.WriteLine($"  Bajo Stock: {(producto.BajoStock ? "SÍ" : "NO")}\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}
