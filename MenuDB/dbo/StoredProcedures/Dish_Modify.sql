CREATE PROCEDURE [dbo].[Dish_Modify]
	@Id int,
	@Name nvarchar(50),
	@Description nvarchar(50),
	@Price float,
	@ContainsLactose bit,
	@ContainsGluten bit, 
	@ContainsFish bit
AS
begin
	update dbo.Dish 
	set Name=@Name, Description=@Description, Price=@Price, 
		ContainsLactose=@ContainsLactose, ContainsGluten=@ContainsGluten, ContainsFish=@ContainsFish
	where Id=@Id;
end
