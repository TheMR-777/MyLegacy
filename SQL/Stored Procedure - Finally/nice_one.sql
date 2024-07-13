-- Definition:
CREATE PROCEDURE 
	-- Name:
	dbo.my_sp_WorkDone_Nice
AS
BEGIN
	SET NOCOUNT ON;

	-- Statement:
	SELECT * FROM vs_WorkDone_Descriptive;
END
GO

-- Run:
exec my_sp_WorkDone_Nice
