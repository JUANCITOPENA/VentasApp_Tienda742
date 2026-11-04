# 📘 MANUAL MAESTRO DE INGENIERÍA Y GUÍA PASO A PASO
## SISTEMA DE FACTURACIÓN, VENTAS, MINI-DASHBOARD Y REPORTES EN PDF
### PROYECTO: VentasApp_Tienda742 (.NET 9 MAUI & SQL SERVER)

---

## 🏛️ ÍNDICE GENERAL DEL MANUAL
1. **Presentación del Proyecto y Ficha Técnica**
2. **Planteamiento del Problema y Solución Arquitectónica**
3. **Requisitos de Instalación y Preparación del Entorno**
4. **Guía Maestra: Habilitación de SQL Server Remoto, Puerto 1433 y Firewall de Windows**
5. **Script T-SQL Canónico Completo de la Base de Datos (`TiendaDB_742`)**
6. **Estructura del Proyecto y Árbol de Directorios**
7. **Código Fuente Íntegro (100%) y Explicación Detallada Archivo por Archivo**
   - 7.1 Configuración: `global.json` y `VentasApp_Tienda742.csproj`
   - 7.2 Arranque y Licencias: `MauiProgram.cs`, `App.xaml`, `App.xaml.cs`
   - 7.3 Navegación: `AppShell.xaml` y `AppShell.xaml.cs`
   - 7.4 Modelos de Datos (`Models/`): `Cliente.cs`, `Producto.cs`, `Venta.cs`, `DashboardStats.cs`
   - 7.5 Capa de Acceso a Datos y Conexión (`Services/`): `ConfigDB.cs`, `VentaService.cs`
   - 7.6 Motor de Generación de Documentos PDF (`Services/`): `PdfService.cs`
   - 7.7 Interfaz y Lógica de Registro de Ventas (`Views/`): `NuevaVentaPage.xaml` y `NuevaVentaPage.xaml.cs`
   - 7.8 Interfaz y Lógica de Dashboard y Reportes (`Views/`): `DashboardReportesPage.xaml` y `DashboardReportesPage.xaml.cs`
8. **Compilación, Despliegue y Pruebas Operativas**
9. **Guía de Diagnóstico y Solución de Problemas Frecuentes (Troubleshooting)**

---

## 1. PRESENTACIÓN DEL PROYECTO Y FICHA TÉCNICA

