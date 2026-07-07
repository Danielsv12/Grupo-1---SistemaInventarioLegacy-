USE InventarioLegacyDB;
GO

CREATE OR ALTER VIEW dbo.vw_Productos AS
SELECT  
    p.Id,
    p.Codigo,
    p.Nombre,
    c.Nombre AS Categoria,
    p.Descripcion,
    p.PrecioCompra,
    p.PrecioVenta,
    p.Stock,
    p.StockMinimo,
    p.CategoriaId,
    p.Activo,
    p.FechaCreacion,
    p.UltimaModificacion
FROM dbo.Productos p
LEFT JOIN dbo.Categorias c 
    ON c.Id = p.CategoriaId;
GO


USE InventarioLegacyDB;
GO

SELECT TOP 10 *
FROM dbo.vw_Productos;