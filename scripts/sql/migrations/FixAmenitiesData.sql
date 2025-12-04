-- ============================================================================
-- FIX AMENITIES DATA IN ACCOMMODATION TYPES TABLE
-- ============================================================================
-- This script fixes the amenities data that was corrupted due to EF Core
-- change tracking not detecting changes to private fields.
--
-- ISSUE: Admin unchecked amenities but they still show on the homepage
-- CAUSE: EF Core wasn't marking the _amenities field as modified
-- SOLUTION: Manually update the database to match admin selections
--
-- Run this script to clean up the old data and reset amenities to defaults
-- ============================================================================

-- First, let's see the current state of amenities
SELECT Id, Type, CampsiteId, Amenities 
FROM AccommodationTypes 
ORDER BY CampsiteId, Type;

-- ============================================================================
-- OPTION 1: Reset ALL amenities to empty (then use admin UI to set them)
-- ============================================================================
-- Uncomment the following line to clear all amenities:
-- UPDATE AccommodationTypes SET Amenities = '';

-- ============================================================================
-- OPTION 2: Reset amenities to the seeder defaults
-- ============================================================================
-- This will restore the amenities to what the seeder originally intended

-- Copenhagen Beach Camp (CampsiteId = 1)
UPDATE AccommodationTypes 
SET Amenities = 'WiFi,Electric-Hookup,Water-Hookup,Heating,Kitchen,Bathroom' 
WHERE CampsiteId = 1 AND Type = 'Cabin';

UPDATE AccommodationTypes 
SET Amenities = 'Electric-Hookup,Fire-Pit,Picnic-Table' 
WHERE CampsiteId = 1 AND Type = 'Tent Site';

UPDATE AccommodationTypes 
SET Amenities = 'WiFi,Electric-Hookup,Heating,Bathroom,Kitchen,Air-Conditioning,Linens-Provided' 
WHERE CampsiteId = 1 AND Type = 'Glamping';

UPDATE AccommodationTypes 
SET Amenities = 'Electric-Hookup,Water-Hookup,Sewer-Hookup,WiFi' 
WHERE CampsiteId = 1 AND Type = 'RV Spot';

-- Skagen North Point (CampsiteId = 2)
UPDATE AccommodationTypes 
SET Amenities = 'WiFi,Electric-Hookup,Water-Hookup,Heating,Kitchen,Bathroom' 
WHERE CampsiteId = 2 AND Type = 'Cabin';

UPDATE AccommodationTypes 
SET Amenities = 'Electric-Hookup,Fire-Pit,Picnic-Table' 
WHERE CampsiteId = 2 AND Type = 'Tent Site';

-- Aarhus Forest Retreat (CampsiteId = 3)
UPDATE AccommodationTypes 
SET Amenities = 'WiFi,Electric-Hookup,Heating,Kitchen,Bathroom' 
WHERE CampsiteId = 3 AND Type = 'Cabin';

UPDATE AccommodationTypes 
SET Amenities = 'Fire-Pit,Picnic-Table' 
WHERE CampsiteId = 3 AND Type = 'Tent Site';

-- Odense Family Camp (CampsiteId = 4)
UPDATE AccommodationTypes 
SET Amenities = 'WiFi,Electric-Hookup,Heating,Kitchen,Bathroom' 
WHERE CampsiteId = 4 AND Type = 'Cabin';

UPDATE AccommodationTypes 
SET Amenities = 'Electric-Hookup,Fire-Pit,Picnic-Table' 
WHERE CampsiteId = 4 AND Type = 'Tent Site';

-- Bornholm Island Camp (CampsiteId = 5)
UPDATE AccommodationTypes 
SET Amenities = 'WiFi,Electric-Hookup,Water-Hookup,Heating,Kitchen,Bathroom' 
WHERE CampsiteId = 5 AND Type = 'Cabin';

UPDATE AccommodationTypes 
SET Amenities = 'Electric-Hookup,Fire-Pit,Picnic-Table' 
WHERE CampsiteId = 5 AND Type = 'Tent Site';

-- ============================================================================
-- OPTION 3: Fix specific RV Spot issue (only Kitchen should be checked)
-- ============================================================================
-- Based on your report, the RV Spot should only have "Kitchen" amenity
UPDATE AccommodationTypes 
SET Amenities = 'Kitchen' 
WHERE CampsiteId = 1 AND Type = 'RV Spot';

-- ============================================================================
-- Verify the changes
-- ============================================================================
SELECT Id, Type, CampsiteId, Amenities 
FROM AccommodationTypes 
ORDER BY CampsiteId, Type;

-- ============================================================================
-- NOTES:
-- ============================================================================
-- After running this script:
-- 1. Restart your application
-- 2. Navigate to http://localhost:5063/
-- 3. Verify that the RV Spot now shows only "Kitchen" amenity
-- 4. Navigate to http://localhost:5063/admin/campsites
-- 5. Verify that the admin UI shows only "Kitchen" checked for RV Spot
-- 6. Try unchecking/checking amenities - they should now persist correctly
-- ============================================================================

