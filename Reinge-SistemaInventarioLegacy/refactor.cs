using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

// ══════════════════════════════════════════════════════════════════════════════
//  REFACTORING: GestorVentas
//
//  Code smells corregidos (catálogo Fowler):
//    1. God Class          → SRP: tres clases con una sola responsabilidad
//    2. Long Method        → Extract Method: ValidarPedido / CalcularTotales
//    3. Primitive Obsession → Value Objects: DireccionCliente, ItemVenta
//    4. Data Clumps        → Parameter Object: PedidoVenta
//    5. Feature Envy       → Strategy Pattern: ICalculadorDescuento
//    6. Magic Strings      → Enum TipoCliente
//    7. Magic Numbers      → Constante TasaIVA
//    8. Switch Statements  → Factory Method + polimorfismo
//
//  Principios SOLID aplicados:
//    SRP – GestorVentas solo orquesta; cada clase tiene una razón para cambiar
//    OCP – Agregar un tipo de cliente nuevo NO modifica GestorVentas
//    DIP – GestorVentas depende de abstracciones (interfaces), no de SMTP/File
//    ISP – Interfaces pequeñas y cohesivas
// ══════════════════════════════════════════════════════════════════════════════

namespace Reinge_SistemaInventarioLegacy
{
    // ── Enum: elimina Magic Strings ───────────────────────────────────────────

    public enum TipoCliente { Regular, Mayorista, VIP }

    // ── Value Objects ─────────────────────────────────────────────────────────

    /// <summary>
    /// Agrupa los tres campos de dirección que siempre viajan juntos (Data Clumps).
    /// Inmutable por diseño: una dirección no cambia una vez creada en el pedido.
    /// </summary>
    public sealed class DireccionCliente
    {
        public string Calle        { get; }
        public string Ciudad       { get; }
        public string CodigoPostal { get; }

        public DireccionCliente(string calle, string ciudad, string codigoPostal)
        {
            if (string.IsNullOrWhiteSpace(calle))
                throw new ArgumentException("La calle es requerida.", nameof(calle));
            if (string.IsNullOrWhiteSpace(ciudad))
                throw new ArgumentException("La ciudad es requerida.", nameof(ciudad));
            if (string.IsNullOrWhiteSpace(codigoPostal))
                throw new ArgumentException("El código postal es requerido.", nameof(codigoPostal));

            Calle        = calle;
            Ciudad       = ciudad;
            CodigoPostal = codigoPostal;
        }

        public override string ToString() => $"{Calle}, {Ciudad} {CodigoPostal}";
    }

    /// <summary>
    /// Reemplaza la tupla anónima (int, int, decimal) por un tipo con semántica clara.
    /// Subtotal se calcula aquí: Feature Envy eliminado del gestor.
    /// </summary>
    public sealed class ItemVenta
    {
        public int     ProductoId     { get; }
        public int     Cantidad       { get; }
        public decimal PrecioUnitario { get; }
        public decimal Subtotal       => Cantidad * PrecioUnitario;

