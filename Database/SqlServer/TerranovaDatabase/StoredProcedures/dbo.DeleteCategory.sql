USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 11-06-2025				--
-- Description: Delete Category	--
-- in dbo.Category				--
----------------------------------
CREATE PROCEDURE [dbo].[DeleteCategory]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Delete a category
	UPDATE [dbo].[Category] SET IsDeleted = 1 WHERE Id = @Id;
END;
GO