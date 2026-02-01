USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 11-06-2025				--
-- Description: Show Category	--
-- Information					--
----------------------------------
CREATE PROCEDURE [dbo].[GetCategory]
AS
BEGIN
	SET NOCOUNT ON;

	-- Select category information
	SELECT Id, Name, Description FROM [dbo].[Category] WHERE IsDeleted = 0;
END;
GO