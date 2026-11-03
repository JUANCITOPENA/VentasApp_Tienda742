using System;

namespace VentasApp_Tienda742.Models
{
    public class Venta
    {
        public int PedidoID { get; set; }
        public int ClienteID { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public string? Ciudad { get; set; }
        public string? Telefono { get; set; }
        
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        
        public decimal Subtotal => Precio * Cantidad;
        public decimal Impuesto => Subtotal * 0.18m;
        public decimal Total => Subtotal + Impuesto;
        
        public DateTime FechaPedido { get; set; }

        public string FechaFormateada => FechaPedido.ToString("dd/MM/yyyy hh:mm tt");
        public string TotalFormateado => $"RD$ {Total:N2}";
        public string DetalleResumen => $"{Cantidad}x {ProductoNombre} @ RD$ {Precio:N2}";
    }
}