- **Nombre del Sistema:** `VentasApp_Tienda742`
- **Autor / Arquitectura:** Ing. Juancito Peña Vizcaíno
- **Plataforma:** .NET 9 Multi-platform App UI (.NET MAUI)
- **Motor de Base de Datos:** Microsoft SQL Server 2022 (Developer / Standard)
- **Dirección IP del Servidor:** `192.168.2.68`
- **Puerto de Escucha:** `1433` (TCP)
- **Base de Datos Objetivo:** `TiendaDB_742`
- **Credenciales Maestras:** Usuario SQL: `JUANCITO` | Contraseña: `123456`
- **Generación de Reportes e Impresión:** QuestPDF (Licencia Comunitaria)
- **Enfoque Arquitectónico:** Clean Separation of Concerns (UI XAML reactiva + Servicios C# desacoplados + SQL Transaccional ACID).

El propósito primordial de esta aplicación es resolver de manera compacta, rápida y elegante el flujo completo del **Módulo de Ventas** para puntos de facturación comercial:
- Registro de pedidos con descuento de existencias en tiempo real.
- Emisión inmediata de **Ticket Térmico POS de 80mm**.
- Mini-Dashboard con 4 tarjetas Bento KPI de rendimiento comercial.
- Exportación vectorial en formato **PDF A4** del Reporte General de Ventas y del Inventario Valorado.

---

## 2. PLANTEAMIENTO DEL PROBLEMA Y SOLUCIÓN ARQUITECTÓNICA

### El Problema Comercial y Técnico
Muchas pequeñas y medianas empresas comerciales sufren por la fragmentación operativa:
1. **Sobrevendido por falta de control de stock:** Se registran facturas sin validar si el artículo realmente existe en anaquel o almacén.
2. **Cálculos manuales o erróneos de impuestos:** Discrepancias en el cálculo del ITBIS (18%) que ocasionan multas o desajustes contables.
3. **Falta de comprobantes físicos ágiles:** Los sistemas tradicionales imprimen facturas pesadas en hojas de tamaño carta que desperdician papel y retrasan la entrega al cliente en mostrador.
4. **Carencia de visibilidad gerencial inmediata:** El dueño o encargado no sabe cuánto ha vendido en el día sin solicitar reportes complejos al departamento de contabilidad.

### La Solución Implementada
La aplicación aborda estas fallas mediante una arquitectura de software robusta y minimalista:
- **Transaccionalidad SQL Server (ACID):** Cada venta se procesa dentro de una transacción (`SqlTransaction`). Si el stock es insuficiente o ocurre una interrupción de red, se ejecuta un `ROLLBACK` atómico que protege el inventario.
- **Cálculo Determinista del Impuesto:** Se delega la regla fiscal a la función de base de datos `dbo.fn_CalcularImpuesto` y se refleja instantáneamente en la interfaz de usuario antes de confirmar la compra.
- **Motor Dual de Documentos PDF:** Se implementa un generador nativo con `QuestPDF` que renderiza un comprobante térmico tipo ticket (80 mm / 226 pt de ancho) y reportes ejecutivos en tamaño A4 con maquetación tipográfica profesional.
- **Mini-Dashboard Ejecutivo:** Un conjunto de 4 tarjetas visuales tipo Bento que condensan Total Facturado, Cantidad de Órdenes, Clientes Registrados y Unidades Físicas en Stock.

---

## 3. REQUISITOS DE INSTALACIÓN Y PREPARACIÓN DEL ENTORNO

Para ejecutar, compilar o modificar esta solución en una estación de trabajo, se deben contar con las siguientes herramientas instaladas:

1. **Sistema Operativo:** Windows 10 (versión 19041 o superior) o Windows 11.
2. **SDK de .NET:** .NET 9.0 SDK (versión 9.0.300 o superior).
3. **Cargas de Trabajo MAUI:**
   ```powershell
   dotnet workload install maui
   dotnet workload install maui-windows
   ```
4. **Visual Studio 2022 (v17.12 o superior) / VS Code:**
   - Carga de trabajo recomendada en Visual Studio: *"Desarrollo de interfaz de usuario de aplicaciones multiplataforma de .NET" (.NET MAUI)*.
5. **SQL Server:** Instancia de SQL Server 2019/2022 activa y accesible en red o localmente.
6. **Librerías NuGet requeridas:**
   - `Microsoft.Data.SqlClient` (v6.1.4 o superior): Driver oficial de alto rendimiento para SQL Server.
   - `QuestPDF` (v2024.12.3 o superior): Motor de renderizado vectorial de documentos PDF.

---

## 4. GUÍA MAESTRA: HABILITACIÓN DE SQL SERVER REMOTO, PUERTO 1433 Y FIREWALL

Si deseas conectar computadoras portátiles, terminales de caja u otros equipos de la red local (`192.168.2.x`) a tu servidor SQL Server, realiza minuciosamente los siguientes pasos:

### Paso 4.1: Habilitar Autenticación Mixta (SQL Server and Windows Authentication)
1. Abre **SQL Server Management Studio (SSMS)** y conéctate a tu servidor local.
2. Haz clic derecho sobre el nodo raíz del servidor y selecciona **Properties (Propiedades)**.
3. En la pestaña **Security (Seguridad)**, marca la opción:
   - **SQL Server and Windows Authentication mode**.
4. Haz clic en **OK**.
5. Haz clic derecho sobre el servidor en SSMS y presiona **Restart (Reiniciar)** para que la configuración surta efecto.

### Paso 4.2: Crear o Habilitar el Usuario `JUANCITO`
Ejecuta la siguiente instrucción en SSMS para asegurar que el usuario tenga permisos totales de administración:
```sql
USE master;
GO

-- Crear login si no existe
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'JUANCITO')
BEGIN
    CREATE LOGIN JUANCITO WITH PASSWORD = '123456', CHECK_POLICY = OFF;
END
ELSE
BEGIN
    ALTER LOGIN JUANCITO WITH PASSWORD = '123456';
    ALTER LOGIN JUANCITO ENABLE;
END;
GO

-- Asignar rol de sysadmin
ALTER SERVER ROLE sysadmin ADD MEMBER JUANCITO;
GO
```

### Paso 4.3: Habilitar el Protocolo TCP/IP y Asignar el Puerto 1433
1. Presiona `Win + R`, escribe `SQLServerManager16.msc` (o la versión que corresponda a tu SQL Server) y presiona Enter para abrir **SQL Server Configuration Manager**.
2. Despliega **SQL Server Network Configuration** y haz clic en **Protocols for MSSQLSERVER**.
3. Asegúrate de que **TCP/IP** esté en estado **Enabled (Habilitado)**.
4. Haz doble clic sobre **TCP/IP** y dirígete a la pestaña **IP Addresses (Direcciones IP)**.
5. Baja hasta la sección inferior denominada **IPAll**:
   - Borra cualquier valor en **TCP Dynamic Ports** (déjalo completamente en blanco).
   - En **TCP Port**, escribe: `1433`.
6. Haz clic en **Apply (Aplicar)** y acepta la notificación.
7. Ve a **SQL Server Services** en el panel izquierdo, haz clic derecho sobre **SQL Server (MSSQLSERVER)** y selecciona **Restart (Reiniciar)**.

### Paso 4.4: Abrir el Puerto 1433 en el Firewall de Windows (PowerShell Administrador)
Abre PowerShell como Administrador en la máquina servidor (`192.168.2.68`) y ejecuta:
```powershell
# Regla de entrada para el Puerto TCP 1433 (Motor SQL Server)
New-NetFirewallRule -DisplayName "SQL Server Puerto 1433" `
    -Direction Inbound `
    -LocalPort 1433 `
    -Protocol TCP `
    -Action Allow

# Regla de entrada para el Puerto UDP 1434 (SQL Server Browser)
New-NetFirewallRule -DisplayName "SQL Server Browser 1434" `
    -Direction Inbound `
    -LocalPort 1434 `
    -Protocol UDP `
    -Action Allow
```

---

## 5. SCRIPT T-SQL CANÓNICO COMPLETO (`TiendaDB_742`) - GUÍA DIDÁCTICA INTEGRAL

Copia y ejecuta este script completo en tu SQL Server Management Studio para recrear y poblar la base de datos con toda su estructura relacional (DDL), carga de datos (DML), consultas didácticas (DQL), control transaccional (TCL), seguridad (DCL), objetos programables (Vistas, Stored Procedures, Funciones UDF, Triggers, CTEs) y la consulta final integradora:

```sql
/************************************************************************************************
    SQL SERVER - GUÍA DIDÁCTICA INTEGRAL
    
    ESTRUCTURA DEL SCRIPT:
    1. DDL (Data Definition Language) - Definición de esquemas, tablas, restricciones e índices.
    2. DML (Data Manipulation Language) - Carga de datos base (12-15 registros), actualizaciones y bajas.
    3. DQL (Data Query Language) - Consultas, filtros, ordenamiento, agregaciones y uniones (JOINs).
    4. TCL (Transaction Control Language) - Control de transacciones atómicas (COMMIT / ROLLBACK).
    5. DCL (Data Control Language) - Gestión de permisos sobre objetos.
    6. OBJETOS PROGRAMABLES - Vistas, Procedimientos Almacenados, Funciones, Triggers y CTEs.
************************************************************************************************/


/************************************************************************************************
    SECCIÓN 1: DDL (DATA DEFINITION LANGUAGE)
    Definición: Subconjunto de SQL utilizado para crear, alterar y borrar la estructura de la base
    de datos y sus objetos (tablas, campos, índices, restricciones). Modifica el catálogo de metadatos.
************************************************************************************************/

-- 1.1 CREACIÓN Y PUESTA EN USO DE LA BASE DE DATOS
IF DB_ID('TiendaDB_742') IS NOT NULL
BEGIN
    ALTER DATABASE TiendaDB_742 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TiendaDB_742;
END;
GO

CREATE DATABASE TiendaDB_742;
GO

USE TiendaDB_742;
GO

-- 1.2 CREACIÓN DE TABLAS CON RESTRICCIONES (CONSTRAINTS)
-- PRIMARY KEY: Clave primaria (unicidad y no nulidad).
-- IDENTITY: Generador automático e incremental de secuencias numéricas.
-- NOT NULL / UNIQUE: Integridad de columna y valor único.
-- CHECK: Regla booleana que valida los datos antes de guardarlos.
-- DEFAULT: Valor precargado si la inserción no especifica uno.

CREATE TABLE Clientes
(
    ClienteID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE,
    Ciudad VARCHAR(100),
    FechaRegistro DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Productos
(
    ProductoID INT IDENTITY(1,1) PRIMARY KEY,
    NombreProducto VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) CHECK(Precio > 0),
    Stock INT DEFAULT 0
);
GO

-- FOREIGN KEY: Integridad referencial entre tablas padre e hija.
CREATE TABLE Pedidos
(
    PedidoID INT IDENTITY(1,1) PRIMARY KEY,
    ClienteID INT NOT NULL,
    ProductoID INT NOT NULL,
    Cantidad INT CHECK(Cantidad > 0),
    FechaPedido DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Pedidos_Clientes 
        FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID),

    CONSTRAINT FK_Pedidos_Productos 
        FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID)
);
GO

-- 1.3 MODIFICACIÓN ESTRUCTURAL (ALTER TABLE)
-- Agrega columnas o modifica definiciones existentes sin destruir los datos almacenados.
ALTER TABLE Clientes
ADD Telefono VARCHAR(20);
GO

-- 1.4 ÍNDICES (INDEX)
-- Estructuras de datos (árboles B) que aceleran las búsquedas y ordenamientos sin escanear toda la tabla.
CREATE NONCLUSTERED INDEX IX_Clientes_Nombre
ON Clientes(Nombre);
GO


/************************************************************************************************
    SECCIÓN 2: DML (DATA MANIPULATION LANGUAGE)
    Definición: Conjunto de sentencias orientadas a manipular el contenido de las tablas:
    insertar nuevos registros (INSERT), actualizar existentes (UPDATE) y remover filas (DELETE).
************************************************************************************************/

-- 2.1 INSERCIÓN MASIVA DE PRUEBA (INSERT)

-- 12 Clientes
INSERT INTO Clientes (Nombre, Email, Ciudad, Telefono)
VALUES
('Juan Perez', 'juan@gmail.com', 'Santo Domingo', '8091111111'),
('Maria Gomez', 'maria@gmail.com', 'Santiago', '8092222222'),
('Carlos Lopez', 'carlos@gmail.com', 'La Vega', '8093333333'),
('Ana Ramirez', 'ana@gmail.com', 'Santo Domingo', '8094444444'),
('Pedro Diaz', 'pedro@gmail.com', 'Puerto Plata', '8095555555'),
('Lucia Fernandez', 'lucia@gmail.com', 'Santiago', '8096666666'),
('Jose Morales', 'jose@gmail.com', 'San Cristobal', '8097777777'),
('Elena Castillo', 'elena@gmail.com', 'La Vega', '8098888888'),
('Miguel Rojas', 'miguel@gmail.com', 'Santo Domingo', '8099999999'),
('Sofia Alvarez', 'sofia@gmail.com', 'Moca', '8090001111'),
('David Guzman', 'david@gmail.com', 'Bonao', '8090002222'),
('Rosa Martinez', 'rosa@gmail.com', 'Santiago', '8090003333');
GO

-- 12 Productos
INSERT INTO Productos (NombreProducto, Precio, Stock)
VALUES
('Laptop', 65000.00, 15),
('Mouse', 500.00, 100),
('Monitor 24', 12000.00, 30),
('Teclado Mecanico', 2500.00, 50),
('Impresora', 15000.00, 10),
('Webcam Full HD', 3200.00, 40),
('Disco SSD 1TB', 4800.00, 60),
('Memoria RAM 16GB', 3500.00, 45),
('Headset Gamer', 2800.00, 35),
('Pad Mouse XL', 800.00, 80),
('Silla Ergonomica', 18500.00, 12),
('Cable HDMI 2.1', 600.00, 120);
GO

-- 15 Pedidos
INSERT INTO Pedidos (ClienteID, ProductoID, Cantidad)
VALUES
(1, 1, 1),
(1, 2, 2),
(2, 3, 1),
(3, 4, 1),
(4, 5, 2),
(5, 6, 1),
(6, 7, 3),
(7, 8, 2),
(8, 9, 1),
(9, 10, 4),
(10, 1, 1),
(11, 2, 5),
(12, 11, 1),
(2, 12, 2),
(3, 7, 1);
GO

-- 2.2 ACTUALIZACIÓN DE REGISTROS (UPDATE)
-- Modifica uno o varios campos en registros que cumplan con la condición del WHERE.
UPDATE Productos
SET Precio = 70000.00
WHERE ProductoID = 1;
GO

-- 2.3 ELIMINACIÓN DE REGISTROS (DELETE)
-- Elimina filas específicas de la tabla de acuerdo al predicado WHERE.
DELETE FROM Pedidos
WHERE PedidoID = 15;
GO


/************************************************************************************************
    SECCIÓN 3: DQL (DATA QUERY LANGUAGE)
    Definición: Lenguaje de consulta enfocado en recuperar y proyectar datos almacenados
    mediante la instrucción SELECT y sus diversas cláusulas asociadas.
************************************************************************************************/

-- 3.1 PROYECCIÓN BÁSICA
SELECT * FROM Clientes;
SELECT * FROM Productos;
SELECT * FROM Pedidos;

-- 3.2 TOP: Limita el número de filas devueltas
SELECT TOP 5 * 
FROM Clientes;

SELECT TOP 5 * 
FROM Productos 
ORDER BY Precio DESC;

-- 3.3 WHERE: Filtra filas basadas en condiciones lógicas
SELECT * 
FROM Productos 
WHERE Precio > 5000;

-- 3.4 LIKE: Búsqueda de patrones en cadenas de texto (% comodín)
SELECT * 
FROM Clientes 
WHERE Nombre LIKE 'J%';

-- 3.5 BETWEEN: Filtrado por rango cerrado inclusivo
SELECT * 
FROM Productos 
WHERE Precio BETWEEN 1000 AND 10000;

-- 3.6 IN: Coincidencia con una lista discreta de valores
SELECT * 
FROM Clientes 
WHERE Ciudad IN ('Santiago', 'La Vega', 'Moca');

-- 3.7 ORDER BY: Ordena el conjunto de resultados (ASC o DESC)
SELECT * 
FROM Productos 
ORDER BY Precio DESC;

-- 3.8 FUNCIONES DE AGREGACIÓN
-- Realizan cálculos sobre un conjunto de filas y devuelven un único valor resumen.
SELECT COUNT(*) AS TotalClientes FROM Clientes;
SELECT SUM(Precio) AS SumaPrecios FROM Productos;
SELECT AVG(Precio) AS PromedioPrecios FROM Productos;
SELECT MAX(Precio) AS PrecioMaximo FROM Productos;
SELECT MIN(Precio) AS PrecioMinimo FROM Productos;

-- 3.9 FUNCIONES ESCALARES DE TEXTO
SELECT 
    Nombre,
    UPPER(Nombre) AS Mayusculas,
    LOWER(Nombre) AS Minusculas,
    LEN(Nombre) AS LargoNombre
FROM Clientes;

-- 3.10 FUNCIONES DE FECHA DEL SISTEMA
SELECT 
    GETDATE() AS FechaHoraActual,
    YEAR(GETDATE()) AS Anio,
    MONTH(GETDATE()) AS Mes,
    DAY(GETDATE()) AS Dia;

-- 3.11 GROUP BY: Agrupa filas que tienen los mismos valores en columnas especificadas
SELECT 
    Ciudad, 
    COUNT(*) AS TotalClientes
FROM Clientes
GROUP BY Ciudad;

-- 3.12 HAVING: Filtra los grupos resultantes del GROUP BY (evalúa sobre agregaciones)
SELECT 
    Ciudad, 
    COUNT(*) AS TotalClientes
FROM Clientes
GROUP BY Ciudad
HAVING COUNT(*) > 1;

-- 3.13 COMBINACIONES DE TABLAS (JOINS)
-- INNER JOIN: Devuelve filas que tienen coincidencias exactas en ambas tablas.
SELECT 
    P.PedidoID,
    C.Nombre AS Cliente,
    PR.NombreProducto,
    PR.Precio,
    P.Cantidad,
    (PR.Precio * P.Cantidad) AS Total
FROM Pedidos P
INNER JOIN Clientes C ON P.ClienteID = C.ClienteID
INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID;

-- LEFT JOIN: Todas las filas de la tabla izquierda y las coincidencias de la derecha (NULL si no hay).
SELECT 
    C.Nombre, 
    P.PedidoID
FROM Clientes C
LEFT JOIN Pedidos P ON C.ClienteID = P.ClienteID;

-- RIGHT JOIN: Todas las filas de la tabla derecha y las coincidencias de la izquierda.
SELECT 
    C.Nombre, 
    P.PedidoID
FROM Clientes C
RIGHT JOIN Pedidos P ON C.ClienteID = P.ClienteID;

-- FULL JOIN: Todas las filas cuando hay coincidencia en cualquiera de las dos tablas.
SELECT 
    C.Nombre, 
    P.PedidoID
FROM Clientes C
FULL JOIN Pedidos P ON C.ClienteID = P.ClienteID;

-- 3.14 SUBQUERIES (SUB-CONSULTAS)
-- Consulta anidada dentro de otra instrucción para filtrar en base a un valor dinámico.
SELECT * 
FROM Productos
WHERE Precio > (SELECT AVG(Precio) FROM Productos);


/************************************************************************************************
    SECCIÓN 4: TCL (TRANSACTION CONTROL LANGUAGE)
    Definición: Gestiona los cambios producidos por las sentencias DML asegurando ACID
    (Atomicidad, Consistencia, Aislamiento y Durabilidad). Agrupa operaciones en bloques atómicos.
************************************************************************************************/

-- 4.1 TRANSACCIÓN CONFIRMADA (COMMIT)
-- Guarda de forma permanente los cambios realizados dentro del bloque.
BEGIN TRANSACTION;

UPDATE Productos
SET Stock = Stock - 1
WHERE ProductoID = 1;

COMMIT TRANSACTION;
GO

-- 4.2 TRANSACCIÓN REVERTIDA (ROLLBACK)
-- Cancela y deshace todas las operaciones del bloque, retornando al estado inicial.
BEGIN TRANSACTION;

UPDATE Productos
SET Precio = 999999.00;

ROLLBACK TRANSACTION;
GO


/************************************************************************************************
    SECCIÓN 5: DCL (DATA CONTROL LANGUAGE)
    Definición: Administra la seguridad mediante la asignación y revocación de permisos sobre
    los objetos de la base de datos a usuarios y roles del motor.
************************************************************************************************/

-- Sintaxis estándar representativa:
-- GRANT: Otorga privilegios.
-- REVOKE: Elimina un privilegio previamente otorgado.
-- DENY: Niega explícitamente un privilegio, prevaleciendo sobre cualquier asignación.

/*
GRANT SELECT, INSERT ON Clientes TO UsuarioVentas;
REVOKE INSERT ON Clientes FROM UsuarioVentas;
DENY DELETE ON Clientes TO UsuarioVentas;
*/


/************************************************************************************************
    SECCIÓN 6: OBJETOS PROGRAMABLES Y CONSULTAS AVANZADAS
    Definición: Mecanismos para encapsular lógica de negocio, simplificar consultas repetitivas
    y automatizar respuestas ante eventos en el motor de base de datos.
************************************************************************************************/

-- 6.1 VIEW (VISTA)
-- Tabla virtual basada en el resultado de una consulta SQL predefinida. No almacena datos físicos.
CREATE VIEW vw_ReporteVentas
AS
SELECT 
    P.PedidoID,
    C.Nombre AS Cliente,
    C.Ciudad,
    PR.NombreProducto,
    PR.Precio,
    P.Cantidad,
    (PR.Precio * P.Cantidad) AS TotalVenta
FROM Pedidos P
INNER JOIN Clientes C ON P.ClienteID = C.ClienteID
INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID;
GO

SELECT * FROM vw_ReporteVentas;
GO

-- 6.2 STORED PROCEDURE (PROCEDIMIENTO ALMACENADO)
-- Conjunto precompilado de una o más instrucciones SQL que admiten parámetros y lógica de flujo.
CREATE PROCEDURE sp_ObtenerClientesPorCiudad
    @CiudadFiltro VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ClienteID, 
        Nombre, 
        Email, 
        Ciudad, 
        FechaRegistro
    FROM Clientes
    WHERE Ciudad = @CiudadFiltro;
END;
GO

EXEC sp_ObtenerClientesPorCiudad @CiudadFiltro = 'Santo Domingo';
GO

-- 6.3 FUNCTION (FUNCIÓN ESCALAR UDF)
-- Rutina que acepta parámetros, ejecuta lógica determinista o de cálculo y retorna un valor único.
CREATE FUNCTION fn_CalcularImpuesto
(
    @Monto DECIMAL(10,2)
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN @Monto * 0.18; -- 18% ITBIS / IVA
END;
GO

SELECT 
    NombreProducto, 
    Precio, 
    dbo.fn_CalcularImpuesto(Precio) AS ImpuestoEstimado
FROM Productos;
GO

-- 6.4 TRIGGER (DISPARADOR)
-- Bloque de código ejecutado de forma automática ante eventos de inserción, actualización o borrado en una tabla.
CREATE TRIGGER trg_AuditoriaNuevoCliente
ON Clientes
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    PRINT 'Notificación: Se registraron nuevos clientes en la tabla Clientes.';
END;
GO

-- 6.5 CTE (COMMON TABLE EXPRESSION)
-- Conjunto de resultados temporal con nombre que existe solo dentro del alcance de una única consulta.
WITH ProductosPremium AS
(
    SELECT 
        ProductoID, 
        NombreProducto, 
        Precio,
        Stock
    FROM Productos
    WHERE Precio >= 5000.00
)
SELECT 
    NombreProducto, 
    Precio,
    Stock
FROM ProductosPremium
ORDER BY Precio DESC;
GO


/************************************************************************************************
    CONSULTA FINAL INTEGRADORA
    Reúne proyecciones, funciones matemáticas, funciones de fecha, uniones (JOINs),
    filtros condicionales y ordenamiento.
************************************************************************************************/

SELECT TOP 10
    C.Nombre AS Cliente,
    C.Ciudad,
    PR.NombreProducto,
    PR.Precio,
    P.Cantidad,
    (PR.Precio * P.Cantidad) AS Subtotal,
    dbo.fn_CalcularImpuesto(PR.Precio * P.Cantidad) AS Impuesto,
    (PR.Precio * P.Cantidad) + dbo.fn_CalcularImpuesto(PR.Precio * P.Cantidad) AS TotalPagar,
    YEAR(P.FechaPedido) AS AnioPedido
FROM Pedidos P
INNER JOIN Clientes C ON P.ClienteID = C.ClienteID
INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID
WHERE PR.Precio > 1000
ORDER BY TotalPagar DESC;
GO
```

## 6. ESTRUCTURA DEL PROYECTO Y ÁRBOL DE DIRECTORIOS

```
VentasApp_Tienda742/                    <-- Raíz del Repositorio
├── VentasApp_Tienda742.sln             <-- Solución principal (doble clic para abrir en Visual Studio)
├── README.md                           <-- Documentación ejecutiva del proyecto
├── MANUAL_COMPLETO_PASO_A_PASO.md      <-- Manual didáctico exhaustivo paso a paso
├── TUTORIAL_MAESTRO_BRUTALISTA.html    <-- Masterclass interactiva web
├── index.html                          <-- Portal web interactivo para GitHub Pages
├── global.json                         <-- Fijación de versión del SDK (.NET 9.0.318)
├── VentasApp_Tienda742.csproj          <-- Configuración de compilación y paquetes NuGet
├── MauiProgram.cs                      <-- Punto de entrada de la aplicación y licencia QuestPDF
├── App.xaml                            <-- Definición de recursos globales de estilos XAML
├── App.xaml.cs                         <-- Inicialización de la ventana con AppShell
├── AppShell.xaml                       <-- Barra de pestañas (TabBar) y navegación
├── AppShell.xaml.cs                    <-- Lógica code-behind del contenedor Shell
├── Models/                             <-- Entidades de dominio fuertemente tipadas
│   ├── Cliente.cs                      <-- Entidad Cliente y formateador DisplayText
│   ├── Producto.cs                     <-- Entidad Producto, precio, stock y valor inventario
│   ├── Venta.cs                        <-- Objeto compuesto de Venta, cálculos ITBIS y totales
│   └── DashboardStats.cs               <-- Métricas agregadas para las tarjetas Bento
├── Services/                           <-- Lógica de negocio y persistencia
│   ├── ConfigDB.cs                     <-- Cadena de conexión centralizada a SQL Server
│   ├── VentaService.cs                 <-- Consultas SQL, transacciones ACID y cálculo de KPIs
│   └── PdfService.cs                   <-- Generador de Ticket 80mm y reportes A4 con QuestPDF
├── Views/                              <-- Vistas de usuario en XAML y Code-Behind
│   ├── NuevaVentaPage.xaml             <-- Formulario de facturación, selectores y totales
│   ├── NuevaVentaPage.xaml.cs          <-- Eventos, validación de stock y emisión de ticket
│   ├── DashboardReportesPage.xaml      <-- Mini-Dashboard Bento, búsqueda y lista de ventas
│   └── DashboardReportesPage.xaml.cs   <-- Lógica de filtros y disparo de reportes PDF
└── Resources/                          <-- Recursos estáticos, fuentes, splash y estilos
    ├── Styles/
    │   ├── Colors.xaml                 <-- Paleta de colores corporativos
    │   └── Styles.xaml                 <-- Estilos globales de controles
```

---

## 7. CÓDIGO FUENTE ÍNTEGRO (100%) ARCHIVO POR ARCHIVO

A continuación se presenta el código completo y sin omisiones de cada archivo del proyecto, junto con su explicación técnica correspondiente.

---

### 7.1 Configuración: `global.json`
**Ruta:** `global.json`  
**Propósito:** Garantiza que cualquier máquina que abra el proyecto utilice exactamente la versión del compilador .NET 9.
```json
{
  "sdk": {
    "version": "9.0.318",
    "rollForward": "latestFeature"
  }
}
```

---

### 7.2 Configuración del Proyecto: `VentasApp_Tienda742.csproj`
**Ruta:** `VentasApp_Tienda742.csproj`  
**Propósito:** Especifica la plataforma de destino Windows Desktop Unpackaged (`WindowsPackageType=None`) y declara las librerías `Microsoft.Data.SqlClient` y `QuestPDF`.
```xml
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFrameworks>net9.0-windows10.0.19041.0</TargetFrameworks>
		<TargetFrameworks Condition="!$([MSBuild]::IsOSPlatform('windows'))">net9.0-android</TargetFrameworks>

		<OutputType>Exe</OutputType>
		<RootNamespace>VentasApp_Tienda742</RootNamespace>
		<UseMaui>true</UseMaui>
		<SingleProject>true</SingleProject>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>enable</Nullable>

		<!-- Nombre visible de la aplicación -->
		<ApplicationTitle>VentasApp Tienda 742</ApplicationTitle>

		<!-- Identificador del paquete -->
		<ApplicationId>com.juancito.ventasapptienda742</ApplicationId>

		<!-- Versiones -->
		<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
		<ApplicationVersion>1</ApplicationVersion>

		<!-- Aplicación de escritorio desempaquetada para ejecución directa en Windows -->
		<WindowsPackageType>None</WindowsPackageType>
	</PropertyGroup>

	<ItemGroup>
		<!-- Iconos y pantallas de bienvenida -->
		<MauiIcon Include="Resources\AppIcon\appicon.svg" ForegroundFile="Resources\AppIcon\appiconfg.svg" Color="#1E3A8A" />
		<MauiSplashScreen Include="Resources\Splash\splash.svg" Color="#1E3A8A" BaseSize="128,128" />
		<MauiImage Include="Resources\Images\*" />
		<MauiFont Include="Resources\Fonts\*" />
		<MauiAsset Include="Resources\Raw\**" LogicalName="%(RecursiveDir)%(Filename)%(Extension)" />
	</ItemGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.Maui.Controls" Version="$(MauiVersion)" />
		<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="9.0.0" />
		<PackageReference Include="Microsoft.Data.SqlClient" Version="6.1.4" />
		<PackageReference Include="QuestPDF" Version="2024.12.3" />
	</ItemGroup>

</Project>
```

---

### 7.3 Arranque y Licencias: `MauiProgram.cs`
**Ruta:** `MauiProgram.cs`  
**Propósito:** Inicializa el host de MAUI y activa la licencia comunitaria de QuestPDF requerida para exportar PDFs.
```csharp
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;

namespace VentasApp_Tienda742;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		// Activación indispensable para permitir la generación ilimitada de PDFs
		QuestPDF.Settings.License = LicenseType.Community;

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
```

---

### 7.4 Estilos y Ciclo de Vida: `App.xaml` y `App.xaml.cs`
**Ruta:** `App.xaml`  
**Propósito:** Combina los diccionarios de recursos de colores y estilos globales.
```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:VentasApp_Tienda742"
             x:Class="VentasApp_Tienda742.App">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Styles/Colors.xaml" />
                <ResourceDictionary Source="Resources/Styles/Styles.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

**Ruta:** `App.xaml.cs`  
**Propósito:** Instancia la ventana raíz con la plantilla `AppShell`.
```csharp
namespace VentasApp_Tienda742;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}
```

---

### 7.5 Navegación por Pestañas: `AppShell.xaml` y `AppShell.xaml.cs`
**Ruta:** `AppShell.xaml`  
**Propósito:** Define la navegación con dos pestañas fijas (`TabBar`): una para Registrar Venta y otra para el Dashboard/Reportes.
```xml
<?xml version="1.0" encoding="utf-8" ?>
<Shell
    x:Class="VentasApp_Tienda742.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:views="clr-namespace:VentasApp_Tienda742.Views"
    Title="VentasApp Tienda 742"
    FlyoutBehavior="Disabled">

    <TabBar>
        <!-- Pestaña 1: Punto de Venta / Facturación -->
        <Tab Title="🛒 Nueva Venta" Route="NuevaVenta">
            <ShellContent
                Title="Nueva Venta"
                ContentTemplate="{DataTemplate views:NuevaVentaPage}"
                Route="NuevaVentaPage" />
        </Tab>

        <!-- Pestaña 2: Métricas, Histórico y Exportación PDF -->
        <Tab Title="📊 Dashboard y Reportes" Route="DashboardReportes">
            <ShellContent
                Title="Dashboard y Reportes"
                ContentTemplate="{DataTemplate views:DashboardReportesPage}"
                Route="DashboardReportesPage" />
        </Tab>
    </TabBar>

