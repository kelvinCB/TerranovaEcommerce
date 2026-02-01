USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 10-06-2025				--
-- Description: Update address	--
-- in dbo.ShippingAddress table	--
----------------------------------
CREATE PROCEDURE [dbo].[UpdateShippingAddress]
	@Id INT,
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
		IF (@DefaultShippingAddressId > 0 AND @DefaultShippingAddressId <> @Id)
		BEGIN
			UPDATE [dbo].[ShippingAddress] SET IsDefault = 0 WHERE Id = @DefaultShippingAddressId;
		END;
	END;

	-- Update Shipping Address
	BEGIN TRY
		BEGIN TRAN

		UPDATE [dbo].[ShippingAddress] SET FullName = @FullName, PhoneNumber = @PhoneNumber,
		AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2, Country = @Country,
		City = @City, State = @State, PostalCode = @PostalCode, IsDefault = @IsDefault, UpdateAt = GETDATE()
		WHERE Id = @Id;

		COMMIT;

	END TRY
	BEGIN CATCH
		ROLLBACK;
		THROW;
	END CATCH;
END;
GO