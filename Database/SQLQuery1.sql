--creating a database named DSIP
--CREATE DATABASE DSIP;

--using the DSIP db
USE DSIP;


--creating shipments table
IF NOT EXISTS (     --checking that table already exists so this wont give error when run multiple times
    SELECT *
    FROM sys.tables
    WHERE name = 'Shipments'
)
BEGIN

CREATE TABLE Shipments
(
    ShipmentId INT IDENTITY(1,1) PRIMARY KEY,

    AWBNumber NVARCHAR(20) NOT NULL UNIQUE,

    SenderName NVARCHAR(100) NOT NULL,

    ReceiverName NVARCHAR(100) NOT NULL,

    Origin NVARCHAR(100) NOT NULL,

    Destination NVARCHAR(100) NOT NULL,

    WeightKg DECIMAL(8,2) NOT NULL
        CHECK (WeightKg > 0),

    Status NVARCHAR(30) NOT NULL
        DEFAULT ('Booked'),

    BookedAt DATETIME NOT NULL
        DEFAULT (GETDATE()),

    DeliveredAt DATETIME NULL
);

END
GO

--creating a non clustered index(IX_Shipments_Status) 
--for fast retrieval like on filter processes
IF NOT EXISTS
(
    SELECT *
    FROM sys.indexes
    WHERE name = 'IX_Shipments_Status'
      AND object_id = OBJECT_ID('Shipments')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_Shipments_Status
    ON Shipments(Status);
END
GO


--STORED PROCEDURES
--To get all shipments ordered by Booking Date
CREATE OR ALTER PROCEDURE usp_GetAllShipments
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Shipments
    ORDER BY BookedAt DESC;
END
GO

--stored procedure to get shipments by AIB number 
CREATE OR ALTER PROCEDURE usp_GetShipmentByAWB
(
    @AWBNumber NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM Shipments
    WHERE AWBNumber = @AWBNumber;
END
GO

--procedure to update shipment status
CREATE OR ALTER PROCEDURE usp_UpdateShipmentStatus
(
    @AWBNumber NVARCHAR(20),
    @NewStatus NVARCHAR(30)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Shipments
    SET
        Status = @NewStatus,
        DeliveredAt = CASE
                        WHEN @NewStatus = 'Delivered'
                        THEN GETDATE()
                        ELSE DeliveredAt
                      END
    WHERE AWBNumber = @AWBNumber;
END
GO


--idempotant view script to get shipment dashboard data
IF OBJECT_ID('vw_ShipmentDashboard', 'V') IS NOT NULL
    DROP VIEW vw_ShipmentDashboard;
GO

CREATE VIEW vw_ShipmentDashboard
AS
SELECT
    AWBNumber,
    SenderName,
    ReceiverName,
    Origin,
    Destination,
    Status,    
    BookedAt,
    DeliveredAt
FROM Shipments;
GO


--SEED DATA INSERTION
IF NOT EXISTS (SELECT 1 FROM Shipments WHERE AWBNumber = 'DEL2025001')
BEGIN
INSERT INTO Shipments
(AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status)
VALUES
('DEL2025001','Rahul Sharma','Amit Kumar','Delhi','Mumbai',2.50,'Booked');
END

IF NOT EXISTS (SELECT 1 FROM Shipments WHERE AWBNumber = 'DEL2025002')
BEGIN
INSERT INTO Shipments
(AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status)
VALUES
('DEL2025002','Priya Singh','Neha Patel','Hyderabad','Pune',1.25,'In Transit');
END

IF NOT EXISTS (SELECT 1 FROM Shipments WHERE AWBNumber = 'DEL2025003')
BEGIN
INSERT INTO Shipments
(AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status)
VALUES
('DEL2025003','Arjun Reddy','Kiran Rao','Chennai','Bengaluru',5.00,'Out for Delivery');
END

IF NOT EXISTS (SELECT 1 FROM Shipments WHERE AWBNumber = 'DEL2025004')
BEGIN
INSERT INTO Shipments
(AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status, DeliveredAt)
VALUES
('DEL2025004','Suresh Kumar','Lakshmi Devi','Delhi','Jaipur',3.40,'Delivered',GETDATE()-5);
END

IF NOT EXISTS (SELECT 1 FROM Shipments WHERE AWBNumber = 'DEL2025005')
BEGIN
INSERT INTO Shipments
(AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status, DeliveredAt)
VALUES
('DEL2025005','Meena Joshi','Rohit Sharma','Nagpur','Surat',6.20,'Delivered',GETDATE()-2);
END

IF NOT EXISTS (SELECT 1 FROM Shipments WHERE AWBNumber = 'DEL2025006')
BEGIN
INSERT INTO Shipments
(AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status)
VALUES
('DEL2025006','Anjali Verma','Deepak Singh','Lucknow','Patna',1.80,'RTO');
END

IF NOT EXISTS (SELECT 1 FROM Shipments WHERE AWBNumber = 'DEL2025007')
BEGIN
INSERT INTO Shipments
(AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status)
VALUES
('DEL2025007','Vikram Shah','Pooja Nair','Ahmedabad','Goa',4.75,'Booked');
END

IF NOT EXISTS (SELECT 1 FROM Shipments WHERE AWBNumber = 'DEL2025008')
BEGIN
INSERT INTO Shipments
(AWBNumber, SenderName, ReceiverName, Origin, Destination, WeightKg, Status)
VALUES
('DEL2025008','Karthik R','Sneha Iyer','Coimbatore','Kochi',2.10,'In Transit');
END
GO

