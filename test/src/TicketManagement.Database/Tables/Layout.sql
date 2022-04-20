CREATE TABLE [dbo].[Layout]
(
	[Id] int identity primary key,
	[Name] NVARCHAR(120) NOT NULL,
	[VenueId] int NOT NULL,
	[Description] nvarchar(120) NOT NULL, 
    [Width] INT NOT NULL, 
    [Height] INT NOT NULL, 
)
