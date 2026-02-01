USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 10-06-2025				--
-- Description: User Information--
-- by Id						--
----------------------------------
CREATE PROCEDURE [dbo].[GetUserById]
	@IdUser INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Select User Information by Id
	SELECT Id, FirstName, LastName, PhoneNumber, DateOfBirth, Gender, Country, City, State, Address,
	PostalCode, EmailAddress
	FROM [dbo].[User] 
	WHERE Id = @IdUser AND IsDeleted = 0;
END;
GO