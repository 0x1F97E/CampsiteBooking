-- Add AreaSquareMeters column to AccommodationTypes table
-- This column stores the optional area in square meters for each accommodation type

-- Check if column exists before adding it
SET @dbname = DATABASE();
SET @tablename = 'AccommodationTypes';
SET @columnname = 'AreaSquareMeters';

SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (table_name = @tablename)
      AND (table_schema = @dbname)
      AND (column_name = @columnname)
  ) > 0,
  'SELECT 1',
  CONCAT('ALTER TABLE ', @tablename, ' ADD COLUMN ', @columnname, ' DECIMAL(10,2) NULL')
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

