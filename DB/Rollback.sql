USE InventarioLegacyDB;
GO

IF OBJECT_ID('dbo.vw_Productos','V') IS NOT NULL
    DROP VIEW dbo.vw_Productos;
GO

SELECT OBJECT_ID('dbo.vw_Productos','V') AS VistaProductos;
GO