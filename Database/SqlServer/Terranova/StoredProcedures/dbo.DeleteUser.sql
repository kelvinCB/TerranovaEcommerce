USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 10-06-2025				--
-- Description: Delete user in	--
-- dbo.User table				--
----------------------------------
CREATE PROCEDURE [dbo].[DeleteUser]
	@IdUser INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Delete User Row
	UPDATE [dbo].[User] SET IsDeleted = 1 WHERE Id = @IdUser;
END;
GO