/************************************************************************************************
    SQL SERVER 2019 / 2022 - GUÍA DIDÁCTICA INTEGRAL Y ESTRUCTURA COMPLETA
    BASE DE DATOS: TiendaDB_742
    AUTOR: Ing. Juancito Peña Vizcaíno
    
    ESTRUCTURA DEL SCRIPT:
    1. DDL (Data Definition Language) - Creación de DB, esquemas, tablas, llaves foráneas e índices.
    2. DML (Data Manipulation Language) - Carga de catálogos base, updates y deletes controlados.
    3. DQL (Data Query Language) - Consultas, proyecciones, filtros, agregaciones y uniones JOINs.
    4. TCL (Transaction Control Language) - Transacciones atómicas (COMMIT / ROLLBACK).
    5. DCL (Data Control Language) - Gestión de permisos sobre objetos (GRANT, REVOKE, DENY).
    6. OBJETOS PROGRAMABLES - Vistas, Stored Procedures, Funciones UDF, Triggers y CTEs.
    7. CONSULTA FINAL INTEGRADORA - Reporte ejecutivo con ITBIS 18% y funciones de fecha.
************************************************************************************************/


/************************************************************************************************
    SECCIÓN 1: DDL (DATA DEFINITION LANGUAGE)
    Definición: Crea y altera la estructura de la base de datos y sus metadatos.
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

ALTER DATABASE TiendaDB_742 SET MULTI_USER;
GO

USE TiendaDB_742;
GO

-- 1.2 CREACIÓN DE TABLAS CON RESTRICCIONES (CONSTRAINTS)
-- TABLA: Clientes
CREATE TABLE Clientes
(
    ClienteID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE,
    Ciudad VARCHAR(100),
    Telefono VARCHAR(20),
    FechaRegistro DATETIME DEFAULT GETDATE()
);
GO

-- Índice no agrupado para optimizar búsquedas por nombre
CREATE NONCLUSTERED INDEX IX_Clientes_Nombre ON Clientes(Nombre);
GO

-- TABLA: Productos
CREATE TABLE Productos
(
    ProductoID INT IDENTITY(1,1) PRIMARY KEY,
    NombreProducto VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) CHECK(Precio > 0),
    Stock INT DEFAULT 0 CHECK(Stock >= 0)
);
GO

-- TABLA: Vendedores (Gestores Comerciales)
CREATE TABLE Vendedores
(
    VendedorID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Email VARCHAR(150),
    Telefono VARCHAR(20),
    Zona VARCHAR(50)
);
GO

-- TABLA: Pedidos (Ventas Transaccionales)
CREATE TABLE Pedidos
(
    PedidoID INT IDENTITY(1,1) PRIMARY KEY,
    ClienteID INT NOT NULL,
    ProductoID INT NOT NULL,
    VendedorID INT NULL,
    Cantidad INT CHECK(Cantidad > 0),
    FechaPedido DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_Pedidos_Clientes 
        FOREIGN KEY (ClienteID) REFERENCES Clientes(ClienteID),

    CONSTRAINT FK_Pedidos_Productos 
        FOREIGN KEY (ProductoID) REFERENCES Productos(ProductoID),

    CONSTRAINT FK_Pedidos_Vendedores 
        FOREIGN KEY (VendedorID) REFERENCES Vendedores(VendedorID)
);
GO


/************************************************************************************************
    SECCIÓN 2: DML (DATA MANIPULATION LANGUAGE)
    Definición: Carga de catálogos base, modificaciones y borrados.
************************************************************************************************/

-- 2.1 INSERCIÓN DE CLIENTES INICIALES (12 REGISTROS)
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

-- 2.2 INSERCIÓN DE PRODUCTOS (12 ARTÍCULOS)
INSERT INTO Productos (NombreProducto, Precio, Stock)
VALUES
('Laptop Dell Core i7', 65000.00, 150),
('Mouse Inalambrico Logitech', 500.00, 500),
('Monitor Samsung 24', 12000.00, 200),
('Teclado Mecanico RGB', 2500.00, 300),
('Impresora Termica POS 80mm', 15000.00, 80),
('Webcam Full HD', 3200.00, 250),
('Disco SSD 1TB NVMe', 4800.00, 400),
('Memoria RAM 16GB DDR4', 3500.00, 350),
('Headset Gamer Bluetooth', 2800.00, 220),
('Pad Mouse XL Impermeable', 800.00, 600),
('Silla Ergonomica de Oficina', 18500.00, 90),
('Cable HDMI 2.1 Ultra HD', 600.00, 800);
GO

