using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VentasApp_Tienda742.Models;

namespace VentasApp_Tienda742.Services
{
    public class PdfService
    {
        public async Task<string> GenerarTicketVentaPdfAsync(Venta venta)
        {
            string fileName = $"Ticket_Venta_{venta.PedidoID}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(226, 420, Unit.Point); // 80mm POS Thermal Receipt
                    page.Margin(10);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(8.5f).FontFamily("Courier New").FontColor(QuestPDF.Helpers.Colors.Black));

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text("TIENDA 742 JPV").FontSize(12).Bold();
                        col.Item().AlignCenter().Text("RNC: 101-987654-3").FontSize(7.5f);
                        col.Item().AlignCenter().Text("Santo Domingo, Rep. Dominicana").FontSize(7.5f);
                        col.Item().AlignCenter().Text("Tel: (809) 555-0199").FontSize(7.5f);
                        col.Item().PaddingVertical(3).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(4).Column(col =>
                    {
                        col.Item().Text($"FECHA : {venta.FechaFormateada}");
                        col.Item().Text($"TICKET: #{venta.PedidoID}").Bold();
                        col.Item().Text($"CLIENT: {venta.ClienteNombre}");
                        if (!string.IsNullOrEmpty(venta.Ciudad))
                            col.Item().Text($"CIUDAD: {venta.Ciudad}");
                        if (!string.IsNullOrEmpty(venta.Telefono))
                            col.Item().Text($"TEL   : {venta.Telefono}");

                        col.Item().PaddingVertical(3).LineHorizontal(0.5f);

                        // Tabla de Items
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Descripcion
                                columns.RelativeColumn(1); // Cant
                                columns.RelativeColumn(2); // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("DESCRIPCION").Bold();
                                header.Cell().AlignCenter().Text("CANT").Bold();
                                header.Cell().AlignRight().Text("TOTAL").Bold();
                            });

