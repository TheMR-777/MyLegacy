-- w3schools

SELECT GROUP_CONCAT(CustomerID, ', ') AS 'Customer IDs', Country
FROM Customers
GROUP BY Country;