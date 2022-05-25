CREATE TABLE [dbo].[Event]
(
	[Id] int primary key identity,
	[Name] nvarchar(120) NOT NULL,
	[Description] nvarchar(max) NOT NULL,
	[Start] DATETIME NOT NULL, 
    [Finish] DATETIME NOT NULL,
	[LayoutId] int NOT NULL, 
    [Image] NVARCHAR(MAX) NULL, 
)
