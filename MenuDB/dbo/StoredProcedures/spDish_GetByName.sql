CREATE PROCEDURE [dbo].[spDish_GetByName]
	@Name nvarchar(50)
AS
begin
	SELECT * 
	from dbo.Dish 
	where Name=@Name;
end

