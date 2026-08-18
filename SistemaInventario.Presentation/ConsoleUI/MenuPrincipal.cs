using SistemaInventario.Application.UseCases.Productos;

namespace SistemaInventario.Presentation.ConsoleUI
{
    /// <summary>
    /// Menú principal de la aplicación de consola.
    /// Orquesta los menús específicos y no contiene lógica de negocio.
    /// Clean Architecture: la presentación es lo más delgado posible.
    /// </summary>
    public class MenuPrincipal
    {
        private readonly IServiceProvider _serviceProvider;
        private MenuProductos _menuProductos;
        private MenuMovimientos _menuMovimientos;

        public MenuPrincipal(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _menuProductos = new MenuProductos(serviceProvider);
            _menuMovimientos = new MenuMovimientos(serviceProvider);
        }

        public async Task EjecutarAsync()
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("╔════════════════════════════════════════╗");
                Console.WriteLine("║   Sistema de Gestión de Inventario    ║");
                Console.WriteLine("║        PYME - Clean Architecture       ║");
                Console.WriteLine("╚════════════════════════════════════════╝\n");
                Console.WriteLine("1. Gestionar Productos");
                Console.WriteLine("2. Registrar Movimientos");
                Console.WriteLine("3. Salir\n");
                Console.Write("Seleccione una opción: ");

                if (int.TryParse(Console.ReadLine(), out int opcion))
                {
                    switch (opcion)
                    {
                        case 1:
                            await _menuProductos.EjecutarAsync();
                            break;
                        case 2:
                            await _menuMovimientos.EjecutarAsync();
                            break;
                        case 3:
                            salir = true;
                            Console.WriteLine("\n¡Hasta luego!");
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
    }
}
