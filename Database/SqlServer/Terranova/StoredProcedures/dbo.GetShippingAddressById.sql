USE [Terranova]
GO
----------------------------------
-- Autor: Briangel Abreu		--
-- Date: 10-06-2025				--
-- Description: Shipping Address--
-- Information by Id			--
----------------------------------
CREATE PROCEDURE [dbo].[GetShippingAddressById]
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Select Shipping Address Information by Id
	SELECT Id, FullName, PhoneNumber, AddressLine1, AddressLine2, Country, City, State, PostalCode, IsDefault
	FROM [dbo].[ShippingAddress]
	WHERE Id = @Id AND IsDeleted = 0;
END;
GO