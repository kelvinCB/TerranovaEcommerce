USE [Terranova]
GO
-----------------------------------
-- Autor: Briangel Abreu		 --
-- Date: 10-06-2025				 --
-- Description: Delete Shipping	 --
-- Address in dbo.ShippingAddress--
-----------------------------------
CREATE PROCEDURE [dbo].[DeleteShippingAddress]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Check if the ShippingAddress is default
	DECLARE @IsShippingAddressDefault BIT;
	SELECT @IsShippingAddressDefault = IsDefault FROM [dbo].[ShippingAddress] WHERE Id = @Id;

	-- Throw a error if the user try to delete the default Shipping Address
	IF @IsShippingAddressDefault = 1
	BEGIN
		throw 50001, 'Cannot delete a default shipping address. Please set another address as default before deleting this one.', 1;
		return;
	END;

	UPDATE [dbo].[ShippingAddress] SET IsDeleted = 1 WHERE Id = @Id;
END;
GO