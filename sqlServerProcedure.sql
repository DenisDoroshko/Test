  CREATE OR ALTER PROCEDURE [dbo].[MyProc1]
	@id int
  AS 
	DECLARE @int var1;
	IF ( @c = 0 ) 
	BEGIN
		RETURN -60003
	END
	RETURN 0
  
  CREATE OR ALTER PROCEDURE [dbo].[MyProc2]
	@id bigint OUTPUT
  AS 
	DECLARE @int var1;
	IF ( @c = 0 ) 
	BEGIN
		RETURN -60003
	END
	RETURN 0
