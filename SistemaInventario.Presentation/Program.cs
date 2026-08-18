using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaInventario.Application;
using SistemaInventario.Infrastructure;
using SistemaInventario.Presentation.ConsoleUI;

// Construcción de la configuración
var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // Opcional
    .AddEnvironmentVariables();  // Prioritario

var configuration = configBuilder.Build();

// Construcción del contenedor de dependencias
var services = new ServiceCollection();

services.AddSingleton(configuration);
services.AddApplication();

// Connection string: desde env vars > appsettings.json > default
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection") 
    ?? configuration.GetConnectionString("DefaultConnection")
    ?? "Server=sqlserver;Database=InventarioDb;User Id=sa;Password=YourPassword123!;Encrypt=false;";

Console.WriteLine($"[INFO] Conectando a: {(connectionString.Contains("sqlserver") ? "Docker SQL Server" : "SQL Server local")}");

services.AddInfrastructure(connectionString);

var serviceProvider = services.BuildServiceProvider();

// Migrar BD
await serviceProvider.MigrateAndSeedAsync();

// Ejecutar aplicación de consola
var app = new MenuPrincipal(serviceProvider);
await app.EjecutarAsync();
