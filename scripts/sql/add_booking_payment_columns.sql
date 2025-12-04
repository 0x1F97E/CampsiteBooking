-- Add missing columns to Bookings table
ALTER TABLE Bookings ADD COLUMN GuestId INT NULL AFTER BookingId;
ALTER TABLE Bookings ADD COLUMN CampsiteId INT NULL AFTER GuestId;
ALTER TABLE Bookings ADD COLUMN AccommodationSpotId INT NULL AFTER CampsiteId;
ALTER TABLE Bookings ADD COLUMN AccommodationTypeId INT NULL AFTER AccommodationSpotId;
ALTER TABLE Bookings ADD COLUMN Period TEXT NULL AFTER AccommodationTypeId;
ALTER TABLE Bookings ADD COLUMN Status VARCHAR(50) NULL AFTER Period;
ALTER TABLE Bookings ADD COLUMN BasePrice TEXT NULL AFTER Status;
ALTER TABLE Bookings ADD COLUMN TotalPrice TEXT NULL AFTER BasePrice;
ALTER TABLE Bookings ADD COLUMN NumberOfAdults INT NULL AFTER TotalPrice;
ALTER TABLE Bookings ADD COLUMN NumberOfChildren INT NULL AFTER NumberOfAdults;
ALTER TABLE Bookings ADD COLUMN SpecialRequests TEXT NULL AFTER NumberOfChildren;
ALTER TABLE Bookings ADD COLUMN CreatedDate DATETIME NULL AFTER SpecialRequests;
ALTER TABLE Bookings ADD COLUMN LastModifiedDate DATETIME NULL AFTER CreatedDate;
ALTER TABLE Bookings ADD COLUMN CancellationDate DATETIME NULL AFTER LastModifiedDate;

-- Add missing columns to Payments table
ALTER TABLE Payments ADD COLUMN BookingId INT NULL AFTER PaymentId;
ALTER TABLE Payments ADD COLUMN Amount TEXT NULL AFTER BookingId;
ALTER TABLE Payments ADD COLUMN Method VARCHAR(50) NULL AFTER Amount;
ALTER TABLE Payments ADD COLUMN TransactionId VARCHAR(100) NULL AFTER Method;
ALTER TABLE Payments ADD COLUMN Status VARCHAR(50) NULL AFTER TransactionId;
ALTER TABLE Payments ADD COLUMN PaymentDate DATETIME NULL AFTER Status;
ALTER TABLE Payments ADD COLUMN RefundDate DATETIME NULL AFTER PaymentDate;
ALTER TABLE Payments ADD COLUMN RefundAmount TEXT NULL AFTER RefundDate;
ALTER TABLE Payments ADD COLUMN CreatedDate DATETIME NULL AFTER RefundAmount;

