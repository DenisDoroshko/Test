ALTER TABLE dbo.EventSeat
ADD CONSTRAINT FK_AspNetUsers_EventSeat FOREIGN KEY (UserId)     
    REFERENCES dbo.AspNetUsers (Id)