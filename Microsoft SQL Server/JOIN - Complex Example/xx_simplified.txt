SELECT 
	o.OrderID,
	CustomerName as [Customer Names],
	CookieName as [Cookie Name],
	OrderTotal as [$],
	Country,
	OrderDate

FROM dbo.Orders o
JOIN Customers c ON c.CustomerID = o.CustomerID
JOIN Order_Product op ON op.OrderID = o.OrderID
JOIN Product p ON op.CookieID = p.CookieID
