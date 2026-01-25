USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Create SubCategory	--
-- in dbo.SubCategory				--
--------------------------------------
CREATE PROCEDURE [dbo].[CreateSubCategory]
	@CategoryId INT,
	@Name VARCHAR(50),
	@Description VARCHAR(500) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	-- Insert data in SubCategory table
	INSERT INTO [dbo].[SubCategory] (CategoryId, Name, Description)
	VALUES (@CategoryId, @Name, @Description);
END;
GO