        public ItemVenta(int productoId, int cantidad, decimal precioUnitario)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));
            if (precioUnitario < 0)
                throw new ArgumentException("El precio no puede ser negativo.", nameof(precioUnitario));

            ProductoId     = productoId;
            Cantidad       = cantidad;
            PrecioUnitario = precioUnitario;
        }
    }

    // ── Parameter Object ──────────────────────────────────────────────────────

    /// <summary>
    /// Introduce Parameter Object: reemplaza los 7 primitivos sueltos del método original.
    /// La validación básica se centraliza en el constructor.
    /// </summary>
    public sealed class PedidoVenta
    {
        public string                  ClienteNombre { get; }
        public string                  ClienteEmail  { get; }
        public DireccionCliente        Direccion     { get; }
        public TipoCliente             TipoCliente   { get; }
        public IReadOnlyList<ItemVenta> Items        { get; }

        public PedidoVenta(
            string clienteNombre,
            string clienteEmail,
            DireccionCliente direccion,
            TipoCliente tipoCliente,
            IReadOnlyList<ItemVenta> items)
        {
            if (string.IsNullOrWhiteSpace(clienteNombre))
                throw new ArgumentException("El nombre del cliente es requerido.", nameof(clienteNombre));
            if (string.IsNullOrWhiteSpace(clienteEmail))
                throw new ArgumentException("El email del cliente es requerido.", nameof(clienteEmail));
            if (items == null || items.Count == 0)
                throw new ArgumentException("El pedido debe tener al menos un ítem.", nameof(items));

            ClienteNombre = clienteNombre;
            ClienteEmail  = clienteEmail;
            Direccion     = direccion ?? throw new ArgumentNullException(nameof(direccion));
            TipoCliente   = tipoCliente;
            Items         = items;
        }
    }

    /// <summary>
    /// Value Object de salida: encapsula los números calculados para que el llamador
    /// no necesite reconstruirlos.
    /// </summary>
    public sealed class ResultadoVenta
    {
        public decimal Subtotal  { get; }
        public decimal Descuento { get; }
        public decimal Impuesto  { get; }
        public decimal Total     { get; }

        public ResultadoVenta(decimal subtotal, decimal descuento, decimal impuesto)
        {
            Subtotal  = subtotal;
            Descuento = descuento;
            Impuesto  = impuesto;
            Total     = subtotal - descuento + impuesto;
        }
    }

    // ── Strategy: ICalculadorDescuento (OCP) ─────────────────────────────────

    public interface ICalculadorDescuento
    {
        decimal Calcular(decimal subtotal);
    }

    public sealed class DescuentoRegular   : ICalculadorDescuento
    {
        public decimal Calcular(decimal subtotal) => subtotal * 0.05m;
    }

    public sealed class DescuentoMayorista : ICalculadorDescuento
    {
        public decimal Calcular(decimal subtotal) => subtotal * 0.20m;
    }

    public sealed class DescuentoVIP       : ICalculadorDescuento
    {
        public decimal Calcular(decimal subtotal) => subtotal * 0.15m;
    }

    /// <summary>
    /// Factory Method: centraliza la selección de estrategia.
    /// Agregar TipoCliente.Distribuidor solo requiere tocar esta clase.
    /// </summary>
    public static class FabricaDescuento
    {
        public static ICalculadorDescuento Crear(TipoCliente tipo) => tipo switch
        {
            TipoCliente.Regular   => new DescuentoRegular(),
            TipoCliente.Mayorista => new DescuentoMayorista(),
            TipoCliente.VIP       => new DescuentoVIP(),
            _                     => throw new ArgumentOutOfRangeException(nameof(tipo))
        };
    }

    // ── Abstracciones de infraestructura (DIP) ────────────────────────────────

    public interface IServicioNotificacion
    {
        void EnviarConfirmacion(string clienteNombre, string clienteEmail, ResultadoVenta resultado);
    }

    public interface IGeneradorFactura
    {
        void Generar(PedidoVenta pedido, ResultadoVenta resultado);
    }

    // ── Implementaciones de infraestructura ───────────────────────────────────

    /// <summary>
    /// SRP: única responsabilidad = enviar emails de confirmación.
    /// GestorVentas no sabe nada de SMTP.
    /// </summary>
    public sealed class ServicioNotificacionEmail : IServicioNotificacion
    {
        private readonly string _smtpHost;
        private readonly string _remitente;

        public ServicioNotificacionEmail(string smtpHost, string remitente)
        {
            _smtpHost  = smtpHost  ?? throw new ArgumentNullException(nameof(smtpHost));
            _remitente = remitente ?? throw new ArgumentNullException(nameof(remitente));
        }

        public void EnviarConfirmacion(string clienteNombre, string clienteEmail, ResultadoVenta resultado)
        {
            using var smtp    = new SmtpClient(_smtpHost);
            using var mensaje = new MailMessage(
                _remitente,
                clienteEmail,
                "Confirmación de Pedido",
                $"Estimado {clienteNombre}, su total es {resultado.Total:C}");

            smtp.Send(mensaje);
        }
    }

    /// <summary>
    /// SRP: única responsabilidad = persistir la factura en disco.
    /// GestorVentas no sabe nada de File.WriteAllText.
    /// </summary>
    public sealed class GeneradorFacturaTxt : IGeneradorFactura
    {
        private readonly string _rutaBase;

        public GeneradorFacturaTxt(string rutaBase) =>
            _rutaBase = rutaBase ?? throw new ArgumentNullException(nameof(rutaBase));

        public void Generar(PedidoVenta pedido, ResultadoVenta resultado)
        {
            string ruta      = Path.Combine(_rutaBase, $"factura_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            string contenido = ComponeContenido(pedido, resultado);
            File.WriteAllText(ruta, contenido);
        }

        private static string ComponeContenido(PedidoVenta pedido, ResultadoVenta resultado) =>
            $"FACTURA\n"                               +
            $"Cliente:   {pedido.ClienteNombre}\n"     +
            $"Dirección: {pedido.Direccion}\n"         +
            $"Subtotal:  {resultado.Subtotal:C}\n"     +
            $"Descuento: {resultado.Descuento:C}\n"    +
            $"Impuesto:  {resultado.Impuesto:C}\n"     +
            $"Total:     {resultado.Total:C}";
    }

    // ── GestorVentas refactorizado ────────────────────────────────────────────

    /// <summary>
    /// Después del refactoring, GestorVentas es un orquestador puro:
    ///   1. Valida el pedido (Extract Method)
    ///   2. Calcula los totales (Extract Method)
    ///   3. Delega la factura y la notificación a colaboradores inyectados (DIP)
    ///
    /// No importa qué proveedor de email o formato de factura se use:
    /// GestorVentas nunca cambia por esas razones (SRP + OCP).
    /// </summary>
    public sealed class GestorVentas
    {
        private const decimal TasaIVA = 0.13m; // IVA Costa Rica

        private readonly IServicioNotificacion _notificacion;
        private readonly IGeneradorFactura     _generadorFactura;

        public GestorVentas(
            IServicioNotificacion notificacion,
            IGeneradorFactura     generadorFactura)
        {
            _notificacion     = notificacion     ?? throw new ArgumentNullException(nameof(notificacion));
            _generadorFactura = generadorFactura ?? throw new ArgumentNullException(nameof(generadorFactura));
        }

        // Punto de entrada: orquesta sin encapsular detalles de infraestructura.
        public ResultadoVenta ProcesarVenta(PedidoVenta pedido)
        {
            if (pedido == null) throw new ArgumentNullException(nameof(pedido));

            ValidarPedido(pedido);                  // Extract Method – validación

            var resultado = CalcularTotales(pedido); // Extract Method – cálculo

            _generadorFactura.Generar(pedido, resultado);                               // notificación
            _notificacion.EnviarConfirmacion(pedido.ClienteNombre, pedido.ClienteEmail, resultado);

            return resultado;
        }

        // ── Extract Method: validación ────────────────────────────────────────
        private static void ValidarPedido(PedidoVenta pedido)
        {
            foreach (var item in pedido.Items)
            {
                if (item.Cantidad <= 0)
                    throw new ArgumentException(
                        $"Cantidad inválida para producto {item.ProductoId}.");
                if (item.PrecioUnitario < 0)
                    throw new ArgumentException(
                        $"Precio inválido para producto {item.ProductoId}.");
            }
        }

        // ── Extract Method: cálculo ───────────────────────────────────────────
        private static ResultadoVenta CalcularTotales(PedidoVenta pedido)
        {
            decimal subtotal  = CalcularSubtotal(pedido.Items);
            decimal descuento = AplicarDescuento(subtotal, pedido.TipoCliente);
            decimal impuesto  = AplicarImpuesto(subtotal - descuento);

            return new ResultadoVenta(subtotal, descuento, impuesto);
        }

        private static decimal CalcularSubtotal(IReadOnlyList<ItemVenta> items)
        {
            decimal total = 0;
            foreach (var item in items)
                total += item.Subtotal;
            return total;
        }

        private static decimal AplicarDescuento(decimal subtotal, TipoCliente tipo)
        {
            var calculador = FabricaDescuento.Crear(tipo);
            return calculador.Calcular(subtotal);
        }

        private static decimal AplicarImpuesto(decimal baseImponible) =>
            baseImponible * TasaIVA;
    }

    // ── Ejemplo de composición (Program.cs / DI container) ───────────────────
    //
    //  var gestor = new GestorVentas(
    //      new ServicioNotificacionEmail(Configuracion.ServidorSMTP, Configuracion.EmailAdmin),
    //      new GeneradorFacturaTxt(Configuracion.RutaReportes));
    //
    //  var pedido = new PedidoVenta(
    //      clienteNombre : "Ana López",
    //      clienteEmail  : "ana@example.com",
    //      direccion     : new DireccionCliente("Av. Central 10", "San José", "10101"),
    //      tipoCliente   : TipoCliente.VIP,
    //      items         : new[]
    //      {
    //          new ItemVenta(productoId: 1, cantidad: 3, precioUnitario: 1500m),
    //          new ItemVenta(productoId: 7, cantidad: 1, precioUnitario: 4200m),
    //      });
    //
    //  ResultadoVenta resultado = gestor.ProcesarVenta(pedido);
    //  Console.WriteLine($"Total: {resultado.Total:C}");
}

