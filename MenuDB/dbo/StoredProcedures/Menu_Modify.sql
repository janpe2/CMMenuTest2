CREATE PROCEDURE [dbo].[Menu_Modify]
	@NewName nvarchar(50),
	@NewDescr nvarchar(50),
	@MenuId int
AS
	update Menu set Name=@NewName, Description=@NewDescr where Id=@MenuId
RETURN 0
