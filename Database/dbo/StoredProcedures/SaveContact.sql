CREATE PROCEDURE [dbo].[SaveContact]
	@Id                int output,
	@FirstName         varchar(50),
	@LastName	       varchar(50),
	@Title	           varchar(50),
	@Company	       varchar(50),
	@Email             varchar(50)	
AS
BEGIN
     Update Contacts
	 Set FirstName = @FirstName,
	     LastName  = @LastName,
		 Title     = @Title,
		 Email     = @Email,
		 Company   = @Company
	Where Id=@Id

	IF @@ROWCOUNT = 0
	BEGIN
	   INSERT INTO Contacts
	    (FirstName,
		 LastName,
		 Email,
		 Title,
		 Company)
	  Values (@FirstName,
	          @LastName,
			  @Email,
			  @Title,
			  @Company);

	  Set @Id = cast(SCOPE_IDENTITY() as int)
	END;
END;
	
