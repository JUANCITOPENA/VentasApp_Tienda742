using System;

namespace VentasApp_Tienda742.Models
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Ciudad { get; set; }
        public string? Telefono { get; set; }
        public DateTime? FechaRegistro { get; set; }

        public string DisplayText => $"{Nombre} ({Ciudad ?? "Sin Ciudad"})";
    }
}
