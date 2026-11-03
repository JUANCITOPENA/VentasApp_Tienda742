# 🏪 VentasApp_Tienda742 - Sistema de Ventas, Facturación y Reportes en .NET MAUI

[![GitHub Pages](https://img.shields.io/badge/GitHub%20Pages-Live%20Demo-brightgreen?style=for-the-badge&logo=github)](https://juancitopena.github.io/VentasApp_Tienda742/)
[![.NET 9](https://img.shields.io/badge/.NET%209-MAUI-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC292B?style=for-the-badge&logo=microsoftsqlserver)](https://www.microsoft.com/es/sql-server/)

> 🌐 **TUTORIAL MAESTRO INTERACTIVO EN VIVO (GITHUB PAGES):**  
> Accede a la masterclass completa paso a paso con visores de código, narrador por voz y descargas de scripts SQL:  
> 👉 **[https://juancitopena.github.io/VentasApp_Tienda742/](https://juancitopena.github.io/VentasApp_Tienda742/)**

Aplicación empresarial desarrollada en **.NET 9 MAUI (C# y XAML)** conectada a **Microsoft SQL Server (192.168.2.68:1433)**, con transaccionalidad ACID en el **Módulo de Ventas**, **Mini Dashboard Ejecutivo**, **Historial de Pedidos** y **Generación e Impresión de Documentos en PDF** (Ticket Térmico POS 80mm, Reporte de Ventas A4 e Inventario A4).

---

> 📖 **MANUAL MAESTRO DE INGENIERÍA PASO A PASO (100% COMPLETO):**  
> Para consultar la guía didáctica exhaustiva con todas las explicaciones teóricas, configuración de red, firewall, script SQL completo y código fuente íntegro archivo por archivo, abre el documento:  
> 👉 [`MANUAL_COMPLETO_PASO_A_PASO.md`](./MANUAL_COMPLETO_PASO_A_PASO.md)

---

## 📌 1. Parámetros de Conexión a Base de Datos
- **Servidor SQL:** `192.168.2.68`
- **Puerto:** `1433` (`192.168.2.68,1433`)
- **Base de Datos:** `TiendaDB_742`
- **Usuario:** `JUANCITO`
- **Contraseña:** `123456`
- **Archivo de Configuración:** [`Services/ConfigDB.cs`](./Services/ConfigDB.cs)

---

## 🛠️ 2. Módulos Principales de la Aplicación

### 🛒 Pestaña 1: Nueva Venta (`Views/NuevaVentaPage.xaml`)
- **Catálogos Reactivos:** Carga automática de clientes y productos con existencias desde SQL Server.
- **Control de Cantidad y Validación:** Control `+`/`-` con bloqueo automático si se intenta vender más unidades que el stock disponible.
- **Cálculo Financiero en Vivo:** Desglose instantáneo de Subtotal, ITBIS (18%) y Total general a pagar.
- **Transacción Atómica (ACID):** Descuenta el stock y registra el pedido de forma simultánea.
- **Emisión Directa de Ticket:** Apertura automática del comprobante térmico en PDF de 80mm tras procesar la venta.

### 📊 Pestaña 2: Dashboard y Reportes (`Views/DashboardReportesPage.xaml`)
- **Mini Dashboard Bento (4 KPIs en tiempo real):**
  1. 💰 **Total Ventas:** Monto acumulado en RD$.
  2. 🧾 **Pedidos:** Cantidad de compras facturadas.
  3. 👥 **Clientes:** Cartera de clientes registrados.
  4. 📦 **Stock Total:** Unidades disponibles en almacén.
- **Búsqueda en Vivo:** Filtrado instantáneo por cliente, producto o ciudad.
- **Reportes Vectoriales PDF (QuestPDF):**
  - `📄 Reporte de Ventas A4:` Cuadro analítico de facturación con desglose de impuestos y totales.
  - `📦 Reporte de Inventario A4:` Catálogo valorado con alertas visuales para productos con bajo stock.
  - `🧾 Reimpresión de Tickets:` Botón individual para reimprimir cualquier ticket del historial.

---

## 🚀 3. Instrucciones de Compilación y Ejecución

### Desde PowerShell / CMD:
```powershell
cd "C:\Users\User\Desktop\CARPETA DE TRABAJO DE ANTIGRAVITY CLI-JPV\VentasApp_Tienda742"
dotnet run -f net9.0-windows10.0.19041.0
```

### Desde Visual Studio 2022:
1. Abrir la carpeta del proyecto en Visual Studio 2022.
2. Seleccionar como destino de compilación **Windows Machine**.
3. Presionar **F5**.
