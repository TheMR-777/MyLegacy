-- LittleOne

SELECT
	PayRate, STRING_AGG(FirstName, ', ') AS Stakeholders
FROM Employees
GROUP BY PayRate