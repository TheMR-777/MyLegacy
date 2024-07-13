CREATE PROCEDURE
	-- Name
	dbo.my_sp_SelectEmployee

	-- Parameters
	@MyName varchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM Employees
	WHERE FirstName = @MyName;
END
GO

EXEC my_sp_SelectEmployee [TheMR]
