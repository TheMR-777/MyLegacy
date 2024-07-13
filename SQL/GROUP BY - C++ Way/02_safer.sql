SELECT
	Domain AS 'Domain Names',
	STRING_AGG(Name, ', ') AS 'Names'
FROM
(
	SELECT
		FirstName AS 'Name',
		Email AS 'Email',
		SUBSTRING(Email, CHARINDEX('.', Email, CHARINDEX('@', Email)) + 1, LEN(Email)) AS 'Domain'
	FROM Employees
	UNION
	SELECT
		Name AS 'Name',
		Email AS 'Email',
		SUBSTRING(Email, CHARINDEX('.', Email, CHARINDEX('@', Email)) + 1, LEN(Email)) AS 'Domain'
	FROM Customers
) AS Result
GROUP BY Domain