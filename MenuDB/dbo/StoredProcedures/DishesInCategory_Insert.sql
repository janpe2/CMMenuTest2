CREATE PROCEDURE [dbo].[DishesInCategory_Insert]
	@DishId int,
	@CategoryId int,
	@MenuId int
AS
begin
	insert into dbo.DishesInCategory (DishId, CategoryId, MenuId)
	values (@DishId, @CategoryId, @MenuId);
end
