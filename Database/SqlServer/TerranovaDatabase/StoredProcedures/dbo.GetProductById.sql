USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Show Product		--
-- Information by Id				--
--------------------------------------
CREATE PROCEDURE [dbo].[GetProductById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

		-- Select a Product and its Main Image by id
		SELECT 
			P.Id, P.UserId, P.Name, P.Description, P.SKU, P.Price, P.Currency, P.StockQuantity, P.Weight, P.Dimensions, P.IsActive, P.CreatedAt, -- Product Field
			I.ImageUrl -- ProductImage Field
		FROM [dbo].[Product] P
		LEFT JOIN [dbo].[ProductImage] I ON(P.Id = I.ProductId AND I.IsMain = 1)
		WHERE P.Id = @Id AND P.IsDeleted = 0 AND P.IsApproved = 1;

END;
GO