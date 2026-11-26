-- Q2-01 English pilot database, SQL Server.
-- Run the complete script in SQL Server Management Studio or sqlcmd.
-- Requires permission to create a database and tables.
-- Existing Categories/Products tables are never overwritten or reseeded.

USE [master];
GO

IF DB_ID(N'PRN212_PE_Q2_01') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [PRN212_PE_Q2_01];');
END;
GO

USE [PRN212_PE_Q2_01];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- Keep schema creation and seed inserts in one batch and one transaction.
IF OBJECT_ID(N'dbo.Categories') IS NOT NULL
   OR OBJECT_ID(N'dbo.Products') IS NOT NULL
BEGIN
    THROW 50001, 'Setup stopped: Categories or Products already exists. Use a clean exam database. No existing data was changed.', 1;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    CREATE TABLE dbo.Categories
    (
        Id INT NOT NULL,
        Name NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_Categories PRIMARY KEY (Id)
    );

    CREATE TABLE dbo.Products
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(60) NOT NULL,
        LookupId INT NOT NULL,
        Amount DECIMAL(10,2) NOT NULL,
        WorkDate DATE NOT NULL,
        IsActive BIT NOT NULL,
        CONSTRAINT PK_Products PRIMARY KEY (Id),
        CONSTRAINT FK_Products_Categories FOREIGN KEY (LookupId)
            REFERENCES dbo.Categories (Id)
    );

    -- Name order (Alpha, Beta, Gamma) differs from Id order.
    -- Beta deliberately has no products in the initial dataset.
    INSERT INTO dbo.Categories (Id, Name)
    VALUES (10, N'Gamma'), (20, N'Alpha'), (30, N'Beta');

    -- Include boundary prices/dates, repeated prices and both active states.
    -- Duplicate product names are allowed by the paper.
    INSERT INTO dbo.Products (Name, LookupId, Amount, WorkDate, IsActive)
    VALUES
        (N'Notebook',       20,     0.00, '20260101', 1),
        (N'Blue Pen',       10,     5.50, '20260115', 1),
        (N'Pencil Set',     20,    10.00, '20260210', 0),
        (N'Paper Pack',     10,    20.00, '20260305', 1),
        (N'Folder',         20,    20.00, '20260412', 1),
        (N'Notebook',       10,    20.50, '20260520', 0),
        (N'Stapler',        20,    50.00, '20260601', 1),
        (N'Desk Organizer', 10,    75.25, '20260718', 0),
        (N'Desk Lamp',      20,   100.00, '20260808', 1),
        (N'Office Chair',   10,   250.00, '20260915', 1),
        (N'Office Desk',    20,   999.99, '20261030', 0),
        (N'Printer',        10, 10000.00, '20261231', 1);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;

SELECT Id, Name FROM dbo.Categories ORDER BY Name;
SELECT p.Id, p.Name, p.LookupId, c.Name AS Category,
       p.Amount, p.WorkDate, p.IsActive
FROM dbo.Products AS p
INNER JOIN dbo.Categories AS c ON c.Id = p.LookupId
ORDER BY p.Id;
GO
