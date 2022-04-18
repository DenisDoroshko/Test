INSERT State(stateId,name,abbreviation,listOrder) VALUES(1,'Alabama',SELECT name from names where id = 123,'1')
INSERT SeqAccountActivityId (x) VALUES (NULL)
INSERT INTO mytable (id, enumcode, comment) VALUES (123, 14, '')

INSERT INTO events
  (
   EVENTID,
   ZONEID,
   NAME
  )
  VALUES
  (
   123,
   myid123,
   'my value 1'
  );