-- 2.3 INSERCIÓN DE VENDEDORES (6 VENDEDORES)
INSERT INTO Vendedores (Nombre, Email, Telefono, Zona)
VALUES
('Juancito Pena', 'jpena@tienda742.com', '8095551001', 'Distrito Nacional'),
('Carlos Mendoza', 'cmendoza@tienda742.com', '8095551002', 'Santiago Cibao'),
('Mildre Santana', 'msantana@tienda742.com', '8095551003', 'Zona Este'),
('Dulce Maria', 'dmaria@tienda742.com', '8095551004', 'La Vega'),
('Pedro Alcantara', 'palcantara@tienda742.com', '8095551005', 'Santo Domingo Este'),
('Ana Morales', 'amorales@tienda742.com', '8095551006', 'Puerto Plata');
GO

-- 2.4 INSERCIÓN DE PEDIDOS INICIALES (15 PEDIDOS)
INSERT INTO Pedidos (ClienteID, ProductoID, VendedorID, Cantidad, FechaPedido)
VALUES
(1, 1, 1, 1, '2026-01-05 09:15:00'),
(1, 2, 1, 2, '2026-01-05 09:20:00'),
(2, 3, 2, 1, '2026-01-10 11:30:00'),
(3, 4, 3, 1, '2026-01-12 14:45:00'),
(4, 5, 4, 2, '2026-01-18 16:10:00'),
(5, 6, 5, 1, '2026-01-22 10:05:00'),
(6, 7, 6, 3, '2026-01-28 17:50:00'),
(7, 8, 1, 2, '2026-02-02 12:20:00'),
(8, 9, 2, 1, '2026-02-08 15:35:00'),
(9, 10, 3, 4, '2026-02-14 18:40:00'),
(10, 1, 4, 1, '2026-02-20 09:50:00'),
(11, 2, 5, 5, '2026-02-24 13:15:00'),
(12, 11, 6, 1, '2026-02-27 16:30:00'),
(2, 12, 2, 2, '2026-03-02 10:40:00'),
(3, 7, 3, 1, '2026-03-05 14:00:00');
GO

-- 2.5 ACTUALIZACIÓN DEMOSTRATIVA (UPDATE)
UPDATE Productos
SET Precio = 68000.00
WHERE ProductoID = 1;
GO

-- 2.6 ELIMINACIÓN DEMOSTRATIVA (DELETE)
DELETE FROM Pedidos
WHERE PedidoID = 15;
GO


/************************************************************************************************
    SECCIÓN 3: DQL (DATA QUERY LANGUAGE)
    Definición: Consultas avanzadas para proyectar y resumir información.
************************************************************************************************/

-- 3.1 PROYECCIÓN BÁSICA
SELECT * FROM Clientes;
SELECT * FROM Productos;
SELECT * FROM Vendedores;
SELECT * FROM Pedidos;

-- 3.2 TOP Y FILTROS LÓGICOS
SELECT TOP 5 * FROM Clientes;
SELECT TOP 5 * FROM Productos ORDER BY Precio DESC;
SELECT * FROM Productos WHERE Precio > 5000;
SELECT * FROM Clientes WHERE Nombre LIKE 'J%';
SELECT * FROM Productos WHERE Precio BETWEEN 1000 AND 10000;
SELECT * FROM Clientes WHERE Ciudad IN ('Santiago', 'La Vega', 'Moca');

-- 3.3 FUNCIONES DE AGREGACIÓN
SELECT COUNT(*) AS TotalClientes FROM Clientes;
SELECT SUM(Precio) AS SumaPrecios FROM Productos;
SELECT AVG(Precio) AS PromedioPrecios FROM Productos;
SELECT MAX(Precio) AS PrecioMaximo FROM Productos;
SELECT MIN(Precio) AS PrecioMinimo FROM Productos;

-- 3.4 FUNCIONES ESCALARES DE TEXTO Y FECHA
SELECT Nombre, UPPER(Nombre) AS Mayusculas, LOWER(Nombre) AS Minusculas, LEN(Nombre) AS LargoNombre FROM Clientes;
SELECT GETDATE() AS FechaHoraActual, YEAR(GETDATE()) AS Anio, MONTH(GETDATE()) AS Mes, DAY(GETDATE()) AS Dia;

-- 3.5 GROUP BY Y HAVING
SELECT Ciudad, COUNT(*) AS TotalClientes
FROM Clientes
GROUP BY Ciudad
HAVING COUNT(*) > 1;

-- 3.6 JOINS (INNER, LEFT, RIGHT, FULL)
SELECT 
    P.PedidoID,
    C.Nombre AS Cliente,
    PR.NombreProducto,
    PR.Precio,
    P.Cantidad,
    (PR.Precio * P.Cantidad) AS Total,
    ISNULL(V.Nombre, 'Sin Vendedor') AS Vendedor
FROM Pedidos P
INNER JOIN Clientes C ON P.ClienteID = C.ClienteID
INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID
LEFT JOIN Vendedores V ON P.VendedorID = V.VendedorID;