                            table.Cell().Text(venta.ProductoNombre);
                            table.Cell().AlignCenter().Text(venta.Cantidad.ToString());
                            table.Cell().AlignRight().Text($"${venta.Subtotal:N2}");
                        });

                        col.Item().PaddingVertical(4).LineHorizontal(0.5f);

                        // Totales
                        col.Item().AlignRight().Text($"SUBTOTAL : RD$ {venta.Subtotal:N2}");
                        col.Item().AlignRight().Text($"ITBIS 18%: RD$ {venta.Impuesto:N2}");
                        col.Item().AlignRight().Text($"TOTAL    : RD$ {venta.Total:N2}").Bold().FontSize(10);

                        col.Item().PaddingVertical(6).LineHorizontal(1);
                        col.Item().AlignCenter().Text("¡GRACIAS POR SU COMPRA!").Bold();
                        col.Item().AlignCenter().Text("Conserve este ticket para garantias.").FontSize(7);
                    });
                });
            });

            doc.GeneratePdf(filePath);
            await AbrirArchivoPdfAsync(filePath);
            return filePath;
        }

        public async Task<string> GenerarReporteVentasPdfAsync(List<Venta> ventas)
        {
            string fileName = $"Reporte_Ventas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            decimal totalVentas = ventas.Sum(v => v.Total);
            decimal totalSubtotal = ventas.Sum(v => v.Subtotal);
            decimal totalItbis = ventas.Sum(v => v.Impuesto);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(headerCol =>
                            {
                                headerCol.Item().Text("TIENDA 742 JPV").FontSize(16).Bold().FontColor("#1E3A8A");
                                headerCol.Item().Text("REPORTE OFICIAL DE VENTAS Y FACTURACIÓN").FontSize(12).SemiBold().FontColor("#475569");
                                headerCol.Item().Text($"Generado el: {DateTime.Now:dd/MM/yyyy hh:mm tt}").FontSize(8).FontColor("#64748B");
                            });

                            row.ConstantItem(140).Column(kpiCol =>
                            {
                                kpiCol.Item().Background("#F1F5F9").Padding(6).Column(box =>
                                {
                                    box.Item().Text("TOTAL GENERAL").FontSize(8).FontColor("#64748B");
                                    box.Item().Text($"RD$ {totalVentas:N2}").FontSize(12).Bold().FontColor("#10B981");
                                    box.Item().Text($"{ventas.Count} pedidos").FontSize(8).FontColor("#334155");
                                });
                            });
                        });

                        col.Item().PaddingVertical(8).LineHorizontal(1).LineColor("#CBD5E1");
                    });

                    page.Content().PaddingVertical(5).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35); // #
                                columns.ConstantColumn(85); // Fecha
                                columns.RelativeColumn(3);  // Cliente
                                columns.RelativeColumn(3);  // Producto
                                columns.ConstantColumn(35); // Cant
                                columns.ConstantColumn(60); // Precio
                                columns.ConstantColumn(65); // Subtotal
                                columns.ConstantColumn(55); // ITBIS
                                columns.ConstantColumn(75); // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#1E3A8A").Padding(4).Text("#").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#1E3A8A").Padding(4).Text("FECHA").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#1E3A8A").Padding(4).Text("CLIENTE").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#1E3A8A").Padding(4).Text("PRODUCTO").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#1E3A8A").Padding(4).AlignCenter().Text("CANT").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#1E3A8A").Padding(4).AlignRight().Text("PRECIO").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#1E3A8A").Padding(4).AlignRight().Text("SUBTOTAL").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#1E3A8A").Padding(4).AlignRight().Text("ITBIS").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#1E3A8A").Padding(4).AlignRight().Text("TOTAL").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                            });

                            int index = 0;
                            foreach (var v in ventas)
                            {
                                string bgColor = (index % 2 == 0) ? "#FFFFFF" : "#F8FAFC";
                                index++;

                                table.Cell().Background(bgColor).Padding(3).Text(v.PedidoID.ToString());
                                table.Cell().Background(bgColor).Padding(3).Text(v.FechaPedido.ToString("dd/MM/yy HH:mm"));
                                table.Cell().Background(bgColor).Padding(3).Text(v.ClienteNombre);
                                table.Cell().Background(bgColor).Padding(3).Text(v.ProductoNombre);
                                table.Cell().Background(bgColor).Padding(3).AlignCenter().Text(v.Cantidad.ToString());
                                table.Cell().Background(bgColor).Padding(3).AlignRight().Text($"${v.Precio:N2}");
                                table.Cell().Background(bgColor).Padding(3).AlignRight().Text($"${v.Subtotal:N2}");
                                table.Cell().Background(bgColor).Padding(3).AlignRight().Text($"${v.Impuesto:N2}");
                                table.Cell().Background(bgColor).Padding(3).AlignRight().Text($"${v.Total:N2}").Bold();
                            }

                            // Fila de totales
                            table.Cell().ColumnSpan(4).Background("#E2E8F0").Padding(4).Text("TOTALES ACUMULADOS").Bold();
                            table.Cell().Background("#E2E8F0").Padding(4).AlignCenter().Text(ventas.Sum(x => x.Cantidad).ToString()).Bold();
                            table.Cell().Background("#E2E8F0").Padding(4).Text("");
                            table.Cell().Background("#E2E8F0").Padding(4).AlignRight().Text($"${totalSubtotal:N2}").Bold();
                            table.Cell().Background("#E2E8F0").Padding(4).AlignRight().Text($"${totalItbis:N2}").Bold();
                            table.Cell().Background("#E2E8F0").Padding(4).AlignRight().Text($"${totalVentas:N2}").Bold().FontColor("#10B981");
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                        x.Span(" - Tienda 742 JPV Sistema de Facturación");
                    });
                });
            });

            doc.GeneratePdf(filePath);
            await AbrirArchivoPdfAsync(filePath);
            return filePath;
        }

        public async Task<string> GenerarReporteInventarioPdfAsync(List<Producto> productos)
        {
            string fileName = $"Reporte_Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            int totalUnidades = productos.Sum(p => p.Stock);
            decimal valorTotal = productos.Sum(p => p.ValorTotalInventario);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(25);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(headerCol =>
                            {
                                headerCol.Item().Text("TIENDA 742 JPV").FontSize(16).Bold().FontColor("#1E3A8A");
                                headerCol.Item().Text("REPORTE DE INVENTARIO Y DISPONIBILIDAD").FontSize(12).SemiBold().FontColor("#475569");
                                headerCol.Item().Text($"Corte al: {DateTime.Now:dd/MM/yyyy hh:mm tt}").FontSize(8).FontColor("#64748B");
                            });

                            row.ConstantItem(150).Column(kpiCol =>
                            {
                                kpiCol.Item().Background("#F1F5F9").Padding(6).Column(box =>
                                {
                                    box.Item().Text("VALOR EN STOCK").FontSize(8).FontColor("#64748B");
                                    box.Item().Text($"RD$ {valorTotal:N2}").FontSize(12).Bold().FontColor("#10B981");
                                    box.Item().Text($"{totalUnidades} unidades en total").FontSize(8).FontColor("#334155");
                                });
                            });
                        });

                        col.Item().PaddingVertical(8).LineHorizontal(1).LineColor("#CBD5E1");
                    });

                    page.Content().PaddingVertical(5).Column(col =>
                    {
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40); // ID
                                columns.RelativeColumn(4);  // Producto
                                columns.ConstantColumn(90); // Precio Unitario
                                columns.ConstantColumn(80); // Stock
                                columns.ConstantColumn(110); // Valor Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#0F172A").Padding(4).Text("ID").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#0F172A").Padding(4).Text("DESCRIPCIÓN DEL PRODUCTO").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#0F172A").Padding(4).AlignRight().Text("PRECIO UNIT.").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#0F172A").Padding(4).AlignCenter().Text("STOCK").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                                header.Cell().Background("#0F172A").Padding(4).AlignRight().Text("VALOR TOTAL").Bold().FontColor(QuestPDF.Helpers.Colors.White);
                            });

                            int index = 0;
                            foreach (var p in productos)
                            {
                                string bgColor = (index % 2 == 0) ? "#FFFFFF" : "#F8FAFC";
                                index++;

                                table.Cell().Background(bgColor).Padding(4).Text(p.ProductoID.ToString());
                                table.Cell().Background(bgColor).Padding(4).Text(p.NombreProducto);
                                table.Cell().Background(bgColor).Padding(4).AlignRight().Text($"RD$ {p.Precio:N2}");
                                
                                // Color alert si stock es bajo (< 10)
                                string stockColor = p.Stock < 10 ? "#EF4444" : "#0F172A";
                                table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(p.Stock.ToString()).Bold().FontColor(stockColor);
                                
                                table.Cell().Background(bgColor).Padding(4).AlignRight().Text($"RD$ {p.ValorTotalInventario:N2}");
                            }

                            // Totales
                            table.Cell().ColumnSpan(3).Background("#E2E8F0").Padding(4).Text("TOTAL GENERAL").Bold();
                            table.Cell().Background("#E2E8F0").Padding(4).AlignCenter().Text(totalUnidades.ToString()).Bold();
                            table.Cell().Background("#E2E8F0").Padding(4).AlignRight().Text($"RD$ {valorTotal:N2}").Bold().FontColor("#10B981");
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                        x.Span(" - Tienda 742 JPV Sistema de Inventario");
                    });
                });
            });

            doc.GeneratePdf(filePath);
            await AbrirArchivoPdfAsync(filePath);
            return filePath;
        }

        private Task AbrirArchivoPdfAsync(string filePath)
        {
            try
            {
#if WINDOWS
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                return Task.CompletedTask;
#else
                return Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al abrir el PDF: {ex.Message}");
                return Task.CompletedTask;
            }
        }
    }
}
