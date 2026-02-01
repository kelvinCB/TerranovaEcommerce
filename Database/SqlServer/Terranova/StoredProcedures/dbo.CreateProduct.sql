USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Create Product		--
-- in dbo.Product Table 			--
--------------------------------------
CREATE PROCEDURE [dbo].[CreateProduct]
	@UserId INT,
	@Name VARCHAR(225),
	@Description TEXT = NULL,
	@SKU VARCHAR(100) = NULL,
	@Price DECIMAL(18, 2),
	@Currency VARCHAR(10) = 'USD',
	@StockQuantity INT = 1,
	@Weight DECIMAL(18, 2) = NULL,
	@Dimensions VARCHAR(100) = NULL,
	@ImageUrl VARCHAR(500),
	@IsMain BIT = 1
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRAN
		
		-- Insert data in Product table
		INSERT INTO [dbo].[Product] (UserId, Name, Description, SKU, Price, Currency, StockQuantity, Weight, Dimensions)
		VALUES (@UserId, @Name, @Description, @SKU, @Price, @Currency, @StockQuantity, @Weight, @Dimensions)

		-- Catch the Id Product
		DECLARE @ProductId INT = SCOPE_IDENTITY();

		-- Insert the main Product Image
		INSERT INTO [dbo].[ProductImage] (ProductId, ImageUrl, IsMain)
		VALUES (@ProductId, @ImageUrl, @IsMain);

		COMMIT;
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH;
END;
GO