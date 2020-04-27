CREATE PROCEDURE [dbo].[Dish_GetAllDishesSortedAlphabetically]
as
begin
	SELECT * from dbo.Dish
	order by Name
end
