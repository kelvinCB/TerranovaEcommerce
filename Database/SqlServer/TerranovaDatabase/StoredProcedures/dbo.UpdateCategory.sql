USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 11-06-2025				--
-- Description: Update Category	--
-- in dbo.Category				--
----------------------------------
CREATE PROCEDURE [dbo].[UpdateCategory]
	@Id INT,
	@Name VARCHAR(50),
	@Description VARCHAR(500)
AS
BEGIN
	SET NOCOUNT ON;

	-- Update a category
	UPDATE [dbo].[Category] SET Name = @Name, Description = @Description, UpdatedAt = GETDATE() WHERE Id = @Id;
END;
GO