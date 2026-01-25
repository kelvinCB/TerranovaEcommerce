USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Approve Product		--
-- in dbo.Product Table 			--
--------------------------------------
CREATE PROCEDURE [dbo].[ApproveProduct]
	@ProductIdESC INT,
	@IsApproved BIT,
	@SubCategoryIdESC INT
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		IF @IsApproved = 1
		BEGIN
			BEGIN TRAN

			-- Update IsApproved field in Product Table
			UPDATE [dbo].[Product] SET IsApproved = IsApproved WHERE Id = @ProductIdESC;

			-- Check if Product is already in a SubCategory
			DECLARE @IsProductInSubCategory BIT;

			SELECT @IsProductInSubCategory = CASE WHEN EXISTS(SELECT 1 FROM [dbo].[ProductSubCategory] 
			WHERE SubCategory = @SubCategoryIdESC AND ProductId = @ProductIdESC) 
			THEN 1 ELSE 0 END;

			IF @IsProductInSubCategory = 0
			BEGIN
				-- Include Product in SubCategory
				EXECUTE [dbo].[CreateProductSubCategory] @ProductId = @ProductIdESC, @SubCategoryId = @SubCategoryIdESC;
			END;

			COMMIT;
		END;
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END;
GO