-- LittleOne

SELECT
	LastName AS 'Name',
	Email,
	CHARINDEX('.', REVERSE(Email)) AS 'Dot from Last',
	LEN(Email) AS 'Total Length',
	LEN(Email) - CHARINDEX('.', REVERSE(Email)) + 1 AS 'Dot from Start',
	SUBSTRING(Email, 1, LEN(Email) - CHARINDEX('.', REVERSE(Email)) + 1) AS 'Till Dot',
	SUBSTRING(Email, LEN(Email) - CHARINDEX('.', REVERSE(Email)) + 2, LEN(Email)) AS 'Interested One'
FROM Employees