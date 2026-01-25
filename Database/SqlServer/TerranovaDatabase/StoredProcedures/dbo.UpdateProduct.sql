USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Update Product		--
-- in dbo.Product Table 			--
--------------------------------------
CREATE PROCEDURE [dbo].[UpdateProduct]
	@Id INT,
	@Name VARCHAR(225),
	@Description TEXT = NULL,
	@SKU VARCHAR(100) = NULL,
	@Price DECIMAL(18, 2),
	@Currency VARCHAR(10) = 'USD',
	@StockQuantity INT,
	@Weight DECIMAL(18, 2) = NULL,
	@Dimensions VARCHAR(100) = NULL,
	@IsActive BIT = 1
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRAN

		-- Update Product Table
		UPDATE [dbo].[Product] 
		SET Name = @Name, Description = @Description, SKU = @SKU, Price = @Price, Currency = @Currency, StockQuantity = @StockQuantity,
		Weight = @Weight, Dimensions = @Dimensions, IsActive = @IsActive
		WHERE Id = @Id

		COMMIT;

	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END;
GO