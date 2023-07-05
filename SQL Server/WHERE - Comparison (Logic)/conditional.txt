SELECT
	*
FROM dbo.Orders
WHERE NOT OrderTotal > 1000
