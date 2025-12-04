-- Migration: Add Address Fields to Campsites table
-- This migration replaces the Region column with StreetAddress, City, and PostalCode columns

-- Step 1: Add new address columns
ALTER TABLE Campsites
ADD COLUMN StreetAddress VARCHAR(255) NULL,
ADD COLUMN City VARCHAR(100) NULL,
ADD COLUMN PostalCode VARCHAR(20) NULL;

-- Step 2: Migrate data from Region to City (as a temporary measure)
UPDATE Campsites SET City = Region WHERE Region IS NOT NULL;

-- Step 3: Update with proper address data for seeded campsites
UPDATE Campsites SET 
    StreetAddress = 'Strandvejen 152',
    City = 'Charlottenlund',
    PostalCode = '2920'
WHERE Name = 'Copenhagen Beach Camp';

UPDATE Campsites SET 
    StreetAddress = 'Fyrvej 45',
    City = 'Skagen',
    PostalCode = '9990'
WHERE Name = 'Skagen North Point';

UPDATE Campsites SET 
    StreetAddress = 'Skovvej 78',
    City = 'Aarhus',
    PostalCode = '8000'
WHERE Name = 'Aarhus Forest Retreat';

UPDATE Campsites SET 
    StreetAddress = 'Campingvej 23',
    City = 'Odense',
    PostalCode = '5000'
WHERE Name = 'Odense Family Camp';

UPDATE Campsites SET 
    StreetAddress = 'Havnevej 12',
    City = 'Rønne',
    PostalCode = '3700'
WHERE Name = 'Bornholm Island Camp';

-- Step 4: Set default values for any remaining NULL values
UPDATE Campsites SET StreetAddress = '' WHERE StreetAddress IS NULL;
UPDATE Campsites SET City = '' WHERE City IS NULL;
UPDATE Campsites SET PostalCode = '' WHERE PostalCode IS NULL;

-- Step 5: Make columns NOT NULL
ALTER TABLE Campsites
MODIFY COLUMN StreetAddress VARCHAR(255) NOT NULL DEFAULT '',
MODIFY COLUMN City VARCHAR(100) NOT NULL DEFAULT '',
MODIFY COLUMN PostalCode VARCHAR(20) NOT NULL DEFAULT '';

-- Step 6: Drop the old Region column (optional - can be done later after verification)
-- ALTER TABLE Campsites DROP COLUMN Region;

-- Verification query
SELECT Id, Name, StreetAddress, City, PostalCode, Region FROM Campsites;

