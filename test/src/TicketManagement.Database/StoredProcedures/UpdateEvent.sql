CREATE PROCEDURE [dbo].[UpdateEvent]
	@Id int = 0,
	@Name nvarchar(120),
	@Description nvarchar(MAX),
	@Start datetime,
	@Finish datetime,
	@LayoutId int,
	@Image nvarchar(MAX)
AS
	UPDATE Event 
	SET Name=@Name,Description=@Description,Start=@Start,Finish=@Finish,LayoutId=@LayoutId,Image=@Image
	WHERE Id=@Id
