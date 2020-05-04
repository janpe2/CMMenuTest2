CREATE PROCEDURE [dbo].[DishesInCategory_DeleteMenu]
	@MenuId int
AS
	delete from DishesInCategory where MenuId=@MenuId;
RETURN 0
