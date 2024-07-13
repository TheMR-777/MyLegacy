-- LittleOne

SELECT
	CustomerID,
	STRING_AGG(City, ', ')
FROM Locations
GROUP BY CustomerID