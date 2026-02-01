USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 11-06-2025				--
-- Description: Show Category	--
-- Information by Id			--
----------------------------------
CREATE PROCEDURE [dbo].[GetCategoryById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Select category information by Id
	SELECT Id, Name, Description FROM [dbo].[Category] WHERE Id = @Id AND IsDeleted = 0;
END;
GO