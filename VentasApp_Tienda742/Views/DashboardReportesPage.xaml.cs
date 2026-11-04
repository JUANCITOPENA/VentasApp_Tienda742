using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using VentasApp_Tienda742.Models;
using VentasApp_Tienda742.Services;

namespace VentasApp_Tienda742.Views
{
    public partial class DashboardReportesPage : ContentPage
    {
        private readonly VentaService _ventaService = new();
        private readonly PdfService _pdfService = new();

        private List<Venta> _todasLasVentas = new();

        public DashboardReportesPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarTodoAsync();
        }

        private async Task CargarTodoAsync()
        {
            try
            {
                SetLoading(true);

                // 1. Cargar KPIs de Dashboard
                var stats = await _ventaService.GetDashboardStatsAsync();
                LblKpiTotalVentas.Text = stats.TotalVentasFormateado;
                LblKpiTotalPedidos.Text = stats.TotalPedidos.ToString();
                LblKpiTotalClientes.Text = stats.TotalClientes.ToString();
                LblKpiTotalStock.Text = $"{stats.TotalStockUnidades} uds";

                // 2. Cargar Lista de Ventas
                _todasLasVentas = await _ventaService.GetVentasAsync();
                FiltrarVentas(TxtBuscarVentas.Text);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error de Conexión", $"No se pudieron cargar los reportes:\n{ex.Message}", "OK");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void OnBuscarVentasChanged(object sender, TextChangedEventArgs e)
        {
            FiltrarVentas(e.NewTextValue);
        }

        private void FiltrarVentas(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                ColeccionVentas.ItemsSource = _todasLasVentas;
            }
            else
            {
                string term = texto.Trim().ToLowerInvariant();
                var filtradas = _todasLasVentas.Where(v =>
                    (v.ClienteNombre?.ToLowerInvariant().Contains(term) ?? false) ||
                    (v.ProductoNombre?.ToLowerInvariant().Contains(term) ?? false) ||
                    (v.Ciudad?.ToLowerInvariant().Contains(term) ?? false) ||
                    v.PedidoID.ToString().Contains(term)
                ).ToList();

                ColeccionVentas.ItemsSource = filtradas;
            }
        }

        private async void OnGenerarReporteVentasPdfClicked(object sender, EventArgs e)
        {
            if (_todasLasVentas == null || _todasLasVentas.Count == 0)
            {
                await DisplayAlert("Reporte Vacío", "No hay registros de ventas para generar el reporte.", "OK");
                return;
            }

            try
            {
                SetLoading(true);
                string ruta = await _pdfService.GenerarReporteVentasPdfAsync(_todasLasVentas);
                await DisplayAlert("PDF Generado", $"El reporte de ventas se generó exitosamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error PDF", $"No se pudo generar el PDF:\n{ex.Message}", "OK");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void OnGenerarReporteInventarioPdfClicked(object sender, EventArgs e)
        {
            try
            {
                SetLoading(true);
                var inventario = await _ventaService.GetInventarioAsync();
                if (inventario == null || inventario.Count == 0)
                {
                    await DisplayAlert("Inventario Vacío", "No hay productos registrados en el inventario.", "OK");
                    return;
                }

                string ruta = await _pdfService.GenerarReporteInventarioPdfAsync(inventario);
                await DisplayAlert("PDF Generado", $"El reporte de inventario se generó exitosamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error PDF", $"No se pudo generar el reporte de inventario:\n{ex.Message}", "OK");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void OnImprimirTicketItemClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Venta venta)
            {
                try
                {
                    SetLoading(true);
                    await _pdfService.GenerarTicketVentaPdfAsync(venta);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error al Generar Ticket", ex.Message, "OK");
                }
                finally
                {
                    SetLoading(false);
                }
            }
        }

        private async void OnRefrescarClicked(object sender, EventArgs e)
        {
            await CargarTodoAsync();
        }

        private void SetLoading(bool isLoading)
        {
            IndicadorCargaReportes.IsRunning = isLoading;
            IndicadorCargaReportes.IsVisible = isLoading;
        }
    }
}