/*
 * 
 // PROBLEMA: God Class + Long Method + Primitive Obsession + Feature Envy
public class GestorVentas
{
    // Primitive Obsession: datos de dirección como strings sueltos
    public decimal CalcularTotal(List<(int productoId, int cantidad, decimal precio)> items,
        string clienteNombre, string clienteEmail, string clienteCalle,
        string clienteCiudad, string clienteCodigoPostal, string tipoCliente)
    {
        // Long Method: todo en un solo método de 50+ líneas
        decimal subtotal = 0;
        foreach (var item in items)
        {
            subtotal += item.cantidad * item.precio;
        }

        // Feature Envy: lógica de descuento debería estar en Cliente
        decimal descuento = 0;
        if (tipoCliente == "VIP")
            descuento = subtotal * 0.15m;
        else if (tipoCliente == "Mayorista")
            descuento = subtotal * 0.20m;
        else if (tipoCliente == "Regular")
            descuento = subtotal * 0.05m;

        decimal impuesto = (subtotal - descuento) * 0.13m; // IVA CR
        decimal total = subtotal - descuento + impuesto;

        // God Class: envío de email dentro del gestor de ventas
        var smtp = new System.Net.Mail.SmtpClient("smtp.empresa.com");
        var msg = new System.Net.Mail.MailMessage(
            "ventas@empresa.com", clienteEmail,
            "Confirmación de Pedido",
            $"Estimado {clienteNombre}, su total es {total:C}");
        smtp.Send(msg);

        // God Class: generación de factura también aquí
        string factura = $"FACTURA\nCliente: {clienteNombre}\n" +
            $"Dirección: {clienteCalle}, {clienteCiudad} {clienteCodigoPostal}\n" +
            $"Subtotal: {subtotal:C}\nDescuento: {descuento:C}\n" +
            $"Impuesto: {impuesto:C}\nTotal: {total:C}";
        File.WriteAllText($"factura_{DateTime.Now:yyyyMMdd}.txt", factura);

        return total;
    }
}

 */