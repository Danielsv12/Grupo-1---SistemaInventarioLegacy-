USE InventarioLegacyDB;
GO

-- A) Total de productos
SELECT COUNT(*) AS TotalProductos 
FROM dbo.Productos;

-- B) Productos sin categoría vinculada
SELECT COUNT(*) AS SinVincular 
FROM dbo.Productos 
WHERE CategoriaId IS NULL;

-- C) Filas en la vista
SELECT COUNT(*) AS FilasVista 
FROM dbo.vw_Productos;