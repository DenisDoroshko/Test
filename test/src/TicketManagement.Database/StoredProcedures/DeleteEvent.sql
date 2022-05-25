CREATE PROCEDURE [dbo].[DeleteEvent]
	@Id int = 0
AS
	DELETE FROM EventSeat WHERE EventAreaId = ANY(SELECT Id FROM EventArea WHERE EventId = @Id)
	DELETE FROM EventArea WHERE EventId=@Id
	DELETE FROM Event WHERE Id=@Id

