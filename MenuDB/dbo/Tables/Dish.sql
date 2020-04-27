CREATE TABLE [dbo].[Dish]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(50) NULL, 
    [Price] FLOAT NULL, 
    [ContainsLactose] BIT NULL, 
    [ContainsGluten] BIT NULL, 
    [ContainsFish] BIT NULL
)
