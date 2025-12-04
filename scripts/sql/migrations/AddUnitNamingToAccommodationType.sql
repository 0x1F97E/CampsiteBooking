-- Add UnitNamingPrefix and UnitNamingPattern columns to AccommodationTypes table
-- These columns allow each accommodation type to have custom unit naming

-- Check if columns exist before adding them
SET @dbname = DATABASE();
SET @tablename = 'AccommodationTypes';
SET @columnname1 = 'UnitNamingPrefix';
SET @columnname2 = 'UnitNamingPattern';

SET @preparedStatement1 = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (table_name = @tablename)
      AND (table_schema = @dbname)
      AND (column_name = @columnname1)
  ) > 0,
  'SELECT 1',
  CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN ', @columnname1, ' VARCHAR(50) NULL')
));
PREPARE alterIfNotExists1 FROM @preparedStatement1;
EXECUTE alterIfNotExists1;
DEALLOCATE PREPARE alterIfNotExists1;

SET @preparedStatement2 = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (table_name = @tablename)
      AND (table_schema = @dbname)
      AND (column_name = @columnname2)
  ) > 0,
  'SELECT 1',
  CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN ', @columnname2, ' VARCHAR(50) NULL DEFAULT ''Numbers''')
));
PREPARE alterIfNotExists2 FROM @preparedStatement2;
EXECUTE alterIfNotExists2;
DEALLOCATE PREPARE alterIfNotExists2;

-- Set default values for existing records
UPDATE AccommodationTypes 
SET UnitNamingPrefix = '' 
WHERE UnitNamingPrefix IS NULL;

UPDATE AccommodationTypes 
SET UnitNamingPattern = 'Numbers' 
WHERE UnitNamingPattern IS NULL;

