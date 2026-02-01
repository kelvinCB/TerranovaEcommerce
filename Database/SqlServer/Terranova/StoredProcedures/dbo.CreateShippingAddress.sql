USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 10-06-2025				--
-- Description: Insert address	--
-- in dbo.ShippingAddress table	--
----------------------------------
CREATE PROCEDURE [dbo].[CreateShippingAddress]
	@UserId INT,
	@FullName VARCHAR(100),
	@PhoneNumber NVARCHAR(20),
	@AddressLine1 VARCHAR(100),
	@AddressLine2 VARCHAR(100) = NULL,
	@Country VARCHAR(50),
	@City VARCHAR(50),
	@State VARCHAR(50),
	@PostalCode NVARCHAR(20),
	@IsDefault BIT = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF @IsDefault = 1
	BEGIN
		-- Check if the user has another default shipping address
		DECLARE @DefaultShippingAddressId INT;
		SELECT @DefaultShippingAddressId = Id from ShippingAddress WHERE UserId = @UserId AND IsDefault = 1;

		-- If User has another default shipping address then update this shipping address has not default
		IF @DefaultShippingAddressId > 0
		BEGIN
			UPDATE [dbo].[ShippingAddress] SET IsDefault = 0 WHERE Id = @DefaultShippingAddressId;
		END;
	END;

	-- Insert Shipping User Address
	BEGIN TRY
		BEGIN TRAN
			INSERT INTO [dbo].[ShippingAddress] (UserId, FullName, PhoneNumber, AddressLine1, AddressLine2,
			Country, City, State, PostalCode, IsDefault)
			VALUES (@UserId, @FullName, @PhoneNumber, @AddressLine1, @AddressLine2, @Country, @City,
			@State, @PostalCode, @IsDefault)
	END TRY
	BEGIN CATCH
		THROW;
	END CATCH
END;
GO