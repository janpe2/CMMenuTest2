CREATE TABLE [dbo].[DishesInCategory]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CategoryId] INT NULL, 
    [DishId] INT NULL, 
    [MenuId] INT NULL, 
    CONSTRAINT [FK_DishesInCategory_Dish] FOREIGN KEY ([DishId]) REFERENCES [Dish]([Id]), 
    CONSTRAINT [FK_DishesInCategory_Menu] FOREIGN KEY ([MenuId]) REFERENCES [Menu]([Id])
)
