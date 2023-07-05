SELECT TOP (3)
	STRING_AGG(FirstName, ', ') AS 'First Names',
	STRING_AGG(LastName, ', ') AS 'Last Names'
FROM Employees