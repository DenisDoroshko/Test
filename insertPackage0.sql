CREATE OR REPLACE PACKAGE insert_package
IS
   PROCEDURE insert_row (
      name           VARCHAR2,
      description    VARCHAR2,
      new_id         OUT   NUMBER
   );
   PROCEDURE select_info (
      id         NUMBER,
      name       OUT   VARCHAR2,
      name2       OUT   VARCHAR2
   );
END insert_package;
