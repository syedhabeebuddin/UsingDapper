CREATE PROCEDURE [dbo].[GetContact]
	@Id int	
AS
BEGIN
	SELECT [Id],
	       [FirstName],
		   [LastName],
		   [Company],
		   [Title],
		   [Email]
	From dbo.Contacts
	Where Id=@Id;

	Select Id,
	       ContactId,
		   AddressType,
		   StreetAddress,
		   City,
		   StateId,
		   PostalCode
	From dbo.Addresses 
	where ContactId=@Id;
	End


