USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Update SubCategory	--
-- in dbo.SubCategory				--
--------------------------------------
CREATE PROCEDURE [dbo].[UpdateSubCategory]
	@Id INT,
	@CategoryId INT,
	@Name VARCHAR(50),
	@Description VARCHAR(500)
AS
BEGIN
	SET NOCOUNT ON;

	-- Check if the CategoryId exists in Category table
	DECLARE @ExistCategoryId BIT;
	SELECT @ExistCategoryId = CASE WHEN EXISTS (SELECT 1 FROM [dbo].[Category] WHERE Id = @CategoryId) THEN 1 ELSE 0 END;

	IF @ExistCategoryId = 0
	BEGIN
		THROW 50001, 'Cannot update the SubCategory. Please set a correct relationship between SubCategory and Category before updating this one.', 1;
		RETURN;
	END;

	-- Update SubCategory data
	UPDATE [dbo].[SubCategory] SET CategoryId = @CategoryId, Name = @Name, Description = @Description, UpdatedAt = GETDATE() WHERE Id = @Id;
END;
GO