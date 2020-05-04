CREATE PROCEDURE [dbo].[Menu_Insert]
	@Name nvarchar(50),
	@Description nvarchar(50)
AS
	insert into Menu (Name, Description) values (@Name, @Description)
RETURN 0
