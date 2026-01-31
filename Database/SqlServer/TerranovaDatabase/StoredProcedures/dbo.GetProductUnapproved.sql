USE [Terranova]
GO
------------------------------------------
-- Autor: Briangel Abreu				--
-- Date: 13-06-2025						--
-- Description: Show Unapproved			--
-- Product List Information				--
------------------------------------------
CREATE PROCEDURE [dbo].[GetProductUnapproved]
AS
BEGIN
	SET NOCOUNT ON;

	-- Select a product list that is only unapproved
	SELECT 
		P.Id, P.UserId, P.Name, P.Description, P.Price, P.Currency, P.StockQuantity, P.CreatedAt, -- Product Fields
		I.ImageUrl -- Product Image Fields
	FROM [dbo].[Product] P
	LEFT JOIN [dbo].[ProductImage] I ON(P.Id = I.ProductId AND I.IsMain = 1)
	WHERE P.IsDeleted = 0 AND P.IsApproved = 0;

END;
GO