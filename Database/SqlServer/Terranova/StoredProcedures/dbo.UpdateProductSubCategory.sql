USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Update Relationship	--
-- Product-SubCategory in			--
-- dbo.ProductSubCategory Table		--
--------------------------------------
CREATE PROCEDURE [dbo].[UpdateProductSubCategory]
	@Id INT,
	@ProductId INT,
	@SubCategory INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Check if the Product exist
	DECLARE @ExistProduct BIT;
	SELECT @ExistProduct = CASE WHEN EXISTS(SELECT 1 FROM [dbo].[Product] WHERE Id = @ProductId) THEN 1 ELSE 0 END;

	-- Check if the SubCategory exist
	DECLARE @ExistSubCategory BIT;
	SELECT @ExistSubCategory = CASE WHEN EXISTS(SELECT 1 FROM [dbo].[SubCategory] WHERE Id = @SubCategory) THEN 1 ELSE 0 END;

	IF (@ExistProduct = 0 OR @ExistSubCategory = 0)
	BEGIN
		THROW 50001, 'The provided SubCategoryId or ProductId does not exist.', 1;
		RETURN;
	END;

	-- Update Relationship between Product and SubCategory
	UPDATE [dbo].[ProductSubCategory] SET ProductId = @ProductId, SubCategory = @SubCategory WHERE Id = Id;
END;
GO