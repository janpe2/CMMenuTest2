CREATE TABLE [dbo].[Dish]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NCHAR(50) NULL, 
    [Description] NCHAR(50) NULL, 
    [Price] FLOAT NULL, 
    [ContainsLactose] BIT NULL, 
    [ContainsGluten] BIT NULL, 
    [ContainsFish] BIT NULL
)
