USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Show SubCategory	--
-- Information by Id				--
--------------------------------------
CREATE PROCEDURE [dbo].[GetSubCategoryById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Select SubCategory information by id
	SELECT Name, CategoryId, Name, Description FROM [dbo].[SubCategory] WHERE Id = @Id AND IsDeleted = 0;
END;
GO