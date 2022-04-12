  CREATE OR ALTER PROCEDURE [dbo].[MyProc1]
	@id int,
	@name NVARCHAR(20)
  AS 
	DECLARE @int var1;
	IF ( @c = 0 ) 
	BEGIN
		RETURN -60003
	END
	RETURN 0
  
  CREATE OR ALTER PROCEDURE [dbo].[MyProc2]
        @name NVARCHAR(20),
	@id bigint OUTPUT,
	@flat int OUTPUT,
  AS 
	DECLARE @int var1;
	IF ( @c = 0 ) 
	BEGIN
		RETURN -60003
	END
	RETURN 10
	
CREATE OR ALTER PROCEDURE [dbo].[MyProc2]
	@id bigint,
  AS 
	RETURN 10
