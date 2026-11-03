using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using VentasApp_Tienda742.Models;
using VentasApp_Tienda742.Services;

namespace VentasApp_Tienda742.Views
{
    public partial class NuevaVentaPage : ContentPage
    {
        private readonly VentaService _ventaService = new();
        private readonly PdfService _pdfService = new();

        private List<Cliente> _clientes = new();
        private List<Producto> _productos = new();

        public NuevaVentaPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarDatosInicialesAsync();
        }

        private async Task CargarDatosInicialesAsync()
        {
            try
            {
                SetLoading(true);

                _clientes = await _ventaService.GetClientesAsync();
                PickerClientes.ItemsSource = _clientes;

                _productos = await _ventaService.GetProductosAsync();
                PickerProductos.ItemsSource = _productos;

                CalcularTotales();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error al conectar", $"No se pudieron cargar los datos de SQL Server:\n{ex.Message}", "Entendido");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void OnClienteSeleccionado(object sender, EventArgs e)
        {
            // Evento de selección de cliente
        }

        private void OnProductoSeleccionado(object sender, EventArgs e)
        {
            if (PickerProductos.SelectedItem is Producto prod)
            {
                LblStockDisponible.Text = $"Stock actual: {prod.Stock} uds";
                LblStockDisponible.TextColor = prod.Stock < 5 ? Color.FromArgb("#EF4444") : Color.FromArgb("#2563EB");
            }
            else
            {
                LblStockDisponible.Text = "Stock: --";
                LblStockDisponible.TextColor = Color.FromArgb("#2563EB");
            }
            CalcularTotales();
        }

        private void OnAumentarCantidad(object sender, EventArgs e)
        {
            if (int.TryParse(TxtCantidad.Text, out int actual))
            {
                TxtCantidad.Text = (actual + 1).ToString();
            }
            else
            {
                TxtCantidad.Text = "1";
            }
        }

        private void OnDisminuirCantidad(object sender, EventArgs e)
        {
            if (int.TryParse(TxtCantidad.Text, out int actual) && actual > 1)
            {
                TxtCantidad.Text = (actual - 1).ToString();
            }
            else
            {
                TxtCantidad.Text = "1";
            }
        }

        private void OnCantidadChanged(object sender, TextChangedEventArgs e)
        {
            CalcularTotales();
        }

        private void CalcularTotales()
        {
            if (PickerProductos?.SelectedItem is Producto prod && int.TryParse(TxtCantidad?.Text, out int cant) && cant > 0)
            {
                decimal subtotal = prod.Precio * cant;
                decimal itbis = subtotal * 0.18m;
                decimal total = subtotal + itbis;

                LblSubtotal.Text = $"RD$ {subtotal:N2}";
                LblImpuesto.Text = $"RD$ {itbis:N2}";
                LblTotal.Text = $"RD$ {total:N2}";
            }
            else
            {
                if (LblSubtotal != null) LblSubtotal.Text = "RD$ 0.00";
                if (LblImpuesto != null) LblImpuesto.Text = "RD$ 0.00";
                if (LblTotal != null) LblTotal.Text = "RD$ 0.00";
            }
        }

        private async void OnRegistrarVentaClicked(object sender, EventArgs e)
        {
            if (PickerClientes.SelectedItem is not Cliente cliente)
            {
                await DisplayAlert("Validación", "Por favor seleccione un cliente de la lista.", "OK");
                return;
            }

            if (PickerProductos.SelectedItem is not Producto producto)
            {
                await DisplayAlert("Validación", "Por favor seleccione un producto a facturar.", "OK");
                return;
            }

            if (!int.TryParse(TxtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                await DisplayAlert("Validación", "Ingrese una cantidad válida mayor a cero.", "OK");
                return;
            }

            if (cantidad > producto.Stock)
            {
                await DisplayAlert("Stock Insuficiente", $"Solo quedan {producto.Stock} unidades disponibles de {producto.NombreProducto}.", "OK");
                return;
            }

            try
            {
                SetLoading(true);

                var ventaRegistrada = await _ventaService.RegistrarVentaAsync(cliente.ClienteID, producto.ProductoID, cantidad);

                // Preguntar al usuario si desea imprimir el ticket de inmediato
                bool imprimir = await DisplayAlert(
                    "¡Venta Registrada con Éxito!",
                    $"Ticket #{ventaRegistrada.PedidoID}\nCliente: {ventaRegistrada.ClienteNombre}\nTotal: RD$ {ventaRegistrada.Total:N2}\n\n¿Desea generar e imprimir el Ticket en PDF ahora?",
                    "Sí, Imprimir Ticket",
                    "Cerrar");

                if (imprimir)
                {
                    await _pdfService.GenerarTicketVentaPdfAsync(ventaRegistrada);
                }

                // Resetear formulario
                TxtCantidad.Text = "1";
                PickerProductos.SelectedItem = null;
                await CargarDatosInicialesAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error en la Venta", $"No se pudo procesar la venta:\n{ex.Message}", "OK");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void OnRecargarClicked(object sender, EventArgs e)
        {
            await CargarDatosInicialesAsync();
        }

        private void SetLoading(bool isLoading)
        {
            IndicadorCarga.IsRunning = isLoading;
            IndicadorCarga.IsVisible = isLoading;
        }
    }
}
