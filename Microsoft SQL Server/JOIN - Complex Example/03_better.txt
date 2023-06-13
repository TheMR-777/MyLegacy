
-- Format: Customer, Product, Country, State, Date, Quantity, Price, Total

SELECT
	c.CustomerName as [Customer],
	pro.CookieName as [Product],
	c.Address,
	o.OrderDate as [Date],
	op.Quantity,
	pro.CostPerCookie as [Price],
	o.OrderTotal as [Total]
FROM Orders o
JOIN Customers c ON o.CustomerID = c.CustomerID
JOIN Order_Product op ON o.OrderID = op.OrderID
JOIN Product pro ON op.CookieID = pro.CookieID