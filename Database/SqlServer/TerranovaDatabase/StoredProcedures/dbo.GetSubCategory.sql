USE [Terranova]
GO
--------------------------------------
-- Autor: Briangel Abreu			--
-- Date: 11-06-2025					--
-- Description: Show SubCategory	--
-- Information						--
--------------------------------------
CREATE PROCEDURE [dbo].[GetSubCategory]
AS
BEGIN
	SET NOCOUNT ON;

	-- Select SubCategory information
	SELECT Name, CategoryId, Name, Description FROM [dbo].[SubCategory] WHERE IsDeleted = 0;
END;
GO