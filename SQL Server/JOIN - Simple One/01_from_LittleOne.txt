

SELECT 
	j.[Name] AS 'Job', c.[Name] AS 'Customer'
FROM Jobs j

JOIN Customers c ON c.ID = j.CustomerID;