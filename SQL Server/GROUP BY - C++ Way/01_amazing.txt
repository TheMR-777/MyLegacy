SELECT
	New.Domain,
	STRING_AGG(New.Name, ', ') AS 'People'
FROM 
(
	SELECT
		FirstName + ' ' + LastName AS 'Name',
		SUBSTRING(Email, CHARINDEX('.', Email, CHARINDEX('@', Email)) + 1, LEN(Email)) AS 'Domain',
		Email
	FROM Employees
	UNION
	SELECT
		Name,
		SUBSTRING(Email, CHARINDEX('.', Email, CHARINDEX('@', Email)) + 1, LEN(Email)) AS 'Domain',
		Email
	FROM Customers
) New
GROUP BY New.Domain