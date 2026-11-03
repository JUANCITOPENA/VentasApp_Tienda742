namespace VentasApp_Tienda742.Models
{
    public class DashboardStats
    {
        public decimal TotalVentasMonto { get; set; }
        public int TotalPedidos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalStockUnidades { get; set; }
        public string TopProductoNombre { get; set; } = "N/A";
        public int TopProductoCantidad { get; set; }

        public string TotalVentasFormateado => $"RD$ {TotalVentasMonto:N2}";
        public string TopProductoResumen => $"{TopProductoNombre} ({TopProductoCantidad} uds)";
    }
}
