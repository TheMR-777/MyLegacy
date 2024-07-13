SET IDENTITY_INSERT Employees ON;
INSERT INTO Employees (ID, FirstName, LastName, Email, PayRate, BillRate)
VALUES (2, 'Mr.', 'Strange', 'mr.strange@doctor.org', 1200, 1200);
SET IDENTITY_INSERT Employees OFF;