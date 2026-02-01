USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Create Relationship	--
-- Product-SubCategory in			--
-- dbo.ProductSubCategory Table		--
--------------------------------------
CREATE PROCEDURE [dbo].[CreateProductSubCategory]
	@ProductId INT,
	@SubCategoryId INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Check if SubCategoryId exists in SubCategory Table
	DECLARE @ExistSubCategoryId BIT;
	SELECT @ExistSubCategoryId = CASE WHEN EXISTS(SELECT 1 FROM [dbo].[SubCategory] WHERE Id = @SubCategoryId) THEN 1 ELSE 0 END;

	IF @ExistSubCategoryId = 0
	BEGIN
		THROW 50001, 'The provided SubCategoryId does not exist.', 1;
		RETURN;
	END

	-- Insert a product in a SubCategory
	INSERT INTO [dbo].[ProductSubCategory] (ProductId, SubCategory)
	VALUES (@ProductId, @SubCategoryId);
END;
GO