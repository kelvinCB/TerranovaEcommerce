USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 11-06-2025				--
-- Description: Create Category	--
-- in dbo.Category				--
----------------------------------
CREATE PROCEDURE [dbo].[CreateCategory]
	@Name VARCHAR(50),
	@Description VARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	-- Insert new category
	INSERT INTO [dbo].[Category] (Name, Description)
	VALUES (@Name, @Description)
END;
GO