USE [Terranova];
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 10-06-2025				--
-- Description: Make a insert	--
-- in dbo.User table			--
----------------------------------
CREATE PROCEDURE [dbo].[CreateUser]
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@PhoneNumber NVARCHAR(20) = NULL,
	@DateOfBirth DATETIME = NULL,
	@Gender CHAR(1) = NULL,
	@PasswordHash NVARCHAR(225) = NULL,
	@Salt NVARCHAR(225) = NULL,
	@RoleHash NVARCHAR(225) = NULL,
	@Country VARCHAR(50) = NULL,
	@City VARCHAR(50) = NULL,
	@State VARCHAR(50) = NULL,
	@Address VARCHAR(50) = NULL,
	@PostalCode VARCHAR(20) = NULL,
	@EmailAddress VARCHAR(50) = NULL
	AS
BEGIN
	SET NOCOUNT ON;

	-- Insert data in User table
	INSERT INTO [dbo].[User] (FirstName, LastName, PhoneNumber, DateOfBirth, Gender, PasswordHash, Salt,
	RoleHash, Country, City, State, Address, PostalCode, EmailAddress)
	VALUES (@FirstName, @LastName, @PhoneNumber, @DateOfBirth, @Gender, @PasswordHash, @Salt, @RoleHash,
	@Country, @City, @State, @Address, @PostalCode, @EmailAddress)
END;
GO