SELECT
	CustomerName AS 'Customer',
    FirstName || ' ' || LastName AS 'Employer',
	ShipperName AS 'Shipper',
    SupplierName AS 'Supplier',
    ProductName AS 'Product',
    CategoryName AS 'Category'
FROM Orders o

JOIN Customers c ON c.CustomerID = o.CustomerID
JOIN Employees e ON e.EmployeeID = o.EmployeeID
JOIN Shippers sh ON sh.ShipperID = o.ShipperID

JOIN OrderDetails d ON d.OrderID = o.OrderID
JOIN Products p ON d.ProductID = p.ProductID
JOIN Suppliers s ON p.SupplierID = s.SupplierID
JOIN Categories ct ON p.CategoryID = ct.CategoryID