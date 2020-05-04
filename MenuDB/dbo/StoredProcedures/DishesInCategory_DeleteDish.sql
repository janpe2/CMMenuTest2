CREATE PROCEDURE [dbo].[DishesInCategory_DeleteDish]
	@DishId int
AS
	delete from DishesInCategory where DishId=@DishId;
RETURN 0
