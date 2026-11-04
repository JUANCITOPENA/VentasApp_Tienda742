using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using VentasApp_Tienda742.Models;

namespace VentasApp_Tienda742.Services
{
    public class VentaService
    {
        private string ConnectionString => ConfigDB.ConnectionString;

        public async Task<List<Cliente>> GetClientesAsync()
        {
            var list = new List<Cliente>();
            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(
                "SELECT ClienteID, Nombre, Email, Ciudad, Telefono, FechaRegistro FROM Clientes ORDER BY Nombre ASC;", 
                conn);
            
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Cliente
                {
                    ClienteID = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Ciudad = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                    FechaRegistro = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
                });
            }
            return list;
        }

        public async Task<List<Producto>> GetProductosAsync()
        {
            var list = new List<Producto>();
            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(
                "SELECT ProductoID, NombreProducto, Precio, Stock FROM Productos ORDER BY NombreProducto ASC;", 
                conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Producto
                {
                    ProductoID = reader.GetInt32(0),
                    NombreProducto = reader.GetString(1),
                    Precio = reader.GetDecimal(2),
                    Stock = reader.GetInt32(3)
                });
            }
            return list;
        }

        public async Task<Venta> RegistrarVentaAsync(int clienteId, int productoId, int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que 0.");

            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();
            try
            {
                // 1. Validar producto y stock disponible
                int currentStock = 0;
                string nombreProd = "";
                decimal precioProd = 0;

                using (var checkCmd = new SqlCommand(
                    "SELECT NombreProducto, Precio, Stock FROM Productos WITH (UPDLOCK) WHERE ProductoID = @pId;", 
                    conn, transaction))
                {
                    checkCmd.Parameters.AddWithValue("@pId", productoId);
                    using var reader = await checkCmd.ExecuteReaderAsync();
                    if (!await reader.ReadAsync())
                        throw new Exception("El producto seleccionado no existe.");

                    nombreProd = reader.GetString(0);
                    precioProd = reader.GetDecimal(1);
                    currentStock = reader.GetInt32(2);
                }

                if (currentStock < cantidad)
                    throw new Exception($"Stock insuficiente. Solo quedan {currentStock} unidades disponibles de {nombreProd}.");

                // 2. Insertar en Pedidos
                int pedidoId = 0;
                DateTime fechaPedido = DateTime.Now;
                using (var insertCmd = new SqlCommand(
                    "INSERT INTO Pedidos (ClienteID, ProductoID, Cantidad, FechaPedido) " +
                    "OUTPUT INSERTED.PedidoID, INSERTED.FechaPedido " +
                    "VALUES (@cId, @pId, @cant, GETDATE());", 
                    conn, transaction))
                {
                    insertCmd.Parameters.AddWithValue("@cId", clienteId);
                    insertCmd.Parameters.AddWithValue("@pId", productoId);
                    insertCmd.Parameters.AddWithValue("@cant", cantidad);

                    using var insertReader = await insertCmd.ExecuteReaderAsync();
                    if (await insertReader.ReadAsync())
                    {
                        pedidoId = insertReader.GetInt32(0);
                        fechaPedido = insertReader.GetDateTime(1);
                    }
                }

                // 3. Descontar Stock en Productos
                using (var updateStockCmd = new SqlCommand(
                    "UPDATE Productos SET Stock = Stock - @cant WHERE ProductoID = @pId;", 
                    conn, transaction))
                {
                    updateStockCmd.Parameters.AddWithValue("@cant", cantidad);
                    updateStockCmd.Parameters.AddWithValue("@pId", productoId);
                    await updateStockCmd.ExecuteNonQueryAsync();
                }

                // 4. Obtener datos del Cliente para armar el objeto Venta completo
                string clienteNombre = "";
                string? ciudad = null;
                string? telefono = null;

                using (var clienteCmd = new SqlCommand(
                    "SELECT Nombre, Ciudad, Telefono FROM Clientes WHERE ClienteID = @cId;", 
                    conn, transaction))
                {
                    clienteCmd.Parameters.AddWithValue("@cId", clienteId);
                    using var cReader = await clienteCmd.ExecuteReaderAsync();
                    if (await cReader.ReadAsync())
                    {
                        clienteNombre = cReader.GetString(0);
                        ciudad = cReader.IsDBNull(1) ? null : cReader.GetString(1);
                        telefono = cReader.IsDBNull(2) ? null : cReader.GetString(2);
                    }
                }

                await transaction.CommitAsync();

                return new Venta
                {
                    PedidoID = pedidoId,
                    ClienteID = clienteId,
                    ClienteNombre = clienteNombre,
                    Ciudad = ciudad,
                    Telefono = telefono,
                    ProductoID = productoId,
                    ProductoNombre = nombreProd,
                    Precio = precioProd,
                    Cantidad = cantidad,
                    FechaPedido = fechaPedido
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Venta>> GetVentasAsync(string? busqueda = null)
        {
            var list = new List<Venta>();
            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            string query = @"
                SELECT 
                    P.PedidoID,
                    P.ClienteID,
                    C.Nombre AS ClienteNombre,
                    C.Ciudad,
                    C.Telefono,
                    P.ProductoID,
                    PR.NombreProducto,
                    PR.Precio,
                    P.Cantidad,
                    P.FechaPedido
                FROM Pedidos P
                INNER JOIN Clientes C ON P.ClienteID = C.ClienteID
                INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID
                WHERE (@q IS NULL OR @q = '' 
                       OR C.Nombre LIKE '%' + @q + '%' 
                       OR PR.NombreProducto LIKE '%' + @q + '%'
                       OR C.Ciudad LIKE '%' + @q + '%')
                ORDER BY P.PedidoID DESC;";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@q", (object?)busqueda ?? DBNull.Value);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Venta
                {
                    PedidoID = reader.GetInt32(0),
                    ClienteID = reader.GetInt32(1),
                    ClienteNombre = reader.GetString(2),
                    Ciudad = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                    ProductoID = reader.GetInt32(5),
                    ProductoNombre = reader.GetString(6),
                    Precio = reader.GetDecimal(7),
                    Cantidad = reader.GetInt32(8),
                    FechaPedido = reader.GetDateTime(9)
                });
            }
            return list;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var stats = new DashboardStats();
            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            // 1. Total Ventas Monto y Total Pedidos
            using (var cmd = new SqlCommand(@"
                SELECT 
                    ISNULL(SUM((PR.Precio * P.Cantidad) + dbo.fn_CalcularImpuesto(PR.Precio * P.Cantidad)), 0) AS TotalVentas,
                    COUNT(P.PedidoID) AS TotalPedidos
                FROM Pedidos P
                INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID;", conn))
            {
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    stats.TotalVentasMonto = reader.GetDecimal(0);
                    stats.TotalPedidos = reader.GetInt32(1);
                }
            }

            // 2. Total Clientes
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes;", conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                stats.TotalClientes = Convert.ToInt32(result ?? 0);
            }

            // 3. Total Stock Unidades
            using (var cmd = new SqlCommand("SELECT ISNULL(SUM(Stock), 0) FROM Productos;", conn))
            {
                var result = await cmd.ExecuteScalarAsync();
                stats.TotalStockUnidades = Convert.ToInt32(result ?? 0);
            }

            // 4. Top Producto Más Vendido
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 
                    PR.NombreProducto, 
                    SUM(P.Cantidad) AS CantidadVendida
                FROM Pedidos P
                INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID
                GROUP BY PR.NombreProducto
                ORDER BY CantidadVendida DESC;", conn))
            {
                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    stats.TopProductoNombre = reader.GetString(0);
                    stats.TopProductoCantidad = reader.GetInt32(1);
                }
            }

            return stats;
        }

        public async Task<List<Producto>> GetInventarioAsync()
        {
            return await GetProductosAsync();
        }
    }
}
