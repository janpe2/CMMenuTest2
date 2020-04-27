CREATE PROCEDURE [dbo].[Dish_GetById]
	@Id int
AS
begin
	SELECT * 
	from dbo.Dish 
	where Id=@Id;
end
