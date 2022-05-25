CREATE TABLE [dbo].[Area]
(
	[Id] int identity primary key,
	[LayoutId] int NOT NULL,
	[Description] nvarchar(200) NOT NULL,
	[CoordX] int NOT NULL,
	[CoordY] int NOT NULL, 
    [Color] NVARCHAR(30) NOT NULL, 
    [Width] INT NOT NULL, 
    [Height] INT NOT NULL,
)
