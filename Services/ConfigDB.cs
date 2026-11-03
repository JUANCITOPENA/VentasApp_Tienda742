namespace VentasApp_Tienda742.Services
{
    public static class ConfigDB
    {
        public static string ConnectionString { get; set; } = 
            "Server=192.168.2.68,1433;Database=TiendaDB_742;User Id=JUANCITO;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=True;Connect Timeout=15;";
    }
}