-- 3.7 SUB-CONSULTAS (SUBQUERIES)
SELECT * FROM Productos WHERE Precio > (SELECT AVG(Precio) FROM Productos);


/************************************************************************************************
    SECCIÓN 4: TCL (TRANSACTION CONTROL LANGUAGE)
    Definición: Asegura transacciones atómicas (ACID).
************************************************************************************************/

-- 4.1 COMMIT TRANSACTION
BEGIN TRANSACTION;
UPDATE Productos SET Stock = Stock - 1 WHERE ProductoID = 1;
COMMIT TRANSACTION;
GO

-- 4.2 ROLLBACK TRANSACTION
BEGIN TRANSACTION;
UPDATE Productos SET Precio = 999999.00 WHERE ProductoID = 1;
ROLLBACK TRANSACTION;
GO


/************************************************************************************************
    SECCIÓN 5: DCL (DATA CONTROL LANGUAGE)
************************************************************************************************/
/*
GRANT SELECT, INSERT ON Clientes TO UsuarioVentas;
REVOKE INSERT ON Clientes FROM UsuarioVentas;
DENY DELETE ON Clientes TO UsuarioVentas;
*/


/************************************************************************************************
    SECCIÓN 6: OBJETOS PROGRAMABLES Y CONSULTAS AVANZADAS
************************************************************************************************/

-- 6.1 VISTA OFICIAL: vw_ReporteVentas
CREATE VIEW dbo.vw_ReporteVentas
AS
SELECT 
    P.PedidoID,
    C.ClienteID,
    C.Nombre AS Cliente,
    C.Ciudad,
    C.Telefono,
    PR.ProductoID,
    PR.NombreProducto,
    PR.Precio,
    P.Cantidad,
    (PR.Precio * P.Cantidad) AS Subtotal,
    (PR.Precio * P.Cantidad) * 0.18 AS Impuesto,
    (PR.Precio * P.Cantidad) * 1.18 AS TotalVenta,
    P.FechaPedido,
    ISNULL(V.Nombre, 'Venta Mostrador') AS Vendedor
FROM Pedidos P
INNER JOIN Clientes C ON P.ClienteID = C.ClienteID
INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID
LEFT JOIN Vendedores V ON P.VendedorID = V.VendedorID;
GO

-- 6.2 STORED PROCEDURE: sp_ObtenerClientesPorCiudad
CREATE PROCEDURE dbo.sp_ObtenerClientesPorCiudad
    @CiudadFiltro VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ClienteID, Nombre, Email, Ciudad, Telefono, FechaRegistro
    FROM Clientes
    WHERE Ciudad = @CiudadFiltro;
END;
GO

-- 6.3 FUNCIÓN ESCALAR UDF: fn_CalcularImpuesto (ITBIS 18%)
CREATE FUNCTION dbo.fn_CalcularImpuesto
(
    @Monto DECIMAL(10,2)
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN ISNULL(@Monto, 0) * 0.18;
END;
GO

-- 6.4 TRIGGER DE AUDITORÍA: trg_AuditoriaNuevoCliente
CREATE TRIGGER dbo.trg_AuditoriaNuevoCliente
ON Clientes
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    PRINT 'Notificación: Se registró un nuevo cliente en Tienda 742.';
END;
GO

-- 6.5 CTE (COMMON TABLE EXPRESSION)
WITH ProductosPremium AS
(
    SELECT ProductoID, NombreProducto, Precio, Stock
    FROM Productos
    WHERE Precio >= 5000.00
)
SELECT NombreProducto, Precio, Stock
FROM ProductosPremium
ORDER BY Precio DESC;
GO


/************************************************************************************************
    SECCIÓN 7: CONSULTA FINAL INTEGRADORA
************************************************************************************************/
SELECT TOP 10
    P.PedidoID,
    C.Nombre AS Cliente,
    C.Ciudad,
    PR.NombreProducto,
    PR.Precio,
    P.Cantidad,
    (PR.Precio * P.Cantidad) AS Subtotal,
    dbo.fn_CalcularImpuesto(PR.Precio * P.Cantidad) AS Impuesto,
    (PR.Precio * P.Cantidad) + dbo.fn_CalcularImpuesto(PR.Precio * P.Cantidad) AS TotalPagar,
    ISNULL(V.Nombre, 'Venta Mostrador') AS Vendedor,
    YEAR(P.FechaPedido) AS AnioPedido
FROM Pedidos P
INNER JOIN Clientes C ON P.ClienteID = C.ClienteID
INNER JOIN Productos PR ON P.ProductoID = PR.ProductoID
LEFT JOIN Vendedores V ON P.VendedorID = V.VendedorID
WHERE PR.Precio > 1000
ORDER BY TotalPagar DESC;
GO
