USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Delete SubCategory	--
-- in dbo.SubCategory				--
--------------------------------------
CREATE PROCEDURE [dbo].[DeleteSubCategory]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Delete SubCategory
	UPDATE [dbo].[SubCategory] SET IsDeleted = 1 WHERE Id = @Id;
END;
GO