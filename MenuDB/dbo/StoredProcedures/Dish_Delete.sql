CREATE PROCEDURE [dbo].[Dish_Delete]
	@Id int
AS
	delete from Dish where Id=@Id
RETURN 0
