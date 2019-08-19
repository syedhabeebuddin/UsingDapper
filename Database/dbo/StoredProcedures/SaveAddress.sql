CREATE PROCEDURE [dbo].[SaveAddress]
    @Id        int output,
	@ContactId int,
	@AddressType varchar(10),
	@StreetAddress varchar(50),
	@StateId     int,
	@City        varchar(50),
	@PostalCode  varchar(20)	
AS
BEGIN
    Update Addresses 
	Set ContactId = @ContactId,
	    AddressType = @AddressType,
		StreetAddress = @StreetAddress,
		StateId = @StateId,
		City = @City,
		PostalCode = @PostalCode
	where Id=@Id;

	IF @@ROWCOUNT = 0
	BEGIN
	INSERT INTO 
	Addresses (ContactId,
	           AddressType,
			   StateId,
			   StreetAddress,
			   City,
			   PostalCode)
	Values (@ContactId,
	        @AddressType,
			@StreetAddress,
			@StateId,
			@City,
			@PostalCode);
    
	Set @Id = cast(SCOPE_IDENTITY() as int)
END
END
	

