CREATE TABLE [dbo].[Venue]
(
	[Id] int identity primary key,
	[Name] NVARCHAR(120) NOT NULL,
	[Description] nvarchar(120) NOT NULL,
	[Address] nvarchar(200) NOT NULL,
	[Phone] nvarchar(30), 
)
