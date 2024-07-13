CREATE FUNCTION dbo.GetTopLevelDomain (@email VARCHAR(255))
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @tld VARCHAR(10)
    SET @tld = RIGHT(@email, CHARINDEX('.', REVERSE(@email)) - 1)
    RETURN @tld
END