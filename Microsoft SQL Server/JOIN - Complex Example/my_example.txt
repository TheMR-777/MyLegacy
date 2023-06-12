SELECT 
	Orders.OrderID,
	CustomerName as [Customer Names],
	CookieName as [Cookie Name],
	OrderTotal as [$],
	Country,
	OrderDate

FROM dbo.Orders
JOIN Customers ON Customers.CustomerID = Orders.CustomerID
JOIN Order_Product ON Order_Product.OrderID = Orders.OrderID
JOIN Product ON Order_Product.CookieID = Product.CookieID
