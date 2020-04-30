CREATE PROCEDURE [dbo].[DishesInCategory_GetDishes]
	@CategoryId int,
	@MenuId int
AS
begin
    -- Get the column DishId of all records in DishesInCategory table if the record has
    -- the specified @CategoryId and @MenuId. Then "resolve" the foreign keys DishId
    -- and get the referenced dishes from Dish table.
    SELECT Dish.*
    FROM DishesInCategory
    INNER JOIN Dish 
    ON DishesInCategory.DishId=Dish.Id and DishesInCategory.CategoryId=@CategoryId and DishesInCategory.MenuId=@MenuId;
end
