SELECT
	Domain AS 'Domain Names',
	STRING_AGG(Name, ', ') AS 'Names'
FROM
(
	SELECT
		FirstName AS 'Name',
		SUBSTRING(Email, LEN(Email) - CHARINDEX('.', REVERSE(Email)) + 2, LEN(Email)) AS 'Domain'
	FROM Employees
	UNION
	SELECT
		Name AS 'Name',
		SUBSTRING(Email, LEN(Email) - CHARINDEX('.', REVERSE(Email)) + 2, LEN(Email)) AS 'Domain'
	FROM Customers
) AS _
GROUP BY Domain