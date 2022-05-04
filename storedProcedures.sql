 CREATE OR ALTER PROCEDURE [dbo].[MyProc2]
  @age bigint,
	@id bigint OUTPUT = NULL,
  @name nvarchar(100) = 'some value'
  AS 
	DECLARE @int var1;
	IF ( @c = 0 ) 
	BEGIN
		RETURN -60003
	END
	RETURN 0
  
  procedure insert_row (
         last_name                in     varchar2,
         first_name               in     varchar2,
         country                   in    varchar2 default null,
         stateid                  in     varchar2 default null,
         account_number           in out number,
         alternatelastname        in     varchar2 default null,
         alternatefirstname       in     varchar2 default null)
     is
         acc_number   varchar2 (100);
         cust_pwd     varchar2 (15);
     begin
         if account_number is null
         then
             acc_number := get_next_account_number ();
 
             select acc_number into account_number from dual;
         else
             acc_number := account_number;
         end if;
     end insert_row;
