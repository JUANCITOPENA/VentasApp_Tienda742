namespace VentasApp_Tienda742.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        public string DisplayText => $"{NombreProducto} - RD$ {Precio:N2} (Stock: {Stock})";
        public decimal ValorTotalInventario => Precio * Stock;
    }
}
