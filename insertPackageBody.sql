
   
CREATE OR REPLACE PACKAGE BODY insert_package
IS
   ---Insert Row
   PROCEDURE insert_row (
      name           VARCHAR2,
      description    VARCHAR,
      details    VARCHAR,
      new_id         OUT   NUMBER
   )
   IS
      num  number(12):=null;
      num2  number(12):=null;
   BEGIN
      if (num > 0 ) then
         num2 := 10;
      end if;
   END insert_row;
   
   PROCEDURE select_info (
      id         NUMBER,
      name       OUT   VARCHAR
   )
   IS
      num  number(12):=null;
      num2  number(12):=null;
   BEGIN
      if (num > 0 ) then
         num2 := 10;
      end if;
   END select_info;
END insert_package;
