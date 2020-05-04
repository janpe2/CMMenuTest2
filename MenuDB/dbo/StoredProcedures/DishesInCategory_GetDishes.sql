CREATE PROCEDURE [dbo].[DishesInCategory_GetDishes]
	@CategoryId int,
	@MenuId int
AS
begin
    -- This uses JOIN to "resolve" the foreign keys DishId
    -- and get the referenced dishes from Dish table.
    SELECT Dish.*
    FROM DishesInCategory
    INNER JOIN Dish 
    ON DishesInCategory.DishId=Dish.Id
    WHERE DishesInCategory.CategoryId=@CategoryId AND DishesInCategory.MenuId=@MenuId
    ORDER BY Dish.Name;
end