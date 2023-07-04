--w3schools

SELECT Customers.CustomerName, Orders.OrderID
FROM Customers
LEFT JOIN Orders
ON Customers.CustomerID=Orders.CustomerID
--WHERE OrderID IS NULL
ORDER BY Customers.CustomerName;