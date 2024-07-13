
-- LittleOne

SELECT
	PayRate AS 'Pay',
	STRING_AGG(FirstName + ' ' + LastName, ', ') AS 'Names'
FROM Employees
WHERE PayRate > 50
GROUP BY PayRate
HAVING COUNT(1) > 1
ORDER BY Pay DESC

-- This query selects the pay rate and concatenates employee names 
-- for those with a pay rate greater than 50, grouping them by pay 
-- rate, and filtering for groups with more than one employee. The 
-- results are then sorted in descending order by pay rate.
