-- Add Amenities column to AccommodationTypes table to store comma-separated amenities
-- This allows each accommodation type to have its own set of amenities

ALTER TABLE AccommodationTypes 
ADD COLUMN Amenities TEXT NULL 
COMMENT 'Comma-separated list of amenities for this accommodation type';

-- Set default empty string for existing records
UPDATE AccommodationTypes 
SET Amenities = '' 
WHERE Amenities IS NULL;

