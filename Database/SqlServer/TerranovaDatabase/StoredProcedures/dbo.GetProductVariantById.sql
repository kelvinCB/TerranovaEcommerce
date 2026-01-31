USE [Terranova]
GO
------------------------------------------
-- Autor: Briangel Abreu				--
-- Date: 12-06-2025						--
-- Description: Show a Product Variant	--
-- Information and its Main Image by Id	--
------------------------------------------
CREATE PROCEDURE [dbo].[GetProductVariantById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Select a Product Varian with its main images
	SELECT 
		V.Id, V.ProductId, V.VariantName, V.VariantValue, V.PriceAdjustment, V.StockQuantity, V.CreateAt, -- Product Variant Fields
		I.ImageUrl -- Product Variant Image Fields
	FROM [dbo].[ProductVariant] V
	LEFT JOIN [dbo].[ProductVariantImage] I ON (V.Id = I.ProductVariantId AND I.IsMain = 1)
	WHERE V.Id = @Id AND V.IsDeleted = 0;

END;
GO