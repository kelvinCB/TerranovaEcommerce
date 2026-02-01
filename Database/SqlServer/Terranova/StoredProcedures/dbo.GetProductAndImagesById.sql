USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Show Product		--
-- Information and its Images by Id	--
--------------------------------------
CREATE PROCEDURE [dbo].[GetProductAndImagesById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Select a Product with its Images by Id
	SELECT 
		P.Id, P.UserId, P.Name, P.Description, P.SKU, P.Price, P.Currency, P.StockQuantity, P.Weight, P.Dimensions, P.IsActive, P.CreatedAt, -- Product Field
		(SELECT 
			Id, ImageUrl, IsMain 
		 FROM [dbo].[ProductImage] I 
		 WHERE I.ProductId = P.Id FOR JSON PATH) AS ProductImages --Images Field as Json
	FROM [dbo].[Product] P
	WHERE P.Id = @Id AND P.IsDeleted = 0 AND P.IsApproved = 1
	FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;

END;
GO