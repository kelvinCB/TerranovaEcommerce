USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 10-06-2025				--
-- Description: Make a update	--
-- in dbo.User table			--
----------------------------------
CREATE PROCEDURE [dbo].[UpdateUser]
	@IdUser int,
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@PhoneNumber NVARCHAR(20) = NULL,
	@DateOfBirth DATETIME = NULL,
	@Gender CHAR(1) = NULL,
	@Country VARCHAR(50) = NULL,
	@City VARCHAR(50) = NULL,
	@State VARCHAR(50) = NULL,
	@Address VARCHAR(50) = NULL,
	@PostalCode VARCHAR(20) = NULL,
	@EmailAddress VARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	-- Update Database User
	BEGIN TRY
		BEGIN TRAN
		UPDATE [dbo].[User] SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber,
		DateOfBirth = @DateOfBirth, Gender = @Gender, Country = @Country, City = @City, State = @State,
		Address = @Address, PostalCode = @PostalCode, EmailAddress = @EmailAddress, UpdateAt = GETDATE()
		WHERE Id = @IdUser;
		COMMIT;
	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH
END;
GO