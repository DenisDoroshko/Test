CREATE PROCEDURE [dbo].[CreateEvent]
	@Id int OUTPUT,
	@Name nvarchar(120),
	@Description nvarchar(MAX),
	@Start datetime,
	@Finish datetime,
	@LayoutId int,
	@Image nvarchar(MAX)
AS
	DECLARE @eventId int
	INSERT Event(Name,Description,Start,Finish,LayoutId,Image) VALUES (@Name,@Description,@Start,@Finish,@LayoutId,@Image)
	SET @eventId = CONVERT(int,SCOPE_IDENTITY())
	
	DECLARE @i int
	DECLARE @rowsNumber int
	DECLARE @AreaTable TABLE (Idx int Primary Key IDENTITY(1,1),Id int, Description nvarchar(200),CoordX int, CoordY int)
	DECLARE @eventAreaId int
	DECLARE @currentAreaId int

	INSERT @AreaTable
	SELECT Id,Description,CoordX,CoordY FROM Area WHERE LayoutId = @LayoutId

	SET @rowsNumber = (SELECT COUNT(*) FROM @AreaTable)
	SET @i = 1
	IF @rowsNumber > 0
		WHILE (@i <= (SELECT MAX(Idx) FROM @AreaTable))
		BEGIN
			SET @currentAreaId = (SELECT Id FROM @AreaTable WHERE Idx = @i)

			INSERT INTO EventArea (EventId,Description,CoordX,CoordY,Price, Color) SELECT @eventId,Description,CoordX,CoordY,0, Color
			FROM Area WHERE LayoutId = @LayoutId AND Id = @currentAreaId
			SET @eventAreaId = CONVERT(int,SCOPE_IDENTITY())

			INSERT INTO EventSeat (EventAreaId,Row,Number,State) SELECT @eventAreaId,Row,Number,0
			FROM Seat WHERE AreaId = @currentAreaId
			SET @i = @i + 1
		END
SELECT @Id = @eventId



