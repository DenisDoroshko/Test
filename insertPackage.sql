CREATE OR REPLACE PACKAGE insert_package
IS
   PROCEDURE insert_row (
      name           VARCHAR2,
      description    VARCHAR2,
      new_id         OUT   NUMBER,
      id             NUMBER
   );
   PROCEDURE select_info (
      id         NUMBER,
      name       VARCHAR2
   );
   
   PROCEDURE select_info (
      id         NUMBER,
      name       VARCHAR2
   );
END insert_package;
