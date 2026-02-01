USE [Terranova]
GO
------------------------------------------
-- Autor: Briangel Abreu				--
-- Date: 12-06-2025						--
-- Description: Show a Product and its	--
-- Variants Information with their		--
-- Main Image by Id						--
------------------------------------------
CREATE PROCEDURE [dbo].[GetProductAndVariantsById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Select a product with its Main Image and Variants
	SELECT
		P.Id, P.UserId, P.Name, P.Description, P.SKU, P.Price, P.Currency, P.StockQuantity, P.Weight, P.Dimensions, P.IsActive, P.CreatedAt, -- Product Fields
		IP.ImageUrl, -- Product Image Field
		(SELECT 
			V.Id, V.VariantName, V.VariantValue, V.PriceAdjustment, V.StockQuantity, V.CreateAt, -- Product Variant Fields
			IV.ImageUrl -- Product Variant Image Field
		 FROM [dbo].[ProductVariant] V
		 LEFT JOIN [dbo].[ProductVariantImage] IV ON(V.Id = IV.ProductVariantId AND IV.IsMain = 1)
		 WHERE V.ProductId = P.Id AND V.IsDeleted = 0
		 FOR JSON PATH
		)
	FROM [dbo].[Product] P
	LEFT JOIN [dbo].[ProductImage] IP ON(P.Id = IP.ProductId AND IP.IsMain = 1)
	WHERE P.Id = @Id AND P.IsDeleted = 0 AND P.IsApproved = 1
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;

END;
GO