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