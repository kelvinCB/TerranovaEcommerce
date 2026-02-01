USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Delete Product		--
-- in dbo.Product Table 			--
--------------------------------------
CREATE PROCEDURE [dbo].[DeleteProduct]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRAN

		--Delete Product
		UPDATE [dbo].[Product] SET IsDeleted = 1 WHERE Id = @Id;

		COMMIT;
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END;
GO