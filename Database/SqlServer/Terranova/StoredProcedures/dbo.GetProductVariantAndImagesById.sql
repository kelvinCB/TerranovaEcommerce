USE [Terranova]
GO
------------------------------------------
-- Autor: Briangel Abreu				--
-- Date: 12-06-2025						--
-- Description: Show a Product Variant	--
-- Information and its Images by Id		--
------------------------------------------
CREATE PROCEDURE [dbo].[GetProductVariantAndImagesById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Select a Product Variant with 
	SELECT 
		V.Id, V.ProductId, V.VariantName, V.VariantValue, V.PriceAdjustment, V.StockQuantity, V.CreateAt, -- Product Variant Fields
		(SELECT
			Id, ImageUrl, IsMain
		 FROM [dbo].[ProductVariantImage] I
		 WHERE I.ProductVariantId = V.Id
		 FOR JSON PATH
		) AS ProductVariantImages -- Product Variant Images Field as Json
	FROM [dbo].[ProductVariant] V
	WHERE V.Id = @Id AND V.IsDeleted = 0
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;

END;
GO