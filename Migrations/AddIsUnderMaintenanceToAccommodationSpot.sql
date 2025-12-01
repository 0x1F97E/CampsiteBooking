-- Add IsUnderMaintenance column to AccommodationSpots table
-- This replaces the need for the Status column to track maintenance status
-- When IsUnderMaintenance is true, the spot will not appear in search results

-- Check if column exists before adding
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'AccommodationSpots' 
    AND COLUMN_NAME = 'IsUnderMaintenance'
)
BEGIN
    ALTER TABLE AccommodationSpots
    ADD IsUnderMaintenance BIT NOT NULL DEFAULT 0;
    
    PRINT 'Added IsUnderMaintenance column to AccommodationSpots table';
END
ELSE
BEGIN
    PRINT 'IsUnderMaintenance column already exists in AccommodationSpots table';
END

-- Update existing spots that have Status = 'Maintenance' to have IsUnderMaintenance = 1
UPDATE AccommodationSpots
SET IsUnderMaintenance = 1
WHERE Status = 'Maintenance' OR Status = '3';

PRINT 'Migration completed: IsUnderMaintenance column added to AccommodationSpots';

