CREATE PROCEDURE [dbo].[DishesInCategory_GetMenu]
	@MenuId int
AS
	select * from Menu where Id=@MenuId
	ORDER BY Name
RETURN 0
