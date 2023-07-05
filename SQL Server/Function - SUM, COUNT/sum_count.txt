SELECT
	COUNT(*) as 'Entities',
	SUM(OrderTotal) as 'Grand Total'
FROM Orders