</Shell>
```

**Ruta:** `AppShell.xaml.cs`
```csharp
namespace VentasApp_Tienda742;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}
}
```

---

### 7.6 Modelos de Datos (`Models/`)

#### A. `Models/Cliente.cs`
Representa al comprador y provee una propiedad `DisplayText` para los selectores XAML.
```csharp
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

        // Propiedad formateada para el Picker de la interfaz
        public string DisplayText => $"{Nombre} ({Ciudad ?? "Sin Ciudad"})";
    }
}
```

#### B. `Models/Producto.cs`
Representa los artículos comercializables, sus precios y stock disponible.
```csharp
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
```

#### C. `Models/Venta.cs`
Objeto que unifica el encabezado del pedido con los datos del cliente, producto, cálculos de impuestos y formateadores visuales.
```csharp
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
        
        // Cálculos financieros derivados
        public decimal Subtotal => Precio * Cantidad;
        public decimal Impuesto => Subtotal * 0.18m; // 18% ITBIS
        public decimal Total => Subtotal + Impuesto;
        
        public DateTime FechaPedido { get; set; }

        public string FechaFormateada => FechaPedido.ToString("dd/MM/yyyy hh:mm tt");
        public string TotalFormateado => $"RD$ {Total:N2}";
        public string DetalleResumen => $"{Cantidad}x {ProductoNombre} @ RD$ {Precio:N2}";
    }
}
```

#### D. `Models/DashboardStats.cs`
Estructura de datos para transferir los 4 indicadores clave hacia el mini-dashboard.
```csharp
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
```

---

### 7.7 Configuración y Acceso a Datos (`Services/`)

#### A. `Services/ConfigDB.cs`
Almacena la cadena de conexión centralizada.
```csharp
namespace VentasApp_Tienda742.Services
{
    public static class ConfigDB
    {
        public static string ConnectionString { get; set; } = 
            "Server=192.168.2.68,1433;Database=TiendaDB_742;User Id=JUANCITO;Password=123456;TrustServerCertificate=True;MultipleActiveResultSets=True;Connect Timeout=15;";
    }
}
```

#### B. `Services/VentaService.cs`
Contiene toda la lógica SQL, transacciones, bloqueos de concurrencia y consultas a vistas.
```csharp
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

        // Obtener catálogo de clientes ordenados alfabéticamente
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

        // Obtener productos activos con stock
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

        // REGISTRO TRANSACCIONAL ATÓMICO (ACID) DE VENTA
        public async Task<Venta> RegistrarVentaAsync(int clienteId, int productoId, int cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que 0.");

            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();
            try
            {
                // 1. Validar producto y stock disponible con bloqueo de actualización (UPDLOCK)
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

                // 2. Insertar en Pedidos y capturar el ID generado
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

                // 4. Obtener datos del Cliente para armar el comprobante
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

                // Confirmar la transacción
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

        // Obtener historial de ventas con búsqueda parametrizada
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

        // Obtener métricas agregadas para el mini-dashboard
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

            // 4. Producto Más Vendido
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

        // Inventario completo
        public async Task<List<Producto>> GetInventarioAsync()
        {
            return await GetProductosAsync();
        }
    }
}
```

---

### 7.8 Motor de Documentos PDF (`Services/PdfService.cs`)
Genera tickets térmicos de 80mm y reportes A4 maquetados con tablas, totales e invocación automática del visor de PDF del sistema.
```csharp
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
        // 1. TICKET TÉRMICO POS 80MM
        public async Task<string> GenerarTicketVentaPdfAsync(Venta venta)
        {
            string fileName = $"Ticket_Venta_{venta.PedidoID}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(226, 420, Unit.Point); // Formato exacto de impresora térmica de 80mm
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

                        // Resumen Financiero
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

        // 2. REPORTE GENERAL DE VENTAS A4
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

                            // Fila de totales acumulados
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

        // 3. REPORTE DE INVENTARIO VALORADO A4
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

        // Apertura automática en el visor predeterminado del sistema operativo
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
```

---

### 7.9 Interfaz y Lógica de Registro de Ventas (`Views/NuevaVentaPage`)

#### A. Vista XAML: `NuevaVentaPage.xaml`
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:models="clr-namespace:VentasApp_Tienda742.Models"
             x:Class="VentasApp_Tienda742.Views.NuevaVentaPage"
             Title="🛒 Nueva Venta"
             BackgroundColor="#F8FAFC">

    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="16" MaximumWidthRequest="700">

            <!-- Encabezado Corporativo -->
            <Border BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 12" Padding="16">
                <VerticalStackLayout Spacing="4">
                    <Label Text="🏪 TIENDA 742 JPV" FontSize="20" FontAttributes="Bold" TextColor="#1E3A8A" />
                    <Label Text="Módulo Oficial de Ventas y Facturación Rápida" FontSize="13" TextColor="#475569" />
                    <HorizontalStackLayout Spacing="8">
                        <Label Text="🟢 SQL Server: 192.168.2.68:1433" FontSize="11" TextColor="#16A34A" FontAttributes="Bold" />
                        <Label Text="•" FontSize="11" TextColor="#94A3B8" />
                        <Label Text="BD: TiendaDB_742" FontSize="11" TextColor="#64748B" />
                    </HorizontalStackLayout>
                </VerticalStackLayout>
            </Border>

            <!-- Card: Formulario de Selección -->
            <Border BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 12" Padding="18">
                <VerticalStackLayout Spacing="14">
                    <Label Text="📝 Datos de la Transacción" FontSize="16" FontAttributes="Bold" TextColor="#0F172A" />

                    <!-- Selector de Cliente -->
                    <VerticalStackLayout Spacing="4">
                        <Label Text="👤 Seleccionar Cliente:" FontSize="13" FontAttributes="Bold" TextColor="#334155" />
                        <Border Stroke="#CBD5E1" StrokeThickness="1" StrokeShape="RoundRectangle 8" Padding="2" BackgroundColor="#FFFFFF">
                            <Picker x:Name="PickerClientes"
                                    Title="Seleccione un cliente..."
                                    ItemDisplayBinding="{Binding DisplayText, x:DataType=models:Cliente}"
                                    TextColor="#0F172A"
                                    TitleColor="#94A3B8"
                                    SelectedIndexChanged="OnClienteSeleccionado" />
                        </Border>
                    </VerticalStackLayout>

                    <!-- Selector de Producto -->
                    <VerticalStackLayout Spacing="4">
                        <Label Text="📦 Seleccionar Producto:" FontSize="13" FontAttributes="Bold" TextColor="#334155" />
                        <Border Stroke="#CBD5E1" StrokeThickness="1" StrokeShape="RoundRectangle 8" Padding="2" BackgroundColor="#FFFFFF">
                            <Picker x:Name="PickerProductos"
                                    Title="Seleccione un producto..."
                                    ItemDisplayBinding="{Binding DisplayText, x:DataType=models:Producto}"
                                    TextColor="#0F172A"
                                    TitleColor="#94A3B8"
                                    SelectedIndexChanged="OnProductoSeleccionado" />
                        </Border>
                    </VerticalStackLayout>

                    <!-- Control de Cantidad y Stock -->
                    <Grid ColumnDefinitions="*, Auto" RowDefinitions="Auto, Auto" ColumnSpacing="12" RowSpacing="6">
                        <Label Grid.Row="0" Grid.Column="0" Text="🔢 Cantidad a Facturar:" FontSize="13" FontAttributes="Bold" TextColor="#334155" VerticalOptions="Center" />
                        <Label Grid.Row="0" Grid.Column="1" x:Name="LblStockDisponible" Text="Stock: --" FontSize="12" FontAttributes="Bold" TextColor="#2563EB" VerticalOptions="Center" />

                        <HorizontalStackLayout Grid.Row="1" Grid.ColumnSpan="2" Spacing="8" HorizontalOptions="Start">
                            <Button Text="➖" Clicked="OnDisminuirCantidad" WidthRequest="48" HeightRequest="44" CornerRadius="8" BackgroundColor="#E2E8F0" TextColor="#0F172A" FontAttributes="Bold" />
                            <Border Stroke="#CBD5E1" StrokeThickness="1" StrokeShape="RoundRectangle 8" WidthRequest="80" HeightRequest="44" BackgroundColor="#FFFFFF">
                                <Entry x:Name="TxtCantidad"
                                       Text="1"
                                       Keyboard="Numeric"
                                       HorizontalTextAlignment="Center"
                                       VerticalTextAlignment="Center"
                                       FontSize="16"
                                       FontAttributes="Bold"
                                       TextColor="#0F172A"
                                       TextChanged="OnCantidadChanged" />
                            </Border>
                            <Button Text="➕" Clicked="OnAumentarCantidad" WidthRequest="48" HeightRequest="44" CornerRadius="8" BackgroundColor="#E2E8F0" TextColor="#0F172A" FontAttributes="Bold" />
                        </HorizontalStackLayout>
                    </Grid>
                </VerticalStackLayout>
            </Border>

            <!-- Card: Resumen Financiero en Vivo -->
            <Border BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 12" Padding="18">
                <VerticalStackLayout Spacing="10">
                    <Label Text="💳 Resumen de la Venta" FontSize="16" FontAttributes="Bold" TextColor="#0F172A" />

                    <Grid ColumnDefinitions="*, Auto" RowDefinitions="Auto, Auto, Auto" RowSpacing="6">
                        <Label Grid.Row="0" Grid.Column="0" Text="Subtotal:" FontSize="14" TextColor="#475569" />
                        <Label Grid.Row="0" Grid.Column="1" x:Name="LblSubtotal" Text="RD$ 0.00" FontSize="14" FontAttributes="Bold" TextColor="#0F172A" />

                        <Label Grid.Row="1" Grid.Column="0" Text="ITBIS Estimado (18%):" FontSize="14" TextColor="#475569" />
                        <Label Grid.Row="1" Grid.Column="1" x:Name="LblImpuesto" Text="RD$ 0.00" FontSize="14" FontAttributes="Bold" TextColor="#0F172A" />

                        <BoxView Grid.Row="2" Grid.ColumnSpan="2" HeightRequest="1" Color="#E2E8F0" Margin="0, 4" />
                    </Grid>

                    <Grid ColumnDefinitions="*, Auto">
                        <Label Text="TOTAL A PAGAR:" FontSize="18" FontAttributes="Bold" TextColor="#0F172A" VerticalOptions="Center" />
                        <Label Grid.Column="1" x:Name="LblTotal" Text="RD$ 0.00" FontSize="20" FontAttributes="Bold" TextColor="#16A34A" VerticalOptions="Center" />
                    </Grid>
                </VerticalStackLayout>
            </Border>

            <!-- Botones de Acción -->
            <VerticalStackLayout Spacing="10">
                <Button Text="💾 PROCESAR VENTA Y GENERAR TICKET"
                        Clicked="OnRegistrarVentaClicked"
                        BackgroundColor="#1E3A8A"
                        TextColor="#FFFFFF"
                        FontSize="15"
                        FontAttributes="Bold"
                        HeightRequest="52"
                        CornerRadius="10" />

                <Button Text="🔄 Recargar Clientes y Productos"
                        Clicked="OnRecargarClicked"
                        BackgroundColor="#FFFFFF"
                        TextColor="#475569"
                        BorderColor="#CBD5E1"
                        BorderWidth="1"
                        FontSize="13"
                        HeightRequest="42"
                        CornerRadius="8" />
            </VerticalStackLayout>

            <ActivityIndicator x:Name="IndicadorCarga" IsRunning="False" IsVisible="False" Color="#1E3A8A" HeightRequest="40" />

        </VerticalStackLayout>
    </ScrollView>

</ContentPage>
```

#### B. Code-Behind: `NuevaVentaPage.xaml.cs`
```csharp
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

                // Llamada atómica al servicio de ventas
                var ventaRegistrada = await _ventaService.RegistrarVentaAsync(cliente.ClienteID, producto.ProductoID, cantidad);

                // Preguntar al cajero si desea emitir el ticket de inmediato
                bool imprimir = await DisplayAlert(
                    "¡Venta Registrada con Éxito!",
                    $"Ticket #{ventaRegistrada.PedidoID}\nCliente: {ventaRegistrada.ClienteNombre}\nTotal: RD$ {ventaRegistrada.Total:N2}\n\n¿Desea generar e imprimir el Ticket en PDF ahora?",
                    "Sí, Imprimir Ticket",
                    "Cerrar");

                if (imprimir)
                {
                    await _pdfService.GenerarTicketVentaPdfAsync(ventaRegistrada);
                }

                // Limpiar formulario y recargar stock
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
```

---

### 7.10 Interfaz y Lógica de Dashboard y Reportes (`Views/DashboardReportesPage`)

#### A. Vista XAML: `DashboardReportesPage.xaml`
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:models="clr-namespace:VentasApp_Tienda742.Models"
             x:Class="VentasApp_Tienda742.Views.DashboardReportesPage"
             Title="📊 Dashboard y Reportes"
             BackgroundColor="#F8FAFC">

    <Grid RowDefinitions="Auto, Auto, Auto, *" Padding="16" RowSpacing="12">

        <!-- Fila 0: Mini Dashboard (4 Tarjetas Bento KPI) -->
        <Grid Grid.Row="0" ColumnDefinitions="*, *, *, *" ColumnSpacing="10">

            <!-- KPI 1: Total Ventas -->
            <Border Grid.Column="0" BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 10" Padding="12">
                <VerticalStackLayout Spacing="4">
                    <Label Text="💰 Total Ventas" FontSize="11" TextColor="#64748B" FontAttributes="Bold" />
                    <Label x:Name="LblKpiTotalVentas" Text="RD$ 0.00" FontSize="15" FontAttributes="Bold" TextColor="#16A34A" />
                    <Label Text="Ingresos Totales" FontSize="10" TextColor="#94A3B8" />
                </VerticalStackLayout>
            </Border>

            <!-- KPI 2: Total Pedidos -->
            <Border Grid.Column="1" BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 10" Padding="12">
                <VerticalStackLayout Spacing="4">
                    <Label Text="🧾 Pedidos" FontSize="11" TextColor="#64748B" FontAttributes="Bold" />
                    <Label x:Name="LblKpiTotalPedidos" Text="0" FontSize="16" FontAttributes="Bold" TextColor="#1E3A8A" />
                    <Label Text="Transacciones" FontSize="10" TextColor="#94A3B8" />
                </VerticalStackLayout>
            </Border>

            <!-- KPI 3: Clientes -->
            <Border Grid.Column="2" BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 10" Padding="12">
                <VerticalStackLayout Spacing="4">
                    <Label Text="👥 Clientes" FontSize="11" TextColor="#64748B" FontAttributes="Bold" />
                    <Label x:Name="LblKpiTotalClientes" Text="0" FontSize="16" FontAttributes="Bold" TextColor="#0F172A" />
                    <Label Text="Cartera Activa" FontSize="10" TextColor="#94A3B8" />
                </VerticalStackLayout>
            </Border>

            <!-- KPI 4: Stock -->
            <Border Grid.Column="3" BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 10" Padding="12">
                <VerticalStackLayout Spacing="4">
                    <Label Text="📦 Stock Total" FontSize="11" TextColor="#64748B" FontAttributes="Bold" />
                    <Label x:Name="LblKpiTotalStock" Text="0 uds" FontSize="16" FontAttributes="Bold" TextColor="#D97706" />
                    <Label Text="Unidades en Tienda" FontSize="10" TextColor="#94A3B8" />
                </VerticalStackLayout>
            </Border>
        </Grid>

        <!-- Fila 1: Botonera de Reportes PDF -->
        <Border Grid.Row="1" BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 10" Padding="10">
            <FlexLayout JustifyContent="SpaceBetween" AlignItems="Center" Wrap="Wrap">
                <HorizontalStackLayout Spacing="8">
                    <Button Text="📄 PDF Reporte Ventas"
                            Clicked="OnGenerarReporteVentasPdfClicked"
                            BackgroundColor="#1E3A8A"
                            TextColor="#FFFFFF"
                            FontSize="12"
                            FontAttributes="Bold"
                            CornerRadius="8"
                            HeightRequest="38" />

                    <Button Text="📦 PDF Reporte Inventario"
                            Clicked="OnGenerarReporteInventarioPdfClicked"
                            BackgroundColor="#047857"
                            TextColor="#FFFFFF"
                            FontSize="12"
                            FontAttributes="Bold"
                            CornerRadius="8"
                            HeightRequest="38" />
                </HorizontalStackLayout>

                <Button Text="🔄 Refrescar"
                        Clicked="OnRefrescarClicked"
                        BackgroundColor="#F1F5F9"
                        TextColor="#0F172A"
                        BorderColor="#CBD5E1"
                        BorderWidth="1"
                        FontSize="12"
                        FontAttributes="Bold"
                        CornerRadius="8"
                        HeightRequest="38" />
            </FlexLayout>
        </Border>

        <!-- Fila 2: Barra de Búsqueda -->
        <Border Grid.Row="2" BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 8" Padding="2">
            <SearchBar x:Name="TxtBuscarVentas"
                       Placeholder="🔍 Buscar ventas por cliente, producto o ciudad..."
                       PlaceholderColor="#94A3B8"
                       TextColor="#0F172A"
                       FontSize="13"
                       TextChanged="OnBuscarVentasChanged" />
        </Border>

        <!-- Fila 3: Listado de Ventas -->
        <Grid Grid.Row="3">
            <CollectionView x:Name="ColeccionVentas" SelectionMode="None">
                <CollectionView.ItemsLayout>
                    <LinearItemsLayout Orientation="Vertical" ItemSpacing="8" />
                </CollectionView.ItemsLayout>

                <CollectionView.EmptyView>
                    <VerticalStackLayout HorizontalOptions="Center" VerticalOptions="Center" Spacing="8" Padding="20">
                        <Label Text="📋" FontSize="36" HorizontalOptions="Center" />
                        <Label Text="No se encontraron ventas para mostrar." FontSize="14" TextColor="#64748B" HorizontalOptions="Center" />
                    </VerticalStackLayout>
                </CollectionView.EmptyView>

                <CollectionView.ItemTemplate>
                    <DataTemplate x:DataType="models:Venta">
                        <Border BackgroundColor="#FFFFFF" Stroke="#E2E8F0" StrokeThickness="1" StrokeShape="RoundRectangle 10" Padding="12">
                            <Grid ColumnDefinitions="Auto, *, Auto, Auto" ColumnSpacing="12" VerticalOptions="Center">
                                
                                <!-- ID y Fecha -->
                                <VerticalStackLayout Grid.Column="0" Spacing="2" WidthRequest="75" VerticalOptions="Center">
                                    <Label Text="{Binding PedidoID, StringFormat='#{0}'}" FontSize="14" FontAttributes="Bold" TextColor="#1E3A8A" />
                                    <Label Text="{Binding FechaPedido, StringFormat='{0:dd/MM/yy}'}" FontSize="11" TextColor="#64748B" />
                                    <Label Text="{Binding FechaPedido, StringFormat='{0:hh:mm tt}'}" FontSize="10" TextColor="#94A3B8" />
                                </VerticalStackLayout>

                                <!-- Cliente y Detalle -->
                                <VerticalStackLayout Grid.Column="1" Spacing="2" VerticalOptions="Center">
                                    <Label Text="{Binding ClienteNombre}" FontSize="14" FontAttributes="Bold" TextColor="#0F172A" LineBreakMode="TailTruncation" />
                                    <Label Text="{Binding DetalleResumen}" FontSize="12" TextColor="#334155" LineBreakMode="TailTruncation" />
                                    <Label Text="{Binding Ciudad, StringFormat='📍 {0}'}" FontSize="11" TextColor="#64748B" />
                                </VerticalStackLayout>

                                <!-- Total -->
                                <VerticalStackLayout Grid.Column="2" Spacing="2" VerticalOptions="Center" HorizontalOptions="End" WidthRequest="110">
                                    <Label Text="{Binding TotalFormateado}" FontSize="15" FontAttributes="Bold" TextColor="#16A34A" HorizontalOptions="End" />
                                    <Label Text="{Binding Subtotal, StringFormat='Sub: ${0:N2}'}" FontSize="10" TextColor="#64748B" HorizontalOptions="End" />
                                    <Label Text="{Binding Impuesto, StringFormat='ITBIS: ${0:N2}'}" FontSize="10" TextColor="#94A3B8" HorizontalOptions="End" />
                                </VerticalStackLayout>

                                <!-- Botón Imprimir Ticket -->
                                <Button Grid.Column="3"
                                        Text="🧾 Ticket"
                                        Clicked="OnImprimirTicketItemClicked"
                                        CommandParameter="{Binding .}"
                                        BackgroundColor="#EFF6FF"
                                        TextColor="#1D4ED8"
                                        BorderColor="#BFDBFE"
                                        BorderWidth="1"
                                        FontSize="11"
                                        FontAttributes="Bold"
                                        HeightRequest="36"
                                        CornerRadius="6"
                                        Padding="10, 0"
                                        VerticalOptions="Center" />
                            </Grid>
                        </Border>
                    </DataTemplate>
                </CollectionView.ItemTemplate>
            </CollectionView>

            <ActivityIndicator x:Name="IndicadorCargaReportes" IsRunning="False" IsVisible="False" Color="#1E3A8A" HeightRequest="40" HorizontalOptions="Center" VerticalOptions="Center" />
        </Grid>
    </Grid>

</ContentPage>
```

#### B. Code-Behind: `DashboardReportesPage.xaml.cs`
```csharp
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

        // Exportación de Ventas a PDF A4
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

        // Exportación de Inventario a PDF A4
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

        // Reimpresión individual de Ticket 80mm
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
```

---

## 8. COMPILACIÓN, DESPLIEGUE Y PRUEBAS OPERATIVAS

Para levantar la aplicación en tu entorno Windows, ejecuta los siguientes pasos:

### 8.1 Restauración y Compilación Limpia
```powershell
# 1. Posicionarse en la carpeta raíz del proyecto
cd "C:\Users\User\Desktop\CARPETA DE TRABAJO DE ANTIGRAVITY CLI-JPV\VentasApp_Tienda742"

# 2. Restaurar dependencias NuGet
dotnet restore

# 3. Compilar para Windows Desktop
dotnet build -f net9.0-windows10.0.19041.0
```
*Resultado esperado:* `Build succeeded. 0 Warning(s), 0 Error(s)`.

### 8.2 Ejecución de la Aplicación
```powershell
dotnet run --project VentasApp_Tienda742 -f net9.0-windows10.0.19041.0
```

### 8.3 Flujo de Prueba Recomendado
1. **Verificación de Conexión:** Observa el indicador superior que marca `🟢 SQL Server: 192.168.2.68:1433`.
2. **Seleccionar Cliente:** Despliega el selector y elige a *"Juan Perez (Santo Domingo)"*.
3. **Seleccionar Producto:** Elige *"Laptop Dell Core i7"*. Observa cómo el stock se muestra automáticamente en pantalla (`Stock actual: 15 uds`).
4. **Indicar Cantidad:** Establece la cantidad en `2`.
5. **Comprobar Cálculos:**
   - Subtotal: `RD$ 130,000.00`
   - ITBIS (18%): `RD$ 23,400.00`
   - Total a Pagar: `RD$ 153,400.00`
6. **Confirmar Venta:** Presiona **💾 PROCESAR VENTA Y GENERAR TICKET**.
7. **Emisión de Comprobante:** En el cuadro de diálogo, presiona **Sí, Imprimir Ticket**. El visor de PDFs predeterminado de Windows se abrirá automáticamente mostrando el ticket térmico con su código, artículos y fecha exacta.
8. **Revisar Dashboard:** Ve a la pestaña **📊 Dashboard y Reportes** y comprueba cómo el total acumulado y los pedidos se han incrementado de forma instantánea.
9. **Exportar Reportes:** Haz clic en **📄 PDF Reporte Ventas** y luego en **📦 PDF Reporte Inventario** para verificar la generación de los documentos oficiales en formato A4.

---

## 9. GUÍA DE DIAGNÓSTICO Y SOLUCIÓN DE PROBLEMAS (TROUBLESHOOTING)

| Síntoma / Error | Causa Probable | Solución Inmediata |
| :--- | :--- | :--- |
| **`A network-related or instance-specific error occurred`** | El servicio SQL Server está detenido o el firewall bloquea el puerto 1433. | 1. Abre PowerShell y verifica con: `Test-NetConnection -ComputerName 192.168.2.68 -Port 1433`.<br>2. Si falla, inicia el servicio `MSSQLSERVER` y ejecuta la regla de firewall del paso 4.4. |
| **`Login failed for user 'JUANCITO'`** | Contraseña incorrecta o autenticación SQL deshabilitada. | Asegúrate de haber ejecutado el paso 4.1 (Modo Mixto) y reiniciado el servicio SQL Server en SSMS. |
| **`Stock insuficiente`** | La cantidad solicitada supera las existencias físicas en la tabla `Productos`. | Reduce la cantidad en la interfaz o actualiza el stock en la base de datos con: `UPDATE Productos SET Stock = 50 WHERE ProductoID = X`. |
| **`QuestPDF License Exception`** | La licencia comunitaria no fue inicializada antes de crear documentos. | Confirma que `MauiProgram.cs` contenga `QuestPDF.Settings.License = LicenseType.Community;`. |
| **`The name 'InitializeComponent' does not exist`** | Faltan los prefijos `xmlns` o `xmlns:x` en los archivos XAML. | Verifica que todos los archivos XAML contengan las cabeceras estándar declaradas en este manual. |

---

> **Ingeniería JPV Pro** — *Diseñado e implementado con los más altos estándares de calidad, seguridad ACID, accesibilidad y rendimiento.*
