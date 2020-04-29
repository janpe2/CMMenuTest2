CREATE PROCEDURE [dbo].[DishesInCategory_GetDishes]
	@CategoryId int,
	@MenuId int
AS
begin
	-- Does not work!
	select DishesInCategory.DishId from DishesInCategory 
    join Dish on CategoryId=@CategoryId and MenuId=@MenuId
end
